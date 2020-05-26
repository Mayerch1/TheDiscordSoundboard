using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TheDiscordSoundboard.Models;
using TheDiscordSoundboard.Service;

namespace TheDiscordSoundboard.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TrackDataController : ControllerBase
    {
        private readonly ITrackDataService _trackService;
        private readonly SqliteContext _context;

        public TrackDataController(ITrackDataService trackService)
        {
            _trackService = trackService;
        }

        // GET: api/LocalTracks
        [HttpGet]
        public async Task<ActionResult<IEnumerable<TrackData>>> GetLocalTrackItems([FromQuery(Name = "LocalFile")] string fileName)
        {
            if(fileName == null)
            {
                return await _trackService.AllTracks();
            }
            else
            {
                return await _trackService.TrackByLocalFile(fileName);
            }
        }

        // GET: api/LocalTracks/5
        [HttpGet("{id}")]
        public async Task<ActionResult<TrackData>> GetLocalTracks(long id)
        {
            var localTracks = await _trackService.FindById(id);

            if (localTracks == null)
            {
                return NotFound();
            }

            return localTracks;
        }

        // PUT: api/LocalTracks/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for
        // more details, see https://go.microsoft.com/fwlink/?linkid=2123754.
        [HttpPut("{id}")]
        public async Task<IActionResult> PutLocalTracks(long id, TrackData localTracks)
        {
            if (id != localTracks.Id)
            {
                return BadRequest();
            }


            var status = await _trackService.PutTrack(localTracks);

            if (status == System.Net.HttpStatusCode.NotFound)
            {
                return NotFound();
            }
            else
            {
                return NoContent();
            }

        }

        // POST: api/LocalTracks
        // To protect from overposting attacks, enable the specific properties you want to bind to, for
        // more details, see https://go.microsoft.com/fwlink/?linkid=2123754.
        [HttpPost]
        public async Task<ActionResult<TrackData>> PostLocalTracks(TrackData localTracks)
        {
            await _trackService.PostTrack(localTracks);
            return CreatedAtAction(nameof(GetLocalTracks), new { id = localTracks.Id }, localTracks);
        }

        // DELETE: api/LocalTracks/5
        [HttpDelete("{id}")]
        public async Task<ActionResult<TrackData>> DeleteLocalTracks(long id)
        {
            var tracks = await _trackService.DeleteTrack(id);

            if(tracks == null)
            {
                return NotFound();
            }

            return tracks;
        }

      
    }
}
