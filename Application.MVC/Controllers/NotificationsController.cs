using Application.API.Consumer;
using Application.Models.Identity;
using Application.Models.Implementations;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Runtime.InteropServices;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace Application.MVC.Controllers
{
    [Authorize]
    public class NotificationsController : Controller
    {
        // GET: NotificationsController
        public ActionResult Index()
        {
            var currentUserId = GetCurrentUserId();
            var data = Crud<Notification>.GetBy("user", currentUserId);
            return View(data);
        }

        // GET: NotificationsController/Details/5
        public ActionResult Details(int id)
        {
            var data = Crud<Notification>.GetById(id);
            return View(data);
        }

        // GET: NotificationsController/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: NotificationsController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(Notification data)
        {
            try
            {
                data.UserId = GetCurrentUserId(); 
                data.CreatedDate = DateTime.Now;
                data.IsRead = false;

                Crud<Notification>.Create(data);
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", ex.Message);
                return View(data);
            }
        }

        // GET: NotificationsController/Edit/5
        public ActionResult Edit(int id)
        {
            var data = Crud<Notification>.GetById(id);
            return View(data);
        }

        // POST: NotificationsController/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(int id, Notification data)
        {
            try
            {
                Crud<Notification>.Update(id, data);
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", ex.Message);
                return View(data);
            }
        }

        // GET: NotificationsController/Delete/5
        public ActionResult Delete(int id)
        {
            var data = Crud<Notification>.GetById(id);
            return View(data);
        }

        // POST: NotificationsController/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int id, Notification data)
        {
            try
            {
                Crud<Notification>.Delete(id);
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", ex.Message);
                return View(data);
            }
        }

        [HttpPost]
        public ActionResult MarkAsRead(int id)
        {
            try
            {
                var notification = Crud<Notification>.GetById(id);
                if (notification != null && notification.UserId == GetCurrentUserId())
                {
                    notification.IsRead = true;
                    Crud<Notification>.Update(id, notification);
                    TempData["Success"] = "Notificación marcada como leída";
                }
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                TempData["Error"] = ex.Message;
                return RedirectToAction("Index");
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
