using Discord.WebSocket;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DicsordBot
{
    /*
     * Handle for the Bot
     *
     * Derives from bot
     * Overrides all unsafe functions and add handler + information from the Handler.Data
     *
     * Catches and treats exception from base class
     */

    public class BotHandle : Bot
    {
        public BotHandle()
        {
        }

        #region controll stuff

        new public async Task enqueueAsync(ButtonData btn)
        {
            try
            {
                await base.enqueueAsync(btn);
            }
            catch (System.IO.DirectoryNotFoundException ex)
            {
                //TODO: treat
            }
            catch (System.IO.FileNotFoundException ex)
            {
                //TODO: treat
            }
            catch (System.IO.InvalidDataException ex)
            {
                //TODO: treat
            }
            catch (Exception ex)
            {
                Console.WriteLine("Break");
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
                //TODO: catch all possible ex
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
                await base.connectToServerAsync(Handle.Data.Persistent.Token);
            }
            catch (Exception ex)
            {
                Console.WriteLine("break");
                //TODO: catch
                //invalid token
                //no connection
                //everything else
            }
        }

        new public async Task connectToChannelAsync(ulong id = 0)
        {
            if (!Handle.Bot.IsServerConnected)
                await connectToServerAsync();

            //connect to bot owner
            if (id == 0)
            {
                //this cannot throw, bc Connection is ensured above
                var clientList = await getAllClients();
                //iterate through servers
                foreach (var server in clientList)
                {
                    //iterate through connected clints
                    foreach (var client in server)
                    {
                        if (client.Id == Handle.Data.Persistent.ClientId)
                        {
                            try
                            {
                                await base.connectToChannelAsync(client.VoiceChannel.Id);
                            }
                            catch (Exception ex)
                            {
                                Console.WriteLine("Error, connecting to owner async");
                                //TODO: catch
                                //connection lost while connecting
                                //timeout
                                //no permission to join
                            }
                        }
                    }
                }
            }
            else
            {
                try
                {
                    await base.connectToChannelAsync(id);
                }
                catch (Exception ex)
                {
                    //TODO: catch
                    //connection lost while connecting
                    //no permission to join
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
            catch
            {
                //TODO: show dialog
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
            catch
            {
                //TODO: show dialog
                return null;
            }

            return userList;
        }

        #endregion get data
    }
}