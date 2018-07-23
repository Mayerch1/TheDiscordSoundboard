using Discord.WebSocket;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DicsordBot.Bot
{
    /*
     * Handle for the Bot
     *
     * Derives from bot
     * Warps the bot client into a failsafe environment
     * Handles almost everythis or shows the UnhandledException Dialog
     *
     * Catches and treats exception from base class
     *
     * If not connected to channel, methods will return
     */

    public class BotHandle : Bot
    {
        public BotHandle()
        {
        }

        #region properties

        public string Token { get; set; }
        public ulong ChannelId { get; set; }
        public ulong ClientId { get; set; }

        #endregion properties

        #region controll stuff

        new public async Task enqueueAsync(Data.ButtonData btn)
        {
            if (!IsServerConnected)
                await connectToServerAsync();

            try
            {
                base.enqueueAsync(btn);
            }
            catch (System.IO.DirectoryNotFoundException ex)
            {
                //FUTURE: show Dialog
                Console.WriteLine("File Exception no dir");
            }
            catch (System.IO.FileNotFoundException ex)
            {
                //FUTURE: show Dialog
                Console.WriteLine("File Exception not Found");
            }
            catch (System.IO.InvalidDataException ex)
            {
                //FUTURE: show Dialog
                Console.WriteLine("File Exception invalid");
            }
            catch (Exception ex)
            {
                UnhandledException.initWindow(ex);

                Console.WriteLine("EnqueueAsync others");
                //TODO: catch all possible ex
            }
        }

        new public async Task resumeStream()
        {
            if (!IsServerConnected)
                await connectToServerAsync();

            if (!IsChannelConnected)
                await connectToChannelAsync();

            try
            {
                await base.resumeStream();
            }
            catch (Exception ex)
            {
                UnhandledException.initWindow(ex);
                //TODO: catch all possible ex
            }
        }

        new public async Task<bool> setGameState(string msg, string streamUrl = "", bool isStreaming = false)
        {
            if (!IsServerConnected)
                await connectToServerAsync();

            try
            {
                await base.setGameState(msg, streamUrl, isStreaming);
            }
            catch (Exception ex)
            {
                UnhandledException.initWindow(ex, "Failed to set GameStatus");
                Console.WriteLine("GameState Exception");
                return false;
            }

            return true;
        }

        #endregion controll stuff

        #region start stuff

        public async Task connectToServerAsync()
        {
            try
            {
                await base.connectToServerAsync(Token);
            }
            catch (Discord.Net.HttpException ex)
            {
                //FUTURE: show dialog
                Console.WriteLine("connection Exception (Token)");
            }
            catch (System.Net.Http.HttpRequestException ex)
            {
                //FUTURE: show dialog
                Console.WriteLine("connection Exception (Timeout, ...)");
            }
            catch (Exception ex)
            {
                UnhandledException.initWindow(ex, "Couldn'd connect to the Discord Servers");
                Console.WriteLine("general connection Exception");
            }
        }

        public async Task connectToChannelAsync()
        {
            if (!IsServerConnected)
                await connectToServerAsync();

            //connect to bot owner
            if (ChannelId == 0)
            {
                //this cannot throw, bc Connection is ensured above
                var clientList = await getAllClients();

                var client = getClient(clientList);

                if (client == null)
                {
                    //FUTURE: no client found, show msg
                }
                //iterate through servers
                try
                {
                    await base.connectToChannelAsync(client.VoiceChannel.Id);
                }
                catch (System.Threading.Tasks.TaskCanceledException ex)
                {
                    //FUTURE: show dialog
                    Console.WriteLine("connetcion Exception");
                }
                catch (System.TimeoutException ex)
                {
                    Console.WriteLine("connetcion Exception");
                    //FUTURE: show dialog
                }
                catch (Exception ex)
                {
                    Console.WriteLine("connetcion Exception");
                    UnhandledException.initWindow(ex, "Error while connecting to owner channel");
                    //TODO: catch
                    //connection lost while connecting
                    //timeout
                    //no permission to join
                }
            }
            else
            {
                try
                {
                    await base.connectToChannelAsync(ChannelId);
                }
                catch (System.Threading.Tasks.TaskCanceledException ex)
                {
                    //FUTURE: show dialog
                    Console.WriteLine("connetcion Exception");
                }
                catch (System.TimeoutException ex)
                {
                    Console.WriteLine("connetcion Exception");
                    //FUTURE: show dialog
                }
                catch (Exception ex)
                {
                    Console.WriteLine("connetcion Exception");
                    UnhandledException.initWindow(ex, "Error while connecting to specific channel");
                    //TODO: catch
                    //connection lost while connecting
                }
            }
        }

        #endregion start stuff

        #region get data

        new public async Task<List<List<SocketVoiceChannel>>> getAllChannels()
        {
            if (!IsServerConnected)
                await connectToServerAsync();

            List<List<SocketVoiceChannel>> channelList = null;

            try
            {
                channelList = base.getAllChannels();
            }
            catch (Exception ex)
            {
                UnhandledException.initWindow(ex, "Could'n request channel List");
                return null;
            }

            return channelList;
        }

        new public async Task<List<List<SocketGuildUser>>> getAllClients()
        {
            if (!IsServerConnected)
                await connectToServerAsync();

            List<List<SocketGuildUser>> userList = null;

            try
            {
                userList = base.getAllClients();
            }
            catch (Exception ex)
            {
                UnhandledException.initWindow(ex, "Could'n request user List");
                return null;
            }

            return userList;
        }

        public SocketGuildUser getClient(List<List<SocketGuildUser>> clientList)
        {
            foreach (var server in clientList)
            {
                //iterate through connected clints
                foreach (var client in server)
                {
                    if (client.Id == ClientId)
                    {
                        return client;
                    }
                }
            }
            return null;
        }

        #endregion get data
    }
}