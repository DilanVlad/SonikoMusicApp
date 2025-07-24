using Application.API.Consumer;
using Application.Models;
using Application.Models.Identity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Build.Framework;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace Application.MVC.Controllers
{
    [Authorize(Roles = "admins,artists,users")]
    public class AlbumsController : Controller
    {
        // GET: AlbumsController
        public ActionResult Index()
        {
            if (User.IsInRole("artists"))
            {
                // Artista: solo sus albums
                var currentUserId = GetCurrentUserId();
                var data = Crud<Album>.GetBy("artist", currentUserId);
                return View(data);
            }
            else
            {
                // Admin: todos los albums
                var data = Crud<Album>.GetAll();
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

        // GET: AlbumsController/Details/5
        public ActionResult Details(int id)
        {
            try
            {
                var album = Crud<Album>.GetById(id);
                if (album == null)
                {
                    TempData["Error"] = "Álbum no encontrado";
                    return RedirectToAction("Index");
                }

                // si el álbum está vacío y el usuario no es el artista
                var currentUserId = GetCurrentUserId();
                bool isOwner = User.IsInRole("admins") || album.ArtistId == currentUserId;
                bool albumHasMusics = album.Musics?.Any() == true;

                if (!albumHasMusics && !isOwner)
                {
                    TempData["Warning"] = "Este álbum aún no tiene canciones disponibles";
                    return RedirectToAction("Index");
                }

                //  fecha de lanzamiento
                if (album.ReleaseDate > DateTime.Now && !isOwner)
                {
                    TempData["Warning"] = $"Este álbum se lanzará el {album.ReleaseDate:dd/MM/yyyy}";
                    return RedirectToAction("Index");
                }

                ViewBag.IsOwner = isOwner;
                ViewBag.AlbumHasMusics = albumHasMusics;
                ViewBag.IsReleased = album.ReleaseDate <= DateTime.Now;

                return View(album);
            }
            catch (Exception ex)
            {
                TempData["Error"] = ex.Message;
                return RedirectToAction("Index");
            }
        }

        // GET: AlbumsController/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: AlbumsController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(Album data)
        {
            try
            {
                data.ArtistId = GetCurrentUserId();
                Crud<Album>.Create(data);
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", ex.Message);
                return View(data);
            }
        }

        // GET: AlbumsController/Edit/5
        public ActionResult Edit(int id)
        {
            var data = Crud<Album>.GetById(id);
            return View(data);
        }

        // POST: AlbumsController/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(int id, Album data)
        {
            try
            {
                Crud<Album>.Update(id, data);
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", ex.Message);
                return View(data);
            }
        }

        // GET: AlbumsController/Delete/5
        public ActionResult Delete(int id)
        {
            var data = Crud<Album>.GetById(id);
            return View(data);
        }

        // POST: AlbumsController/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int id, Album data)
        {
            try
            {
                Crud<Album>.Delete(id);
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", ex.Message);
                return View(data);
            }
        }

        // GET: Albums/ManageMusics/5
        [Authorize(Roles = "admins,artists")] // gestionar musicas del álbum
        public ActionResult ManageMusics(int id)
        {
            try
            {
                var album = Crud<Album>.GetById(id);
                if (album == null)
                {
                    TempData["Error"] = "Álbum no encontrado";
                    return RedirectToAction("Index");
                }

                // Verificar permisos
                var currentUserId = GetCurrentUserId();
                if (!User.IsInRole("admins") && album.ArtistId != currentUserId)
                {
                    return Forbid();
                }

                // Obtener todas las músicas del artista
                var artistMusics = Crud<Music>.GetBy("artist", album.ArtistId);

                // Separar músicas asignadas y disponibles
                var assignedMusics = artistMusics.Where(m => m.AlbumId == id).ToList();
                var availableMusics = artistMusics.Where(m => m.AlbumId == null || m.AlbumId != id).ToList();

                ViewBag.Album = album;
                ViewBag.AssignedMusics = assignedMusics;
                ViewBag.AvailableMusics = availableMusics;

                return View();
            }
            catch (Exception ex)
            {
                TempData["Error"] = ex.Message;
                return RedirectToAction("Index");
            }
        }


        // POST: Albums/AddMusicToAlbum 
        [HttpPost]
        [Authorize(Roles = "admins,artists")]
        public ActionResult AddMusicToAlbum(int albumId, int musicId) // añadir
        {
            try
            {
                var currentUserId = GetCurrentUserId();
                var album = Crud<Album>.GetById(albumId);
                var music = Crud<Music>.GetById(musicId);

                if (album == null || music == null)
                {
                    TempData["Error"] = "Álbum o música no encontrados";
                    return RedirectToAction("ManageMusics", new { id = albumId });
                }

                // Verificar permisos
                if (!User.IsInRole("admins") && album.ArtistId != currentUserId)
                {
                    return Forbid();
                }

                // Verificar que la música pertenece al mismo artista
                if (music.ArtistId != album.ArtistId)
                {
                    TempData["Error"] = "Solo puedes asignar música del mismo artista";
                    return RedirectToAction("ManageMusics", new { id = albumId });
                }

                // Asignar album a la música
                music.AlbumId = albumId;
                Crud<Music>.Update(musicId, music);

                TempData["Success"] = $"'{music.Title}' agregada al álbum exitosamente";
                return RedirectToAction("ManageMusics", new { id = albumId });
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Error al asignar música: " + ex.Message;
                return RedirectToAction("ManageMusics", new { id = albumId });
            }
        }

        // POST: Albums/RemoveMusicFromAlbum 
        [HttpPost]
        [Authorize(Roles = "admins,artists")]
        public ActionResult RemoveMusicFromAlbum(int albumId, int musicId) // qtar musica del album
        {
            try
            {
                var currentUserId = GetCurrentUserId();
                var album = Crud<Album>.GetById(albumId);
                var music = Crud<Music>.GetById(musicId);

                if (album == null || music == null)
                {
                    TempData["Error"] = "Álbum o música no encontrados";
                    return RedirectToAction("ManageMusics", new { id = albumId });
                }

                // Verificar permisos
                if (!User.IsInRole("admins") && album.ArtistId != currentUserId)
                {
                    return Forbid();
                }

                // Quitar album de la música
                music.AlbumId = null;
                Crud<Music>.Update(musicId, music);

                TempData["Success"] = $"'{music.Title}' quitada del álbum exitosamente";
                return RedirectToAction("ManageMusics", new { id = albumId });
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Error al quitar música: " + ex.Message;
                return RedirectToAction("ManageMusics", new { id = albumId });
            }
        }



    }
}
