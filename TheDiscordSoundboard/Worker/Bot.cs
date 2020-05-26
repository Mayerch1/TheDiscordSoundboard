/*using Discord.Net.Queue;
using Discord.WebSocket;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TheDiscordSoundboard.Worker
{
    public class Bot
    {
        private DiscordSocketClient Client { get; set; }

        public async Task ConnectToServer(string token)
        {
            if(Client != null && Client.ConnectionState == Discord.ConnectionState.Connected)
            {
                // TODO: disconnect client
                // for token changes or kicking out crashed instances
            }

            Client = new DiscordSocketClient();

            await Client.LoginAsync(Discord.TokenType.Bot, token);
            await Client.StartAsync();
        }
    }
}
*/
