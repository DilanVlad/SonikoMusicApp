using Aplication.API.Consumer;
using Aplication.Modelos;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Runtime.InteropServices;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace Aplication.MVC.Controllers
{
    public class MusicasController : Controller
    {
        // GET: MusicasController
        public ActionResult Index()
        {
            var data = Crud<Musica>.GetAll();
            return View(data);
        }

        // GET: MusicasController/Details/5
        public ActionResult Details(int id)
        {
            var data = Crud<Musica>.GetById(id);
            return View(data);
        }

        // GET: MusicasController/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: MusicasController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(Musica data, IFormFile ArchivoMusica)
        {
            try
            {
                // Establecer valores automáticos
                data.FechaSubida = DateTime.Now;
                data.EsActiva = true;
                data.CalificacionPromedio = 0;
                data.TotalCalificaciones = 0;
                data.Reproducciones = 0;
                data.Descargas = 0;

                // TODO: Obtener el ArtistaId del usuario logueado
                // data.ArtistaId = User.Identity.Name; // Cuando tengas Identity funcionando
                data.ArtistaId = "temp-artista-id"; // Temporal para pruebas

                // Manejar subida de archivo
                if (ArchivoMusica != null && ArchivoMusica.Length > 0)
                {
                    var fileName = Path.GetFileName(ArchivoMusica.FileName);
                    var filePath = Path.Combine("wwwroot/musicas", fileName);

                    // Crear directorio si no existe
                    Directory.CreateDirectory(Path.GetDirectoryName(filePath));

                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        ArchivoMusica.CopyTo(stream);
                    }

                    data.RutaArchivo = $"/musicas/{fileName}";
                }
                else
                {
                    ModelState.AddModelError("", "Debe seleccionar un archivo de audio");
                    return View(data);
                }

                Crud<Musica>.Create(data);
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", ex.Message);
                return View(data);
            }
        }

        // GET: MusicasController/Edit/5
        public ActionResult Edit(int id)
        {
            var data = Crud<Musica>.GetById(id);
            return View(data);
        }

        // POST: MusicasController/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(int id, Musica data)
        {
            try
            {
                Crud<Musica>.Update(id, data);
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", ex.Message);
                return View(data);
            }
        }

        // GET: MusicasController/Delete/5
        public ActionResult Delete(int id)
        {
            var data = Crud<Musica>.GetById(id);
            return View(data);
        }

        // POST: MusicasController/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int id, Musica data)
        {
            try
            {
                Crud<Musica>.Delete(id);
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", ex.Message);
                return View(data);
            }
        }


        // meetodos 
        // Busqueda
        public ActionResult Search(string query)
        {
            if (string.IsNullOrEmpty(query))
            {
                return RedirectToAction(nameof(Index));
            }

            try
            {
                var filteredMusicas = Crud<Musica>.GetBy("Search", query);
                ViewBag.SearchQuery = query;
                return View("SearchResults", filteredMusicas);
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", ex.Message);
                return View("SearchResults", new List<Musica>());
            }
        }
    }
}
