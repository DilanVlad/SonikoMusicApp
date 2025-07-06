using TagLib;
using Aplication.API.Consumer;
using Application.Models;
using Application.Models.Identity;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Timeouts;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace Application.MVC.Controllers
{
    public class MusicsController : Controller
    {
        // GET: MusicsController
        public ActionResult Index()
        {
            var data = Crud<Music>.GetAll();
            return View(data);
        }

        // GET: MusicsController/Details/5
        public ActionResult Details(int id)
        {
            var data = Crud<Music>.GetById(id);
            return View(data);
        }

        // GET: MusicsController/Create
        public ActionResult Create()
        {
            ViewBag.Genres = GetGenresList();
            return View();
        }

        private List<SelectListItem> GetGenresList()
        {
            return Enum.GetValues(typeof(Music.MusicalGenre))
                .Cast<Music.MusicalGenre>()
                .Select(g => new SelectListItem
                {
                    Value = ((int)g).ToString(),
                    Text = g.ToString()
                }).ToList();
        }

        // POST: MusicsController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        [RequestSizeLimit(50 * 1024 * 1024)] // 50MB
        [RequestFormLimits(MultipartBodyLengthLimit = 50 * 1024 * 1024)]
        public ActionResult Create(Music data, IFormFile MusicFile)
        {
            try
            {
                // Validar archivo
                if (MusicFile == null || MusicFile.Length == 0)
                {
                    ModelState.AddModelError("", "Debe seleccionar un archivo de música");
                    ViewBag.Genres = GetGenresList();
                    return View(data);
                }

                // Asignar campos automáticos
                data.UploadDate = DateTime.Now;
                data.ArtistId = GetCurrentUserId();

                if (data.ArtistId == 0)
                {
                    ModelState.AddModelError("", "Debe estar logueado para subir música");
                    ViewBag.Genres = GetGenresList();
                    return View(data);
                }

                // Guardar archivo y obtener ruta
                data.FilePath = SaveMusicFile(MusicFile, data.ArtistId);

                // Obtener duración del archivo o usar valor por defecto
                data.Duration = GetAudioDuration(data.FilePath) ?? data.Duration ?? "";

                Crud<Music>.Create(data);
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", ex.Message);
                ViewBag.Genres = GetGenresList();
                return View(data);
            }
        }


        // GET: MusicsController/Edit/5
        public ActionResult Edit(int id)
        {
            var data = Crud<Music>.GetById(id);
            ViewBag.Genres = GetGenresList();
            return View(data);
        }

        // POST: MusicsController/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(int id, Music data)
        {
            try
            {
                Crud<Music>.Update(id, data);
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", ex.Message);
                ViewBag.Genres = GetGenresList();
                return View(data);
            }
        }

        // GET: MusicsController/Delete/5
        public ActionResult Delete(int id)
        {
            var data = Crud<Music>.GetById(id);
            return View(data);
        }

        // POST: MusicsController/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int id, Music data)
        {
            try
            {
                Crud<Music>.Delete(id);
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

        private string SaveMusicFile(IFormFile file, int artistId)
        {
            var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "Storage", "music", $"user_{artistId}");
            Directory.CreateDirectory(uploadsFolder);

            var fileName = Guid.NewGuid().ToString() + Path.GetExtension(file.FileName);
            var filePath = Path.Combine(uploadsFolder, fileName);

            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                file.CopyTo(stream);
            }

            return $"music/user_{artistId}/{fileName}";
        }

        
        private string GetAudioDuration(string filePath)
        {
            try
            {
                var fullPath = Path.Combine(Directory.GetCurrentDirectory(), "Storage", filePath);
                var file = TagLib.File.Create(fullPath);
                var duration = file.Properties.Duration;
                return $"{(int)duration.TotalMinutes}:{duration.Seconds:D2}";
            }
            catch
            {
                return "3:00";
            }
        }




    }
}
