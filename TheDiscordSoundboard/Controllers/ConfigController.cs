using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Security.Policy;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TheDiscordSoundboard.Models.config;
using TheDiscordSoundboard.Service;

namespace TheDiscordSoundboard.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ConfigController : ControllerBase
    {
        // bot has full access to database
        // however only queue and credentials are used
        private readonly IConfigService _service;

        public ConfigController(IConfigService service)
        {
            _service = service;
        }


        [HttpGet]
        public async Task<ActionResult<Config>> GetConfig()
        {
            return await _service.GetConfig();
        }


        [Route("Bot")]
        [HttpGet]
        public async Task<ActionResult<BotConfigDto>> GetBotConfig()
        {
            return new BotConfigDto((await _service.GetConfig()).Value);
        }

        [Route("Bot")]
        [HttpPut]
        public async Task<IActionResult> PutBotConfig(BotConfigDto cfg)
        {
            if(await _service.PutBotConfig(cfg))
            {
                return NoContent();
            }
            else
            {
                return Problem("Invalid Token");
            }
        }


        
        [Route("Buttons")]
        [HttpGet]
        public async Task<ActionResult<ButtonConfigDto>> GetButtonConfig()
        {
            return new ButtonConfigDto((await _service.GetConfig()).Value);
        }
        [Route("Buttons")]
        [HttpPut]
        public async Task<IActionResult> PutButtonConfig(ButtonConfigDto cfg)
        {
            await _service.PutButtonConfig(cfg);
            return NoContent();
        }



    }
}
