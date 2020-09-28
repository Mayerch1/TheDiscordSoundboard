using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using TheDiscordSoundboard.Models;
using TheDiscordSoundboard.Models.config;

namespace TheDiscordSoundboard.Service
{
    public class ConfigService: IConfigService
    {
        private readonly SqliteContext _context;
        private readonly BotContext _botContext = new BotContext();

        public ConfigService(SqliteContext context)
        {
            _context = context;
        }


        public async Task<ActionResult<Models.config.Config>> GetConfig()
        {
            if (!ConfigExists(1))
            {
                // create config
                await CreateConfig();
            }

            return await _context.ConfigItem.FindAsync((long)1);

        }

     

        public async Task PutConfig(Models.config.Config cfg)
        {
            _context.Entry(cfg).State = Microsoft.EntityFrameworkCore.EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException)
            {
                if (!ConfigExists(cfg.Id))
                {
                    // insert into db, as config not existing
                    await CreateConfig(cfg);
                }
                else
                {
                    throw;
                }
            }
        }


        public async Task<bool> PutBotConfig(BotConfigDto cfg)
        {
            var FullCfg = (await GetConfig()).Value;

            FullCfg.Update(cfg);
            await PutConfig(FullCfg);

            // make the bot aware of the config changes
            return await _botContext.Bot.UpdateStatics(cfg);
        }

        public async Task PutButtonConfig(ButtonConfigDto cfg)
        {
            var FullCfg = (await GetConfig()).Value;

            FullCfg.Update(cfg);
            await PutConfig(FullCfg);
        }



        private async Task CreateConfig(Models.config.Config cfg = null)
        {
            if(cfg == null)
            {
                cfg = new Models.config.Config();
            }
            _context.ConfigItem.Add(cfg);
            await _context.SaveChangesAsync();
        }


        private bool ConfigExists(long id)
        {
            return _context.ConfigItem.Any(e => e.Id == id);
        }
    }


    public interface IConfigService
    {
        Task<ActionResult<Models.config.Config>> GetConfig();

        Task PutConfig(Models.config.Config cfg);
        Task<bool> PutBotConfig(BotConfigDto cfg);
        Task PutButtonConfig(ButtonConfigDto cfg);
    }
}
