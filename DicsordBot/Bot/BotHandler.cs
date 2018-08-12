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
     * If not connected to channel, methods will return false (or void)
     */

    public class BotHandle : Bot
    {
        public BotHandle()
        {
        }

        #region event handlers

        public delegate void FileWarningThrown(string msg, string solution);

        public FileWarningThrown FileWarning;

        public delegate void TokenWarningThrown(string msg, string solution);

        public TokenWarningThrown TokenWarning;

        public delegate void ChannelWarningThrown(string msg, string solution);

        public ChannelWarningThrown ChannelWarning;

        public delegate void ClientWarningThrown(string msg, string solution);

        public ClientWarningThrown ClientWarning;

        public delegate void SnackBarWarningThrown(string msg);

        public SnackBarWarningThrown SnackbarWarning;

        #endregion event handlers

        #region properties

        public string Token { get; set; }
        public ulong ChannelId { get; set; }
        private ulong CurrentChannelId { get; set; }
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
            catch (System.IO.DirectoryNotFoundException)
            {
                SnackbarWarning("The Directory of Button " + btn.ID + " (" + btn.Name + ") could not be found.");
            }
            catch (System.IO.FileNotFoundException)
            {
                SnackbarWarning("The file of Button number " + btn.ID + " (" + btn.Name + ") could not be found.");
            }
            catch (System.IO.InvalidDataException)
            {
                SnackbarWarning("The file-type file of Button number " + btn.ID + " (\"" + btn.Name + "\") is not supported.");
            }
            catch (Exception ex)
            {
                await disconnectFromChannelAsync();
                UnhandledException.initWindow(ex, "Error while adding a new file to the queue. (Button Nr: " + btn.ID + ", Name: " + btn.Name + ").");
                Console.WriteLine("EnqueueAsync unhandled");
            }
        }

        new public async Task resumeStream()
        {
            if (!await connectToServerAsync())
                return;

            //if channel id has changed, reconnect to new channel
            if (!IsChannelConnected || ChannelId != CurrentChannelId)
                await connectToChannelAsync();

            try
            {
                await base.resumeStream();
            }
            catch (Exception ex)
            {
                await disconnectFromChannelAsync();
                UnhandledException.initWindow(ex, "Failed to resume stream");

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

            if (Token == null || Token == "")
                return false;

            try
            {
                await base.connectToServerAsync(Token);
            }
            catch (Discord.Net.HttpException)
            {
                //TokenWarning("Your Token seems to be invalid", "Go to the settings tab and check your token.");
                SnackbarWarning("Invalid Token");

                Console.WriteLine("connection Exception (Token)");
                //UnhandledException.initWindow(ex, "failed to connect to Server");
                return false;
            }
            catch (System.Net.Http.HttpRequestException ex)
            {
                UnhandledException.initWindow(ex, "Can't establish a connection to the Discort-Servers");
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
                    ClientWarning("The Bot couldn't locate his owner.", "Join a channel, specify another owner, or manually set a channel for the bot to join.");
                }
            }

            try
            {
                if (ChannelId == 0)
                {
                    //connect to bot owner
                    await base.connectToChannelAsync(client.VoiceChannel.Id);
                    CurrentChannelId = 0;
                }
                else
                {
                    CurrentChannelId = ChannelId;
                    await base.connectToChannelAsync(CurrentChannelId);
                }
            }
            catch (System.Threading.Tasks.TaskCanceledException ex)
            {
                UnhandledException.initWindow(ex, "The User Cancelled the joining event.");
                return false;
            }
            catch (System.TimeoutException)
            {
                ChannelWarning("The bot can't connect to the specified channel", "Check for sufficient permissions to join the requested channel.");
                return false;
            }
            catch (Exception ex)
            {
                Console.WriteLine("Unhandled connetcion Exception");
                UnhandledException.initWindow(ex, "Error while connecting to a voice channel");
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
            catch
            {
                ChannelWarning("The bot can't download the channel-list", "Retry later.");
                return null;
            }

            return channelList;
        }

        new public async Task<List<List<SocketGuildUser>>> getAllClients(bool acceptOffline = false)
        {
            if (!await connectToServerAsync())
                return null;

            List<List<SocketGuildUser>> userList = null;

            try
            {
                userList = base.getAllClients(acceptOffline);
            }
            catch
            {
                ChannelWarning("The bot can't download the client-list", "Retry later.");
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