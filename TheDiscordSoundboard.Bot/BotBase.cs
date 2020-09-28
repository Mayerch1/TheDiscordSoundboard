using Discord;
using Discord.Audio;
using Discord.WebSocket;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using TheDiscordSoundboard.Models.Bot;
using TheDiscordSoundboard.Models.config;

namespace TheDiscordSoundboard.Bot
{
    public class BotBase
    {
        

        protected DiscordSocketClient Client { get; set; } = null;
        protected IAudioClient AudioClient { get; set; }
        private AudioOutStream OutStream { get; set; } = null;



        public BotBase()
        {
           
        }


        



        protected async Task StartStream()
        {
           
        }




        protected async Task JoinChannel(ulong channelId)
        {
            if (!(AudioClient is null) && AudioClient.ConnectionState == ConnectionState.Connected)
            {
                // always leave previous channel
                // this prevents stuck client on program crash
                await LeaveChannel();
            }


            AudioClient = await ((ISocketAudioChannel)Client.GetChannel(channelId)).ConnectAsync(true);
        }


        protected async Task LeaveChannel()
        {
            if (AudioClient.ConnectionState == ConnectionState.Disconnected)
            {
                return;
            }

            //TODO: abort stream, once implemented
            await AudioClient.StopAsync();
        }



        /// <summary>
        /// Login the bot, no action if still connected
        /// </summary>
        /// <param name="token"></param>
        /// <returns></returns>
        protected async Task Login(string token)
        {
            Client = new DiscordSocketClient();

            await Client.LoginAsync(TokenType.Bot, token);
            await Client.StartAsync();




            if (!(Client is null) && Client.ConnectionState == ConnectionState.Connected)
            {
                return;
            } 

            // block until client is fully connected
            while(Client.ConnectionState != ConnectionState.Connected)
            {
                await Task.Delay(1);
            }
            
        }



        /// <summary>
        /// Logoff client if connected, leave voice channel if connected
        /// Dispose client even if not connected
        /// 
        /// </summary>
        /// <returns></returns>
        protected async Task Logout()
        {
            if (Client is null || Client.ConnectionState == ConnectionState.Disconnected)
            {
                return;
            }
            else
            {
                await LeaveChannel();

                await Client.StopAsync();
                await Client.LogoutAsync();
            }


            if (!(Client is null))
            {
                Client.Dispose();
                Client = null;
            }
        }

    }
}
