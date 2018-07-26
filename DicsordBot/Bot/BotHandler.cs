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
            if (!await connectToServerAsync())
                return;

            try
            {
                base.enqueueAsync(btn);
            }
            catch (System.IO.DirectoryNotFoundException ex)
            {
                Console.WriteLine("File Exception no dir");
            }
            catch (System.IO.FileNotFoundException ex)
            {
                Console.WriteLine("File Exception not Found");
            }
            catch (System.IO.InvalidDataException ex)
            {
                Console.WriteLine("File Exception invalid format");
            }
            catch (Exception ex)
            {
                UnhandledException.initWindow(ex);
                await disconnectFromChannelAsync();

                Console.WriteLine("EnqueueAsync unhandled");
            }
        }

        new public async Task resumeStream()
        {
            if (!await connectToServerAsync())
                return;

            if (!IsChannelConnected)
                await connectToChannelAsync();

            try
            {
                await base.resumeStream();
            }
            catch (Exception ex)
            {
                UnhandledException.initWindow(ex);
                await disconnectFromChannelAsync();
                //TODO: catch all possible ex
            }
        }

        new public async Task<bool> setGameState(string msg, string streamUrl = "", bool isStreaming = false)
        {
            if (!await connectToServerAsync())
                return false;

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

        public async Task<bool> connectToServerAsync()
        {
            if (IsServerConnected)
                return true;

            try
            {
                await base.connectToServerAsync(Token);
            }
            catch (Discord.Net.HttpException ex)
            {
                Console.WriteLine("connection Exception (Token)");
                UnhandledException.initWindow(ex, "failed to connect to Server");
                return false;
            }
            catch (System.Net.Http.HttpRequestException ex)
            {
                Console.WriteLine("connection Exception (Timeout, ...)");
                return false;
            }
            catch (Exception ex)
            {
                UnhandledException.initWindow(ex, "Couldn't connect to the Discord Servers");
                Console.WriteLine("general connection Exception");
                return false;
            }

            return true;
        }

        public async Task<bool> connectToChannelAsync()
        {
            if (!await connectToServerAsync())
                return false;

            SocketGuildUser client = null;

            //only search for client if auto-connect is wished
            if (ChannelId == 0)
            {
                //this cannot throw, bc Connection is ensured above
                var clientList = await getAllClients();

                client = getClient(clientList);

                if (client == null)
                {
                    //FUTURE: no client found, show msg
                }
            }

            try
            {
                if (ChannelId == 0)
                    //connect to bot owner
                    await base.connectToChannelAsync(client.VoiceChannel.Id);
                else
                    await base.connectToChannelAsync(ChannelId);
            }
            catch (System.Threading.Tasks.TaskCanceledException ex)
            {
                Console.WriteLine("connetcion Exception (aborted by user)");
                return false;
            }
            catch (System.TimeoutException ex)
            {
                Console.WriteLine("connetcion Exception (timeout,...)");
                //also thrown for missing rights to join

                return false;
            }
            catch (Exception ex)
            {
                Console.WriteLine("Unhandled connetcion Exception");
                UnhandledException.initWindow(ex, "Error while connecting to a voice channel");
                //TODO: catch
                //connection lost while connecting

                return false;
            }
            return true;
        }

        #endregion start stuff

        #region get data

        new public async Task<List<List<SocketVoiceChannel>>> getAllChannels()
        {
            if (!await connectToServerAsync())
                return null;

            List<List<SocketVoiceChannel>> channelList = null;

            try
            {
                channelList = base.getAllChannels();
            }
            catch (Exception ex)
            {
                UnhandledException.initWindow(ex, "Couldn't get channel List");
                return null;
            }

            return channelList;
        }

        new public async Task<List<List<SocketGuildUser>>> getAllClients(bool allowOffline = false)
        {
            if (!await connectToServerAsync())
                return null;

            List<List<SocketGuildUser>> userList = null;

            try
            {
                userList = base.getAllClients(allowOffline);
            }
            catch (Exception ex)
            {
                UnhandledException.initWindow(ex, "Couldn't get user List");
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