using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Security.Policy;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TheDiscordSoundboard.Models;
using TheDiscordSoundboard.Models.Bot;
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
        public async Task<ActionResult<Models.Config>> GetConfig()
        {
            return await _service.GetConfig();
        }


        [HttpPut]
        public async Task<IActionResult> PutConfig(Config cfg)
        {
            // only one config object can exist
            // therefore any adding a new one is not possible

            await _service.PutConfig(cfg);
            return NoContent();
        }

    }
}
