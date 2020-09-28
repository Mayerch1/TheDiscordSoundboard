using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using TheDiscordSoundboard.Models;

namespace TheDiscordSoundboard.Service
{
    public class ButtonService: IButtonService
    {
        private readonly SqliteContext _context;

        public ButtonService(SqliteContext context)
        {
            _context = context;
        }

        public async Task<ActionResult<IEnumerable<Buttons>>> AllButtons()
        {
            // AsQueryable needed where due to clash with Linq.Async
            List<Buttons> btnList =  await _context.ButtonItems.AsQueryable().ToListAsync();

            for (int i = 0; i < btnList.Count; i++)
            {
                var fileObj = await _context.TrackDataItems.FindAsync(btnList[i].TrackId);

                if (fileObj != null)
                {
                    btnList[i].Track = fileObj;
                }
            }

            return btnList;
        }


        public async Task<Buttons> FindById(long id)
        {
            var button =  await _context.ButtonItems.FindAsync(id);

            if(button != null)
            {
                // resolve the track item for ease of access in front-end
                var fileObj = await _context.TrackDataItems.FindAsync(button.Track);

                if (fileObj != null)
                {
                    button.Track = fileObj;
                }
            }

            return button;
        }



        public async Task<HttpStatusCode> PutButton(Buttons data)
        {
            _context.Entry(data).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ButtonsExists(data.Id))
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



        public async Task PostTrack(Buttons data)
        {
            _context.ButtonItems.Add(data);
            await _context.SaveChangesAsync();

        }


        public async Task<Buttons> DeleteTrack(long id)
        {
            var button = await _context.ButtonItems.FindAsync(id);
            if(button == null)
            {
                return null;
            }

            _context.ButtonItems.Remove(button);
            await _context.SaveChangesAsync();

            return button;
        }





        private bool ButtonsExists(long id)
        {
           
            return _context.ButtonItems.Any(e => e.Id == id);

        }


    }

    public interface IButtonService
    {
        Task<ActionResult<IEnumerable<Buttons>>> AllButtons();


        Task<Buttons> FindById(long id);

        Task<HttpStatusCode> PutButton(Buttons data);

        Task PostTrack(Buttons data);

        Task<Buttons> DeleteTrack(long id);
    }
}
