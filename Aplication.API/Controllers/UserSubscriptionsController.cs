using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Application.Models.Suscription;

namespace Application.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserSubscriptionsController : ControllerBase
    {
        private readonly AppDbContext _context;

        public UserSubscriptionsController(AppDbContext context)
        {
            _context = context;
        }

        // GET: api/UserSubscriptions
        [HttpGet]
        public async Task<ActionResult<IEnumerable<UserSubscription>>> GetUserSubscription()
        {
            return await _context.UserSubscriptions
                .Include(us => us.User)
                .Include(us => us.SubscriptionPlan)
                .ToListAsync();
        }

        // GET: api/UserSubscriptions/user/5 
        [HttpGet("user/{userId}")]
        public async Task<ActionResult<IEnumerable<UserSubscription>>> GetSubscriptionsByUser(int userId)
        {
            return await _context.UserSubscriptions
                .Where(us => us.UserId == userId)
                .Include(us => us.SubscriptionPlan)
                .OrderByDescending(us => us.StartDate)
                .ToListAsync();
        }

        // GET: api/UserSubscriptions/user/5/active 
        [HttpGet("user/{userId}/active")]
        public async Task<ActionResult<UserSubscription>> GetActiveSubscriptionByUser(int userId)
        {
            var subscription = await _context.UserSubscriptions
                .Where(us => us.UserId == userId && us.IsActive && us.EndDate > DateTime.Now)
                .Include(us => us.SubscriptionPlan)
                .FirstOrDefaultAsync();

            if (subscription == null)
                return NotFound();

            return subscription;
        }

        // GET: api/UserSubscriptions/5
        [HttpGet("{id}")]
        public async Task<ActionResult<UserSubscription>> GetUserSubscription(int id)
        {
            var userSubscription = await _context.UserSubscriptions.FindAsync(id);

            if (userSubscription == null)
            {
                return NotFound();
            }

            return userSubscription;
        }

        // PUT: api/UserSubscriptions/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutUserSubscription(int id, UserSubscription userSubscription)
        {
            if (id != userSubscription.Id)
            {
                return BadRequest();
            }

            _context.Entry(userSubscription).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!UserSubscriptionExists(id))
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

        // POST: api/UserSubscriptions
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<UserSubscription>> PostUserSubscription(UserSubscription userSubscription)
        {
            _context.UserSubscriptions.Add(userSubscription);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetUserSubscription", new { id = userSubscription.Id }, userSubscription);
        }

        // DELETE: api/UserSubscriptions/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteUserSubscription(int id)
        {
            var userSubscription = await _context.UserSubscriptions.FindAsync(id);
            if (userSubscription == null)
            {
                return NotFound();
            }

            _context.UserSubscriptions.Remove(userSubscription);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool UserSubscriptionExists(int id)
        {
            return _context.UserSubscriptions.Any(e => e.Id == id);
        }

        // GET: api/UserSubscriptions/user/5/active
        [HttpGet("user/{userId}/active")]
        public async Task<ActionResult<UserSubscription>> GetActiveSubscription(int userId)
        {
            var subscription = await _context.UserSubscriptions
                .Where(us => us.UserId == userId && us.IsActive && us.EndDate > DateTime.Now)
                .Include(us => us.SubscriptionPlan)
                .OrderByDescending(us => us.EndDate)
                .FirstOrDefaultAsync();

            if (subscription == null)
                return NotFound("No active subscription found");

            return subscription;
        }

    }
}
