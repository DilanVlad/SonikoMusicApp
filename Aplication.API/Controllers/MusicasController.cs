using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Aplication.Modelos;

namespace Aplication.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MusicasController : ControllerBase
    {
        private readonly AppDbContext _context;

        public MusicasController(AppDbContext context)
        {
            _context = context;
        }

        // GET: api/Musicas
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Musica>>> GetMusica()
        {
            return await _context.Musicas
                .Include(m => m.Artista)
                .ToListAsync();
        }
        // GET: api/Musics/Artista/artistaId
        [HttpGet("Artista/{artistaId}")]
        public async Task<ActionResult<IEnumerable<Musica>>> GetMusicasByArtista(string artistaId)
        {
            var musicas = await _context.Musicas
                .Where(m => m.ArtistaId == artistaId)
                .Include(m => m.Artista)
                .ToListAsync();

            return musicas;
        }

        // GET: api/Musicas/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Musica>> GetMusica(int id)
        {
            var musica = await _context.Musicas
                .Include(m => m.Artista)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (musica == null)
            {
                return NotFound();
            }

            return musica;
        }

        // PUT: api/Musicas/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutMusica(int id, Musica musica)
        {
            if (id != musica.Id)
            {
                return BadRequest();
            }

            _context.Entry(musica).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!MusicaExists(id))
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

        // POST: api/Musicas
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<Musica>> PostMusica(Musica musica)
        {
            _context.Musicas.Add(musica);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetMusica", new { id = musica.Id }, musica);
        }

        // DELETE: api/Musicas/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteMusica(int id)
        {
            var musica = await _context.Musicas.FindAsync(id);
            if (musica == null)
            {
                return NotFound();
            }

            _context.Musicas.Remove(musica);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool MusicaExists(int id)
        {
            return _context.Musicas.Any(e => e.Id == id);
        }


        // Metodo aparte
        // GET: api/Musics/Search/termino
        [HttpGet("Search/{termino}")]
        public async Task<ActionResult<IEnumerable<Musica>>> SearchMusicas(string termino)
        {
            var musicas = await _context.Musicas
                .Where(m => m.Titulo.Contains(termino) || m.Artista.Nombre.Contains(termino))
                .Include(m => m.Artista)
                .ToListAsync();

            return musicas;
        }
    }
}
