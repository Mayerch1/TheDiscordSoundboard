using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TheDiscordSoundboard.Models;
using TheDiscordSoundboard.Models.Bot;

namespace TheDiscordSoundboard.Service
{
    public class BotService: IBotService
    {
        
        private readonly BotContext _context = new BotContext();


        public ActionResult<BotState> GetBot()
        {
            return _context.Bot;
        }

        public ActionResult<List<BotTrackData>> GetQueue()
        {
            return _context.Bot.Queue;
        }


        public void AppendToQueue(BotTrackData append)
        {
            _context.Bot.Queue.Add(append);
            // invoke playing if not started yet
            // skip to first in playlist when forcequeue
        }


        public void UpdateQueue(List<BotTrackData> queue)
        {
            _context.Bot.Queue = queue;
            // invoke playing if not started yet
        }

    }


    public interface IBotService
    {
        ActionResult<BotState> GetBot();
        ActionResult<List<BotTrackData>> GetQueue();

        void AppendToQueue(BotTrackData append);

        void UpdateQueue(List<BotTrackData> queue);
    }
}
