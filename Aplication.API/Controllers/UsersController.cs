using Aplication.Modelos;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Aplication.API.Data;

namespace Application.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly AppDbContext _context;

        public UsersController(AppDbContext context)
        {
            _context = context;
        }

        // GET: api/Users
        [HttpGet]
        public async Task<ActionResult<IEnumerable<User>>> GetUsers()
        {
            return await _context.Users.ToListAsync();
        }

        // GET: api/Users/5
        [HttpGet("{id}")]
        public async Task<ActionResult<User>> GetUser(string id)
        {
            var user = await _context.Users.FindAsync(id);

            if (user == null)
            {
                return NotFound();
            }

            return user;
        }

        // GET: api/Users/Artistas
        [HttpGet("Artistas")]
        public async Task<ActionResult<IEnumerable<User>>> GetArtistas()
        {
            var artistas = await _context.UserRoles
                .Join(_context.Roles,
                      ur => ur.RoleId,
                      r => r.Id,
                      (ur, r) => new { ur.UserId, r.Name })
                .Where(x => x.Name == "Artista")
                .Join(_context.Users,
                      x => x.UserId,
                      u => u.Id,
                      (x, u) => u)
                .ToListAsync();

            return artistas;
        }

        // GET: api/Users/Search/termino
        [HttpGet("Search/{termino}")]
        public async Task<ActionResult<IEnumerable<User>>> SearchArtistas(string termino)
        {
            var artistas = await _context.UserRoles
                .Join(_context.Roles,
                      ur => ur.RoleId,
                      r => r.Id,
                      (ur, r) => new { ur.UserId, r.Name })
                .Where(x => x.Name == "Artista")
                .Join(_context.Users,
                      x => x.UserId,
                      u => u.Id,
                      (x, u) => u)
                .Where(u => u.Nombre.Contains(termino) ||
                           u.Apellido.Contains(termino) ||
                           u.UserName.Contains(termino))
                .ToListAsync();

            return artistas;
        }

        // GET: api/Users/Plan/planId
        [HttpGet("Plan/{planId}")]
        public async Task<ActionResult<IEnumerable<User>>> GetUsersByPlan(int planId)
        {
            return await _context.Users
                .Where(u => u.PlanId == planId)
                .ToListAsync();
        }

        // PUT: api/Users/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutUser(string id, User user)
        {
            if (id != user.Id)
            {
                return BadRequest();
            }

            _context.Entry(user).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!UserExists(id))
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

        // PUT: api/Users/5/Plan/2
        [HttpPut("{id}/Plan/{planId}")]
        public async Task<IActionResult> CambiarPlan(string id, int planId)
        {
            var user = await _context.Users.FindAsync(id);
            if (user == null)
            {
                return NotFound();
            }

            var plan = await _context.Plans.FindAsync(planId);
            if (plan == null)
            {
                return BadRequest("Plan no existe");
            }

            user.PlanId = planId;
            _context.Entry(user).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!UserExists(id))
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

        // POST: api/Users
        [HttpPost]
        public async Task<ActionResult<User>> PostUser(User user)
        {
            user.FechaRegistri = DateTime.Now;
            user.PlanId = 1; // Plan Free por defecto

            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetUser", new { id = user.Id }, user);
        }

        // DELETE: api/Users/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteUser(string id)
        {
            var user = await _context.Users.FindAsync(id);
            if (user == null)
            {
                return NotFound();
            }

            _context.Users.Remove(user);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        // GET: api/Users/5/Musicas
        [HttpGet("{id}/Musicas")]
        public async Task<ActionResult<IEnumerable<Musica>>> GetMusicasByArtista(string id)
        {
            var musicas = await _context.Musicas
                .Where(m => m.ArtistaId == id)
                .Include(m => m.Artista)
                .ToListAsync();

            return musicas;
        }

        private bool UserExists(string id)
        {
            return _context.Users.Any(e => e.Id == id);
        }
    }
}