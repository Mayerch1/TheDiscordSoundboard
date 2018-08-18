using Discord.WebSocket;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DicsordBot.Bot
{
    /// <summary>
    /// BotHandle inherites from Bot, failsave frame around the bot class
    /// </summary>
    /// <remarks>
    /// Does only communicate with api through its base 'Bot'
    /// Catches and treats excetpion from base class
    /// If not connected to channels, methods will return false (or void)
    /// </remarks>
    public class BotHandle : Bot
    {
        public BotHandle()
        {
        }

        #region event handlers

        /// <summary>
        /// FileWarningThrown delegate
        /// </summary>
        /// <param name="msg">Error description</param>
        /// <param name="solution">Solution for the error</param>
        public delegate void FileWarningThrown(string msg, string solution);

        /// <summary>
        /// FileWarningThrown field
        /// </summary>
        public FileWarningThrown FileWarning;

        /// <summary>
        /// TokenWarningThrown delegate
        /// </summary>
        /// <param name="msg">Error description</param>
        /// <param name="solution">Solution for the error</param>
        public delegate void TokenWarningThrown(string msg, string solution);

        /// <summary>
        /// TokenWarningThrown field
        /// </summary>
        public TokenWarningThrown TokenWarning;

        /// <summary>
        /// ChannelWarningThrown delegate
        /// </summary>
        /// <param name="msg">Error description</param>
        /// <param name="solution">Solution for the error</param>
        public delegate void ChannelWarningThrown(string msg, string solution);

        /// <summary>
        /// ChannelWarningThrown field
        /// </summary>
        public ChannelWarningThrown ChannelWarning;

        /// <summary>
        /// ClientWarningThrown delegate
        /// </summary>
        /// <param name="msg">Error description</param>
        /// <param name="solution">Solution for the error</param>
        public delegate void ClientWarningThrown(string msg, string solution);

        /// <summary>
        /// ClientWarningThrown field
        /// </summary>
        public ClientWarningThrown ClientWarning;

        /// <summary>
        /// SnackBarAction enum, passed into delegate for informing eventhandler on requested information
        /// </summary>
        public enum SnackBarAction { Settings, None };

        /// <summary>
        /// SnackBarWarningThrown delegate
        /// </summary>
        /// <param name="msg">Error description</param>
        /// <param name="action">Enum for triggered action in event Handler</param>
        public delegate void SnackBarWarningThrown(string msg, SnackBarAction action = SnackBarAction.None);

        /// <summary>
        /// SnackBarWarningThrown field
        /// </summary>
        public SnackBarWarningThrown SnackbarWarning;

        #endregion event handlers

        #region properties

        /// <summary>
        /// Token property
        /// </summary>
        public string Token { get; set; }

        /// <summary>
        /// ChannelId property
        /// </summary>
        public ulong ChannelId { get; set; }

        /// <summary>
        /// IsTempChannelId property
        /// </summary>
        public bool IsTempChannelId { get; set; } = false;

        /// <summary>
        /// CurrentCahnnelId property
        /// </summary>
        private ulong CurrentChannelId { get; set; }

        /// <summary>
        /// ClientId property
        /// </summary>
        public ulong ClientId { get; set; }

        #endregion properties

        #region controll stuff

        /// <summary>
        /// enques a Button into the list, only the file property is relevant
        /// </summary>
        /// <param name="btn">ButtonData object which should be streamed</param>
        /// <returns>Task</returns>
        /// <remarks>
        /// auto connects to Server, calls enqueAsync(ButtonData) of base
        /// </remarks>
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
                SnackbarWarning("Directory of Button " + btn.ID + " could not be found.");
            }
            catch (System.IO.FileNotFoundException)
            {
                SnackbarWarning("File of Button number " + btn.ID + " could not be found.");
            }
            catch (System.IO.InvalidDataException)
            {
                SnackbarWarning("File of Button number " + btn.ID + " is damaged.");
            }
            catch (System.Runtime.InteropServices.COMException)
            {
                SnackbarWarning("File type of Button number " + btn.ID + " is not supported.");
            }
            catch (Exception ex)
            {
                await disconnectFromChannelAsync();
                UnhandledException.initWindow(ex, "Trying to add a new file to the queue. (Button Nr: " + btn.ID + ", Name: \"" + btn.Name + "\").");
                Console.WriteLine("EnqueueAsync unhandled");
            }
        }

        /// <summary>
        /// resumes or starts the stream
        /// </summary>
        /// <returns>Task</returns>
        /// <remarks>
        /// auto connects to server and channel, calls resumeStream() of base
        /// </remarks>
        new public async Task resumeStream()
        {
            if (!await connectToServerAsync())
                return;

            //if channel id has changed, reconnect to new channel
            if (!IsChannelConnected || ChannelId != CurrentChannelId)
            {
                if (!await connectToChannelAsync())
                    return;
            }

            try
            {
                await base.resumeStream();
            }
            catch (Exception ex)
            {
                await disconnectFromChannelAsync();
                UnhandledException.initWindow(ex, "Trying to start/resume the stream");

                //TODO: catch all possible ex
            }
        }

        /// <summary>
        /// sets the GameState of the bot
        /// </summary>
        /// <param name="msg">Message to be displayed</param>
        /// <param name="streamUrl">Url to twitch-stream, only relevant when isStreamin is true</param>
        /// <param name="isStreaming">bool, if bot is streaming on twitch or not</param>
        /// <returns>shows success of setting the game state</returns>
        /// <remarks>calls setGameState() of base</remarks>
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
                UnhandledException.initWindow(ex, "Trying to set a GameStatus");
                Console.WriteLine("GameState Exception");
                return false;
            }

            return true;
        }

        #endregion controll stuff

        #region start stuff

        /// <summary>
        /// connects to the discord servers
        /// </summary>
        /// <returns>success of operation</returns>
        /// <remarks>
        /// if already connected, returns true, calls connectToServerAsync() of base
        /// </remarks>
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
                SnackbarWarning("Invalid Token", SnackBarAction.Settings);

                Console.WriteLine("connection Exception (Token)");

                return false;
            }
            catch (System.Net.Http.HttpRequestException)
            {
                ClientWarning("Can't reach the Discord-Servers due to timeout", "Check your internet connection, also check the availability of the discord server/api.");
                Console.WriteLine("connection Exception (Timeout, ...)");
                return false;
            }
            catch (Exception ex)
            {
                UnhandledException.initWindow(ex, "Trying to connect to the Discord Servers");
                Console.WriteLine("general connection Exception");
                return false;
            }

            return true;
        }

        /// <summary>
        /// connects to a channel
        /// </summary>
        /// <returns>success of operation</returns>
        /// <remarks>
        /// auto connects to server, gets current channel of Owner if ChannelId is 0, calls connectToChannelAsync() of base
        /// </remarks>
        public async Task<bool> connectToChannelAsync()
        {
            if (!await connectToServerAsync())
                return false;

            SocketGuildUser client = null;

            //only search for client if auto-connect is wished
            if (ChannelId == 0)
            {
                //this should not throw, bc Connection is ensured above
                var clientList = await getAllClients();

                client = getClient(clientList);

                if (client == null)
                {
                    SnackbarWarning("Cannot find specified owner.", SnackBarAction.None);
                    return false;
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
                    //reset channelId to 0, if property was set
                    if (IsTempChannelId)
                        ChannelId = 0;
                    await base.connectToChannelAsync(CurrentChannelId);
                }
            }
            catch (System.Threading.Tasks.TaskCanceledException ex)
            {
                UnhandledException.initWindow(ex, "Trying to connect to a voice channel (cancelled).");
                return false;
            }
            catch (System.TimeoutException)
            {
                ChannelWarning("The bot can't connect to the specified channel", "Check for sufficient permissions to join the requested channel.");
                return false;
            }
            catch (Exception ex)
            {
                Console.WriteLine("Unhandled connection Exception");
                UnhandledException.initWindow(ex, "Trying to connect to a voice channel");
                return false;
            }
            return true;
        }

        #endregion start stuff

        #region get data

        /// <summary>
        /// get a list of all channels of all servers
        /// </summary>
        /// <returns>list of all serves each with a list of all channels </returns>
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
                SnackbarWarning("Cannot request channel list");
                return null;
            }

            return channelList;
        }

        /// <summary>
        /// list of all clients
        /// </summary>
        /// <param name="acceptOffline">also show clients which are currently offline</param>
        /// <returns>list of all serves each with a list of all channels</returns>
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

        /// <summary>
        /// extract client (ClientId) out of the client list
        /// </summary>
        /// <param name="clientList">list of all servers, each with a list of all users</param>
        /// <returns>client object with the id ClientId</returns>
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