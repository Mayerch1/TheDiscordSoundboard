
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TheDiscordSoundboard.Models;

namespace TheDiscordSoundboard.Service
{
    public class TrackDataService: ITrackDataService
    {
        private readonly SqliteContext _context;

        public TrackDataService(SqliteContext context)
        {
            _context = context;
        }


        public async Task<ActionResult<IEnumerable<TrackData>>> AllTracks()
        {
            // AsQueryable needed where due to clash with Linq.Async
            return await _context.TrackDataItems.AsQueryable().ToListAsync();
        }


        public async Task<ActionResult<IEnumerable<TrackData>>> TrackByLocalFile(string localFile)
        {
            return await _context.TrackDataItems.AsQueryable().Where(t => t.LocalFile == localFile).ToListAsync();
        }


        public async Task<ActionResult<TrackData>> FindById(long id)
        {
            return await _context.TrackDataItems.FindAsync(id);
        }

        public async Task<HttpStatusCode> PutTrack(TrackData data)
        {
            _context.Entry(data).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!TrackDataExists(data.Id))
                {
                    return HttpStatusCode.NotFound;
                }
                else
                {
                    throw;
                }
            }

            return HttpStatusCode.NoContent;
        }


        public async Task PostTrack(TrackData data)
        {
            _context.TrackDataItems.Add(data);
            await _context.SaveChangesAsync();
        }



        public async Task<TrackData> DeleteTrack(long id)
        {
            var track = await _context.TrackDataItems.FindAsync(id);

            if(track == null)
            {
                return null;
            }

            _context.TrackDataItems.Remove(track);
            await _context.SaveChangesAsync();

            return track;
        }



        private bool TrackDataExists(long id)
        {
            return _context.TrackDataItems.Any(e => e.Id == id);
        }

    }



    public interface ITrackDataService
    {
        Task<ActionResult<IEnumerable<TrackData>>> AllTracks();
        Task<ActionResult<IEnumerable<TrackData>>> TrackByLocalFile(string localFile);

        Task<ActionResult<TrackData>> FindById(long id);

        Task<HttpStatusCode> PutTrack(TrackData data);

        Task PostTrack(TrackData data);

        Task<TrackData> DeleteTrack(long id);
    }
}
