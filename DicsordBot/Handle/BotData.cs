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

    //TODO: data encapsulation

    public class BotData
    {
        public SocketGuildUser extractClient(List<List<SocketGuildUser>> clientList, ulong id)
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
            return null;
        }

        public async Task updateAvatar()
        {
            if (!Handle.Bot.IsServerConnected)
                await Handle.Bot.connectToServerAsync();

            var clientList = await Handle.Bot.getAllClients();

            if (clientList != null)
            {
                var client = extractClient(clientList, Handle.Data.Persistent.ClientId);

                if (client != null)
                    Handle.Data.Persistent.ClientAvatar = "https://cdn.discordapp.com/avatars/" + client.Id + "/" + client.AvatarId + ".png?size=128";
            }
        }
    }
}