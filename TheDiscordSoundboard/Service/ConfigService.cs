using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using TheDiscordSoundboard.Models;

namespace TheDiscordSoundboard.Service
{
    public class ConfigService: IConfigService
    {
        private readonly SqliteContext _context;
        public ConfigService(SqliteContext context)
        {
            _context = context;
        }


        public async Task<ActionResult<Models.Config>> GetConfig()
        {
            if (!ConfigExists(1))
            {
                // create config
                await CreateConfig();
            }

            return await _context.ConfigItem.FindAsync((long)1);

        }

        public async Task PutConfig(Config cfg)
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



        private async Task CreateConfig(Config cfg = null)
        {
            if(cfg == null)
            {
                cfg = new Config();
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
        Task<ActionResult<Models.Config>> GetConfig();

        Task PutConfig(Config cfg);
    }
}
