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
        private readonly SqliteContext _sqlContext;
        private readonly IConfigService _config;

        public BotService(SqliteContext context)
        {
            _sqlContext = context;
            _config = new ConfigService(context);
        }


        public async Task<bool> ConnectBot()
        {
            bool result = await _context.Bot.StartBot();
            Console.WriteLine(result);
            return result;
        }


        public ActionResult<BotState> GetBot()
        {
            return _context.Bot.State;
        }

        public ActionResult<List<BotTrackData>> GetQueue()
        {
            return _context.Bot.State.Queue;
        }


        public async Task AppendToQueue(BotTrackData append)
        {
            _context.Bot.State.Queue.Add(append);
            // invoke playing if not started yet
            // skip to first in playlist when forcequeue
            Bot.Bot.QueueUpdated().ConfigureAwait(false);
        }


        public async Task UpdateQueue(List<BotTrackData> queue)
        {
            var cfg = (await _config.GetConfig()).Value;


            _context.Bot.State.Queue = queue;
            // invoke playing if not started yet

            Bot.Bot.QueueUpdated().ConfigureAwait(false);
        }

    }


    public interface IBotService
    {
        Task<bool> ConnectBot();

        ActionResult<BotState> GetBot();
        ActionResult<List<BotTrackData>> GetQueue();

        Task AppendToQueue(BotTrackData append);

        Task UpdateQueue(List<BotTrackData> queue);
    }
}
