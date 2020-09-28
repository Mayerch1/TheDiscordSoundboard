using Discord;
using Discord.Audio;
using Discord.WebSocket;
using System;
using System.Dynamic;
using System.Linq;
using System.Threading.Tasks;
using TheDiscordSoundboard.Models.Bot;
using TheDiscordSoundboard.Models.config;

namespace TheDiscordSoundboard.Bot
{
    public class Bot: BotBase
    {
        public delegate Task OnQueueUpdated();

        public static OnQueueUpdated QueueUpdated;


        public BotState State { get; set; } = new BotState();

        private BotConfigDto Cfg { get; set; } = new BotConfigDto();





        public Bot(): base() {
            // only one global listener, as only one global instance allowed
            QueueUpdated = null;
            QueueUpdated += CheckQueue;
        }


        public async Task<bool> UpdateStatics(BotConfigDto cfg)
        {
            Cfg = cfg;
            return await StartBot(true);
            //await JoinOwner();
            return true;

        }


        private async Task CheckQueue()
        {
            // nothing to do when queue is empty
            if (State.Queue.Count == 0)
            {
                return;
            }

            // when there is no forced update do not play anything
            // (only return if current player is not empty
            if (!State.Queue[0].Metadata.ForceReplay && State.Playing.Track != null)
            {
                return;
            }


            //peek if next entry in list is forcePlay
            if(State.Queue.Count >= 2 && State.Queue[1].Metadata.ForceReplay)
            {
                // remove the first entry from the list and start this function recursive
                // this will continue until the last forceReplay track is found
                // the current 'instance' is aborted aftert that
                State.Queue.RemoveAt(0);
                await CheckQueue();
                return;
            }



            // abort the currently played track
            if(State.Queue[0].Metadata.ForceReplay && State.Playing.Track != null)
            {
                //TODO: abort the current track
            }


            // if flow is here:
            // • current title is finished/aborted
            // • queue is not empty
            // • the item at queue[1] is not a forceSkip

            await StartBot(); // no restart if connected
            await JoinOwner();

            LoadStream();
            //await StartStream(State.Queue[0]);

        }




        private void LoadStream()
        {

        }




        private async Task JoinOwner()
        {
            await base.JoinChannel(283664420738039809);
            return;
            var guilds = Client.Guilds;
            foreach(var g in guilds)
            {
                //TODO: resove
                Console.WriteLine("x");
                continue;
            }

        }


        /// <summary>
        /// Login the bot, does not connect to voice yet
        /// </summary>
        /// <param name="restart">restarts the bot if already started, e.g. for token change</param>
        /// <returns></returns>
        public async Task<bool> StartBot(bool restart = false)
        {

            await base.Login(Cfg.Token);
            return true;

            if (restart)
            {
                if (!(Client is null) && Client.ConnectionState == Discord.ConnectionState.Connected)
                {
                    // disconnecting allows token changes without restart
                    await base.Logout();
                }
            }

            try
            {
                await base.Login(Cfg.Token);
            }
            catch (Exception)
            {
                return false;
            }



            return true;
        }


        /// <summary>
        /// Stops the bot and loggs it off
        /// No action if bot is not connected
        /// Disposes Client in any case
        /// </summary>
        /// <returns></returns>
        private async Task StopBot()
        {
            await base.Logout();
        }

    }

        
}
