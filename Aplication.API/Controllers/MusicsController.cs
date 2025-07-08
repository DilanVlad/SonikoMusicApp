using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Application.Models;

namespace Application.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MusicsController : ControllerBase
    {
        private readonly AppDbContext _context;

        public MusicsController(AppDbContext context)
        {
            _context = context;
        }

        // GET: api/Musics
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Music>>> GetMusic()
        {
            return await _context.Musics
                .Include(m=> m.Artist)
                .Include(m=> m.Album)
                .Include(m=> m.PlaylistMusics)
                .ToListAsync();
        }

        // GET: api/Musics/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Music>> GetMusic(int id)
        {
            var music = await _context.Musics.FindAsync(id);

            if (music == null)
            {
                return NotFound();
            }

            return music;
        }

        // GET: api/MusicsByArtist/5
        [HttpGet("artist/{artistId}")]
        public async Task<ActionResult<IEnumerable<Music>>> GetMusicsByArtist(int artistId)
        {
            var musics = await _context.Musics
                .Where(m => m.ArtistId == artistId)
                .Include(m => m.Artist)
                .ToListAsync();

            return Ok(musics);
        }

        // PUT: api/Musics/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutMusic(int id, Music music)
        {
            if (id != music.Id)
            {
                return BadRequest();
            }

            _context.Entry(music).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!MusicExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // POST: api/Musics
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<Music>> PostMusic(Music music)
        {
            _context.Musics.Add(music);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetMusic", new { id = music.Id }, music);
        }

        // DELETE: api/Musics/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteMusic(int id)
        {
            var music = await _context.Musics.FindAsync(id);
            if (music == null)
            {
                return NotFound();
            }

            _context.Musics.Remove(music);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool MusicExists(int id)
        {
            return _context.Musics.Any(e => e.Id == id);
        }


        
        [HttpGet("search")]
        public async Task<ActionResult<IEnumerable<Music>>> SearchMusics(string query)
        {
            if (string.IsNullOrEmpty(query))
            {
                return BadRequest("Query cannot be empty");
            }

            var musics = await _context.Musics
                .Include(m => m.Artist)
                .Where(m =>
                    m.Title.Contains(query) ||
                    (m.Artist.FirstName + " " + m.Artist.LastName).Contains(query) ||
                    m.Artist.FirstName.Contains(query) ||
                    m.Artist.LastName.Contains(query)
                )
                .ToListAsync();

            return Ok(musics);
        }
    }
}
