﻿
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Discord.WebSocket;
using DataManagement;
using Util.IO;

namespace BotModule
{
    /// <summary>
    /// BotHandle inherits from Bot, fail safe frame around the bot class
    /// </summary>
    /// <see cref="Bot"/>
    /// <remarks>
    /// Does only communicate with api through its base 'Bot'
    /// Catches and treats exception from base class
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
        /// load stream of file, Will interrupt current streams
        /// </summary>
        /// <param name="data">BotData object which should be streamed</param>
        /// <remarks>
        /// auto connects to Server, calls base
        /// </remarks>
        public new async Task loadFileAsync(BotData data)
        {
            if (!await connectToServerAsync())
                return;

            base.IncompatibleWave += PublishIncompatibleMic;

            try
            {
                await base.loadFileAsync(data);
            }
            catch (Exception ex)
            {
                await disconnectFromChannelAsync();
                handleReplayException(ex,
                    "Trying to add a new file to the queue. (Button Nr: " + data.id + ", Name: \"" + data.name + "\").",
                    data.id);
            }
            finally
            {
                base.IncompatibleWave -= PublishIncompatibleMic;
            }
        }


        private void PublishIncompatibleMic()
        {
            SnackbarManager.SnackbarMessage("Incompatible Mic Settings", SnackbarManager.SnackbarAction.Log);

            Util.IO.LogManager.LogException(null, "BotModule/BotHandler",
                "Incompatible Mic Settings. To prevent distorted audio, set your device to " + BotWave.sampleRate + " Hz and " + BotWave.channelCount + " channels.", false);
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
            finally
            {
                await disconnectFromChannelAsync();
            }
        }

        /// <summary>
        /// sets the GameState of the bot
        /// </summary>
        /// <param name="msg">Message to be displayed</param>
        /// <param name="streamUrl">Url to twitch-stream, only relevant when isStreaming is true</param>
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
            catch (Exception)
            {
                Util.IO.SnackbarManager.SnackbarMessage("Could not set game status");
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
            catch (Exception ex)
            {
                handleReplayException(ex, "Could not connect to Discord Servers");
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


                if (clientList == null)
                {
                    Util.IO.SnackbarManager.SnackbarMessage("Bot is still connecting. Please Retry");
                    return false;
                }

                client = getClient(clientList);

                if (client == null)
                {
                    Util.IO.SnackbarManager.SnackbarMessage("Cannot find specified owner. Please Retry");
                    Util.IO.LogManager.LogException(null, "BotModule/BotHandler", "User not in channel or invalid username/-id");
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
            catch (Exception ex)
            {
                handleReplayException(ex, "Failed to connect to voice channel");
                return false;
            }
            return true;
        }

        #endregion start stuff

        #region handle exception

        private void handleReplayException(Exception ex, string msg, int btnId = -1)
        {
            const string location = "BotModule/BotHandler";
            string btnStr;
            //resolve button number/name
            if (btnId >= 0)
                //show button Number NOT Id
                btnStr = "File of Button number " + (btnId + 1);
            else
                btnStr = "The file ";

            if (ex == null)
                return;

            switch (ex)
            {
                case System.IO.DirectoryNotFoundException iEx:
                    SnackbarManager.SnackbarMessage(btnStr + " could not be found.");
                    break;

                case System.IO.FileNotFoundException iEx:
                    SnackbarManager.SnackbarMessage(btnStr + " could not be found.");
                    break;

                case System.IO.InvalidDataException iEx:
                    SnackbarManager.SnackbarMessage(btnStr + " is damaged.");
                    break;

                case System.Runtime.InteropServices.COMException iEx:
                    SnackbarManager.SnackbarMessage(btnStr + " is not supported.");
                    break;

                case System.TimeoutException iEx:
                    SnackbarManager.SnackbarMessage("Cannot get channel(s). Check permission");
                    Util.IO.LogManager.LogException(iEx, location, "No permission to join/download channel");
                    break;

                case TaskCanceledException iEx:
                    SnackbarManager.SnackbarMessage("Task cancelled");
                    break;

                case System.DllNotFoundException iEx:
                    SnackbarManager.SnackbarMessage("Missing dll");
                    Util.IO.LogManager.LogException(iEx, location, "Missing dll", true);
                    break;

                case Discord.Net.HttpException iEx:
                    SnackbarManager.SnackbarMessage("Invalid Token", SnackbarManager.SnackbarAction.Settings);
                    Util.IO.LogManager.LogException(iEx, location , "Invalid Token");
                    break;

                case System.Net.Http.HttpRequestException iEx:
                    SnackbarManager.SnackbarMessage("Can't reach the Discord-Servers");
                    Util.IO.LogManager.LogException(iEx, location, "Cannot reach Discord servers");
                    break;

                case System.ArgumentException iEx:
                    string iExMsg = msg;               
                    if(iEx.Message == "Unsupported Wave Format"){
                       iExMsg = "Mic not supported";
                    }
                    SnackbarManager.SnackbarMessage(iExMsg, SnackbarManager.SnackbarAction.None);
                    Util.IO.LogManager.LogException(iEx, location, iExMsg);
                    break;
                case Exception iEx:
                    SnackbarManager.SnackbarMessage(msg, SnackbarManager.SnackbarAction.Log);
                    Util.IO.LogManager.LogException(iEx, location, msg, true);
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
                SnackbarManager.SnackbarMessage("Cannot request channel list");
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
            catch(Exception ex)
            {
                Util.IO.LogManager.LogException(ex, "BotModule/BotHandler", "Exception when trying to GET client-list from Discord Servers");
                SnackbarManager.SnackbarMessage("Cannot get clients. Retry later", SnackbarManager.SnackbarAction.Log);
                return null;
            }
            return userList;
        }


        /// <summary>
        /// resolve a given username, in discord discrimantor schemee
        /// </summary>
        /// <param name="name">username</param>
        /// <param name="discriminator">discriminator</param>
        /// <returns>0 on failure, user_id otherwise</returns>
        public override async Task<ulong> resolveUsername(string name, string discriminator)
        {
            if (!await connectToServerAsync())
                return 0;

            return await base.resolveUsername(name, discriminator);
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
                //iterate through connected clients
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