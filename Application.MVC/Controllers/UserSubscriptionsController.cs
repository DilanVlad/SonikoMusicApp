using Application.API.Consumer;
using Application.Models.Identity;
using Application.Models.Suscription;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace Application.MVC.Controllers
{
    [Authorize(Roles = "admins,users")]
    public class UserSubscriptionsController : Controller
    {
        // GET: UserSubscriptionsController
        public ActionResult Index()
        {
            if (User.IsInRole("admins"))
            {
                // Admin: todas las suscripciones
                var data = Crud<UserSubscription>.GetAll();
                return View(data);
            }
            else
            {
                // Usuario: solo sus suscripciones
                var currentUserId = GetCurrentUserId();
                var data = Crud<UserSubscription>.GetBy("user", currentUserId);
                return View(data);
            }
        }

        // GET: UserSubscriptionsController/Details/5
        public ActionResult Details(int id)
        {
            var data = Crud<UserSubscription>.GetById(id);
            return View(data);
        }

        // GET: UserSubscriptionsController/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: UserSubscriptionsController/Create
        [HttpPost]
        [ValidateAntiForgeryToken] 
        public ActionResult Create(UserSubscription data)
        {
            try
            {
                data.UserId = GetCurrentUserId(); // Asignar usuario actual
                data.StartDate = DateTime.Now;
                data.IsActive = true;
                data.Status = UserSubscription.SubscriptionStatus.Active;

                Crud<UserSubscription>.Create(data);
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", ex.Message);
                return View(data);
            }
        }

        // GET: UserSubscriptionsController/Edit/5
        public ActionResult Edit(int id)
        {
            var data = Crud<UserSubscription>.GetById(id);
            return View(data);
        }

        // POST: UserSubscriptionsController/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(int id, UserSubscription data)
        {
            try
            {
                Crud<UserSubscription>.Update(id, data);
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", ex.Message);
                return View(data);
            }
        }

        // GET: UserSubscriptionsController/Delete/5
        public ActionResult Delete(int id)
        {
            var data = Crud<UserSubscription>.GetById(id);
            return View(data);
        }

        // POST: UserSubscriptionsController/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int id, UserSubscription data)
        {
            try
            {
                Crud<UserSubscription>.Delete(id);
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
