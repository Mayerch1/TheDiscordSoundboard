using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DicsordBot
{
    public class BotInstructor
    {
        public async Task connectServer()
        {
            try
            {
                await Handle.Bot.connectToServerAsync(Handle.Data.persistent.Token);
            }
            catch (BotException ex)
            {
                if (ex.ConnectionError == BotException.connectionError.Token)
                {
                    //IDEA: show token hint
                }
            }
            catch
            {
                //IDEA: show debug exception
                return;
            }
        }



        public async Task connectChannel(ulong id = 0)
        {
            if (!Handle.Bot.IsServerConnected)
                await connectServer();

            //connect to bot owner
            if (id == 0)
            {
                var clientList = Handle.Bot.getAllClients();
                //iterate through servers
                foreach(var server in clientList)
                {
                    //iterate through connected clints
                    foreach(var client in server)
                    {
                        if(client.Id == Handle.Data.persistent.ClientId)
                        {
                            try
                            {
                                await Handle.Bot.connectToChannelAsync(client.VoiceChannel.Id);
                            }
                            catch (BotException ex)
                            {

                            }
                            catch { }

                        }
                    }
                }
            }
            else
            {
                try
                {
                    await Handle.Bot.connectToChannelAsync(id);
                }
                catch (BotException ex) { }
                catch { }
            }
        }
    }
}