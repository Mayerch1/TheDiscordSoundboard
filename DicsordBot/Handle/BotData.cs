using Discord.WebSocket;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DicsordBot
{
    /*
     * Proccesses Data that are not directly pulled from the bot
     *
     * But those data are useless if you're not using a bot
     *
     * It is so far not independent from the handler
     *
     */

    public class BotData
    {
        public SocketGuildUser extractClient(List<List<SocketGuildUser>> clientList, ulong id)
        {
            if (clientList != null)
            {
                foreach (var server in clientList)
                {
                    //iterate through connected clints
                    foreach (var client in server)
                    {
                        if (client.Id == id)
                        {
                            return client;
                        }
                    }
                }
            }
            return null;
        }

        //needs access to handle, bc shared property for this single use is not worth the effort
        public void updateAvatar(SocketGuildUser client)
        {
            if (client != null)
                Handle.Data.Persistent.ClientAvatar = "https://cdn.discordapp.com/avatars/" + client.Id + "/" + client.AvatarId + ".png?size=256";
        }

        //resolve username-> id, return 0 if no user was found
        public async Task<ulong> resolveUserName(string name)
        {
            List<List<SocketGuildUser>> clientList = await Handle.Bot.getAllClients(true);

            foreach (var server in clientList)
            {
                foreach (var client in server)
                {
                    if (client.Username + '#' + client.Discriminator == name)
                    {
                        return client.Id;
                    }
                }
            }

            return 0;
        }
    }
}