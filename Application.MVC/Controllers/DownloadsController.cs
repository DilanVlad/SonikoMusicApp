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
    public class DownloadsController : Controller
    {
        // GET: DownloadsController
        public ActionResult Index()
        {
            if (User.IsInRole("admins"))
            {
                // Admin: todas las descargas
                var data = Crud<Download>.GetAll();
                return View(data);
            }
            else
            {
                // Usuario: solo sus descargas
                var currentUserId = GetCurrentUserId();
                var data = Crud<Download>.GetBy("user", currentUserId);
                return View(data);
            }
        }

        // GET: DownloadsController/Details/5
        public ActionResult Details(int id)
        {
            var data = Crud<Download>.GetById(id);
            return View(data);
        }

        // GET: DownloadsController/Create
        public ActionResult Create()
        {

            return View();
        }

        // POST: DownloadsController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(Download data)
        {
            try
            {
                data.UserId = GetCurrentUserId(); // Asignar usuario actual
                data.DownloadDate = DateTime.Now;
                data.Status = Download.DownloadStatus.Completed;

                Crud<Download>.Create(data);
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", ex.Message);
                return View(data);
            }
        }

        // GET: DownloadsController/Edit/5
        public ActionResult Edit(int id)
        {
            var data = Crud<Download>.GetById(id);
            return View(data);
        }

        // POST: DownloadsController/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(int id, Download data)
        {
            try
            {
                Crud<Download>.Update(id, data);
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", ex.Message);
                return View(data);
            }
        }

        // GET: DownloadsController/Delete/5
        public ActionResult Delete(int id)
        {
            var data = Crud<Download>.GetById(id);
            return View(data);
        }

        // POST: DownloadsController/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int id, Download data)
        {
            try
            {
                Crud<Download>.Delete(id);
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", ex.Message);
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

    }
}
