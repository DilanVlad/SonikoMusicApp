using Application.API.Consumer;
using Application.Models.Identity;
using Application.Models.Implementations;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace Application.MVC.Controllers
{
    [Authorize(Roles = "admins,users")]
    public class FollowController : Controller
    {
        // GET: FollowController
        public ActionResult Index()
        {
            if (User.IsInRole("admins"))
            {
                // Admin: todos los follows
                var data = Crud<Follow>.GetAll();
                return View(data);
            }
            else
            {
                // Usuario: solo sus follows (artistas que sigue)
                var currentUserId = GetCurrentUserId();
                var data = Crud<Follow>.GetBy("following", currentUserId);
                return View(data);
            }
        }

        // GET: FollowController/Details/5
        public ActionResult Details(int id)
        {
            var data = Crud<Follow>.GetById(id);
            return View(data);
        }

        // GET: FollowController/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: FollowController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(Follow data)
        {
            try
            {
                data.FollowerId = GetCurrentUserId(); 
                data.FollowDate = DateTime.Now;

                Crud<Follow>.Create(data);
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", ex.Message);
                return View(data);
            }
        }

        // GET: FollowController/Edit/5
        public ActionResult Edit(int id)
        {
            var data = Crud<Follow>.GetById(id);
            return View(data);
        }

        // POST: FollowController/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(int id, Follow data)
        {
            try
            {
                Crud<Follow>.Update(id, data);
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", ex.Message);
                return View(data);
            }
        }

        // GET: FollowController/Delete/5
        public ActionResult Delete(int id)
        {
            var data = Crud<Follow>.GetById(id);
            return View(data);
        }

        // POST: FollowController/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int id, Follow data)
        {
            try
            {
                Crud<Follow>.Delete(id);
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", ex.Message);
                return View(data);
            }
        }

        [HttpPost]
        public ActionResult FollowArtist(int artistId)
        {
            try
            {
                var follow = new Follow
                {
                    FollowerId = GetCurrentUserId(),
                    ArtistId = artistId,
                    FollowDate = DateTime.Now
                };

                Crud<Follow>.Create(follow);
                TempData["Success"] = "Artista seguido exitosamente";
                return RedirectToAction("Search", "Musics");
            }
            catch (Exception ex)
            {
                TempData["Error"] = ex.Message;
                return RedirectToAction("Search", "Musics");
            }
        }
        private int GetCurrentUserId()
        {
            if (User.Identity.IsAuthenticated)
            {
                var userManager = HttpContext.RequestServices.GetRequiredService<UserManager<User>>();
                var userEmail = User.Identity.Name;
                var user = userManager.FindByNameAsync(userEmail).Result;
                return user?.Id ?? 0;
            }
            return 0;
        }
    }
}
