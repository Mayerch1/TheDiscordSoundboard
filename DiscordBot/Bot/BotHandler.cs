using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Discord.WebSocket;
using DiscordBot.Data;

namespace DiscordBot.Bot
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
        /// <summary>
        /// constructor of class
        /// </summary>
        public BotHandle()
        {
        }

        #region event handlers

#pragma warning disable CS1591

        /// <summary>
        ///  SnackBarAction enum, passed into delegate for informing eventhandler on requested information
        /// </summary>
        public enum SnackbarAction { Settings, Update, None };

#pragma warning restore CS1591

        /// <summary>
        /// SnackBarWarningThrown delegate
        /// </summary>
        /// <param name="msg">Error description</param>
        /// <param name="action">Enum for triggered action in event Handler</param>
        public delegate void SnackbarWarningThrown(string msg, SnackbarAction action = SnackbarAction.None);

        /// <summary>
        /// SnackBarWarningThrown field
        /// </summary>
        public SnackbarWarningThrown SnackbarWarning;

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
        /// enques a Button into the list, only the file property is relevant. Loop and Boost are optional
        /// </summary>
        /// <param name="data">BotData object which should be streamed</param>
        /// <returns>Task</returns>
        /// <remarks>
        /// auto connects to Server, calls enqueAsync(BotData) of base
        /// </remarks>
        public new async Task enqueueAsync(BotData data)
        {
            await enqueueRegardingPriorityAsync(data, false);
        }

        /// <summary>
        /// enques a Button infront of the lsit, only the file property is relevant. Loop and Boost are optional
        /// </summary>
        /// <param name="data">BotData object which should be streamed</param>
        /// <returns>Task</returns>
        /// <remarks>
        /// auto connects to Server, calls enqueAsync(BotData) of base
        /// </remarks>
        public new async Task enqueuePriorityAsync(BotData data)
        {
            await enqueueRegardingPriorityAsync(data, true);
        }

        private async Task enqueueRegardingPriorityAsync(BotData data, bool isPriority)
        {
            if (!await connectToServerAsync())
                return;

            try
            {
                if (isPriority)
                    base.enqueuePriorityAsync(data);
                else
                    base.enqueueAsync(data);
            }
            catch (Exception ex)
            {
                await disconnectFromChannelAsync();
                handleReplayException(ex, "Trying to add a new file to the queue. (Button Nr: " + data.id + ", Name: \"" + data.name + "\").", data.id);
            }
        }

        /// <summary>
        /// resumes or starts the stream
        /// </summary>
        /// <returns>Task</returns>
        /// <remarks>
        /// auto connects to server and channel, calls resumeStream() of base
        /// </remarks>
        public new async Task resumeStream()
        {
            if (!await connectToServerAsync())
                return;
            else if (IsStreaming)
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
                handleReplayException(ex, "Trying to start / resume the stream");
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
        public new async Task<bool> setGameState(string msg, string streamUrl = "", bool isStreaming = false)
        {
            if (!await connectToServerAsync())
                return false;

            try
            {
                await base.setGameState(msg, streamUrl, isStreaming);
            }
            catch (Exception ex)
            {
                UI.UnhandledException.initWindow(ex, "Trying to set a GameStatus");
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

            if (String.IsNullOrEmpty(Token))
                return false;

            try
            {
                await base.connectToServerAsync(Token);
            }
            catch (Discord.Net.HttpException)
            {
                SnackbarWarning("Invalid Token", SnackbarAction.Settings);

                return false;
            }
            catch (System.Net.Http.HttpRequestException)
            {
                SnackbarWarning("Can't reach the Discord-Servers", SnackbarAction.None);
                return false;
            }
            catch (Exception ex)
            {
                UI.UnhandledException.initWindow(ex, "Trying to connect to the Discord Servers");
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
                    SnackbarWarning("Cannot find specified owner.", SnackbarAction.None);
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
                UI.UnhandledException.initWindow(ex, "Trying to connect to a voice channel (cancelled).");
                return false;
            }
            catch (System.TimeoutException)
            {
                SnackbarWarning("Cannot join channel. Check permission", SnackbarAction.None);
                return false;
            }
            catch (Exception ex)
            {
                Console.WriteLine("Unhandled connection Exception");
                UI.UnhandledException.initWindow(ex, "Trying to connect to a voice channel");
                return false;
            }
            return true;
        }

        #endregion start stuff

        #region handle exception

        private void handleReplayException(Exception ex, string msg, int btnId = -1)
        {
            string btnStr;
            //resolve button number/name
            if (btnId >= 0)
                //show button Number NOT Id
                btnStr = "File of Button number " + (btnId + 1);
            else
                btnStr = "The file ";

            switch (ex)
            {
                case System.IO.DirectoryNotFoundException iEx:
                    SnackbarWarning(btnStr + " could not be found.");
                    break;

                case System.IO.FileNotFoundException iEx:
                    SnackbarWarning(btnStr + " could not be found.");
                    break;

                case System.IO.InvalidDataException iEx:
                    SnackbarWarning(btnStr + " is damaged.");
                    break;

                case System.Runtime.InteropServices.COMException iEx:
                    SnackbarWarning(btnStr + " is not supported.");
                    break;

                default:
                    Console.WriteLine("Failed to handle error (BotHandler.cs)");
                    break;
            }
        }

        #endregion handle exception

        #region get data

        /// <summary>
        /// get a list of all channels of all servers
        /// </summary>
        /// <returns>list of all serves each with a list of all channels </returns>
        public new async Task<List<List<SocketVoiceChannel>>> getAllChannels()
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
        public new async Task<List<List<SocketGuildUser>>> getAllClients(bool acceptOffline = false)
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
                SnackbarWarning("Cannot get clients. Retry later");
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