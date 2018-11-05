using System.Collections.Generic;
using System.Threading.Tasks;
using Discord.WebSocket;

namespace DiscordBot.Misc
{
    /// <summary>
    /// Gatheres Data from the Bot class (get methods only), methods uses Bot api with calls into Bot class, does not trigger bot actions
    /// </summary>
    /// <remarks>
    /// Requires Handle class, because it accesses the Bot class inside the Handle
    /// </remarks>
    public class BotMisc
    {
        /// <summary>
        /// get the client object from a client list of all online clients
        /// </summary>
        /// <param name="clientList">list of all clients on all servers (<code>List&lt;Server&lt;Clients&gt;&gt;</code>)</param>
        /// <param name="id">id of requested client (owner)</param>
        /// <returns>client object of owner</returns>
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

        /// <summary>
        /// sets the url to the avatar from the client into the Data class
        /// </summary>
        /// <param name="client">client object of target</param>
        public void updateAvatar(SocketGuildUser client)
        {
            if (client != null)
                Handle.Data.Persistent.ClientAvatar = "https://cdn.discordapp.com/avatars/" + client.Id + "/" + client.AvatarId + ".png?size=64";
        }

        /// <summary>
        /// resolves username to client Id
        /// </summary>
        /// <param name="name">discord username in form of 'Name#1234'</param>
        /// <returns>id of the user packed in Task</returns>
        public async Task<ulong> resolveUserName(string name)
        {
            List<List<SocketGuildUser>> clientList = await Handle.Bot.getAllClients(true);

            if (clientList == null)
                return 0;

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

        /// <summary>
        /// converts ButtonData to BotData, needs to be as sub-routine bc of different namespaces
        /// </summary>
        /// <param name="btn">ButtonData objcet to convert</param>
        /// <returns>BotData object</returns>
        public Data.BotData getBotData(Data.ButtonData btn)
        {
            return new Data.BotData()
            {
                name = btn.Name,
                filePath = btn.File,
                isEarrape = btn.IsEarrape,
                isLoop = btn.IsLoop,
                id = btn.ID,
            };
        }
    }
}