using Application.API.Consumer;
using Application.Models;
using Application.Models.Identity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Scaffolding.Metadata;
using System.Runtime.InteropServices;

namespace Application.MVC.Controllers
{
    [Authorize(Roles = "admins,users")]
    public class PlaylistsController : Controller
    {
        // GET: PlaylistsController
        public ActionResult Index()
        {
            if (User.IsInRole("admins"))
            {
                // Admin: todas las playlists
                var data = Crud<Playlist>.GetAll();
                return View(data);
            }
            else
            {
                // Usuario: solo sus playlists
                var currentUserId = GetCurrentUserId();
                var data = Crud<Playlist>.GetBy("user", currentUserId);
                return View(data);
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

        // GET: PlaylistsController/Details/5
        public ActionResult Details(int id)
        {
            var data = Crud<Playlist>.GetById(id);
            return View(data);
        }

        // GET: PlaylistsController/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: PlaylistsController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(Playlist data)
        {
            try
            {
                data.UserId = GetCurrentUserId();
                Crud<Playlist>.Create(data);
                return RedirectToAction(nameof(Index));
            }
            catch(Exception ex)
            {
                ModelState.AddModelError("", ex.Message);
                return View(data);
            }
        }

        // GET: PlaylistsController/Edit/5
        public ActionResult Edit(int id)
        {
            var data = Crud<Playlist>.GetById(id);
            return View(data);
        }

        // POST: PlaylistsController/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(int id, Playlist data)
        {
            try
            {
                Crud<Playlist>.Update(id, data);
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", ex.Message);
                return View(data);
            }
        }

        // GET: PlaylistsController/Delete/5
        public ActionResult Delete(int id)
        {
            var data = Crud<Playlist>.GetById(id);
            return View(data);
        }

        // POST: PlaylistsController/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int id, Playlist data)
        {
            try
            {
                Crud<Playlist>.Delete(id);
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", ex.Message);
                return View(data);
            }
        }
    }
}
