using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TheDiscordSoundboard.Models;
using TheDiscordSoundboard.Service;

namespace TheDiscordSoundboard.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ButtonsController : ControllerBase
    {
        private readonly IButtonService _service;


        public ButtonsController(IButtonService service)
        {
            _service = service;
        }

        // GET: api/Buttons
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Buttons>>> GetButtonItems()
        {
            // does not resolve foreign keys, faster for large lists
            return await _service.AllButtons();
        }

        // GET: api/Buttons/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Buttons>> GetButtons(long id)
        {
            var buttons = await _service.FindById(id);
            
            if (buttons == null)
            {
                return NotFound();
            }
            return buttons;
        }

        // PUT: api/Buttons/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for
        // more details, see https://go.microsoft.com/fwlink/?linkid=2123754.
        [HttpPut("{id}")]
        public async Task<IActionResult> PutButtons(long id, Buttons buttons)
        {
            // LocalTrack can be set with either id as foreign key of with passing LocalTrack object
            // the LocalTrack field is always overriding the LocalTrackId, (LocalTrackId may be invalid in this case)
            if (id != buttons.Id)
            {
                return BadRequest();
            }

            var status = await _service.PutButton(buttons);
            if(status == System.Net.HttpStatusCode.NotFound)
            {
                return NotFound();
            }

            return NoContent();
        }

        // POST: api/Buttons
        // To protect from overposting attacks, enable the specific properties you want to bind to, for
        // more details, see https://go.microsoft.com/fwlink/?linkid=2123754.
        [HttpPost]
        public async Task<ActionResult<Buttons>> PostButtons(Buttons buttons)
        {
            await _service.PostTrack(buttons);
            //return CreatedAtAction("GetButtons", new { id = buttons.Id }, buttons);
            return CreatedAtAction(nameof(GetButtons), new { id = buttons.Id }, buttons);
        }

        // DELETE: api/Buttons/5
        [HttpDelete("{id}")]
        public async Task<ActionResult<Buttons>> DeleteButtons(long id)
        {
            var buttons = await _service.DeleteTrack(id);

            if (buttons == null)
            {
                return NotFound();
            }

            return buttons;
        }
    }
}
