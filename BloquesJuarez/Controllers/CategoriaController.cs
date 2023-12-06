using BloquesJuarez.Datos;
using BloquesJuarez.Models;
using Microsoft.AspNetCore.Mvc;

namespace BloquesJuarez.Controllers
{
    public class CategoriaController : Controller
    {
        private readonly AplicationDBContext _db;

        public CategoriaController(AplicationDBContext db)
        {
            _db = db;
        }

        // GET: Categoria
        public async Task<IActionResult> Index(string buscar, int? numpag, string filtroActual)
        {
            ViewData["FiltroActual"] = filtroActual;

            if (buscar != null)
                numpag = 1;
            else
                buscar = filtroActual;

            ViewData["Buscar"] = buscar;

            IQueryable<Categoria> query = _db.Categoria;

            if (!String.IsNullOrEmpty(buscar))
            {
                query = query.Where(c => c.NombreCategoria.ToLower().Contains(buscar.ToLower()));
            }

            query = query.OrderBy(m => m.NombreCategoria); // Ordenar alfabéticamente por nombre

            int cantidadregistros = 5;
            var paginacion = await Paginacion<Categoria>.CrearPaginacion(query, numpag ?? 1, cantidadregistros);

            return View(paginacion);
        }



        public IActionResult Crear()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Crear(Categoria categoria)
        {
            if (ModelState.IsValid)
            {
                // Comprobar si la categoria ya existe en la base de datos
                if (_db.Categoria.Any(c => c.NombreCategoria == categoria.NombreCategoria))
                {
                    TempData[WC.Error] = "La categoría ya existe!";

                    ModelState.AddModelError("Nombre", "La Categoría ya existe!");
                    return RedirectToAction(nameof(Index));
                }

                _db.Categoria.Add(categoria);
                _db.SaveChanges();
                TempData[WC.Exitosa] = "Categoría creada exitosamente!";
            }
            TempData[WC.Error] = "Hubo un error al crear la categoría";

            return RedirectToAction(nameof(Index));
        }

        // GET: Categoria/Editar
        public IActionResult Editar(int? Id)
        {
            if (Id == null || Id == 0)
            {
                return NotFound();
            }
            var obj = _db.Categoria.Find(Id);
            if (obj == null)
            {
                return NotFound();
            }
            return View(obj);

        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Editar(Categoria categoria)
        {
            if (ModelState.IsValid)
            {
                _db.Categoria.Update(categoria);
                _db.SaveChanges();
                TempData[WC.Exitosa] = "Categoría Actualizada exitosamente";
                return RedirectToAction(nameof(Index));
            }
            TempData[WC.Error] = "Hubo un error al actualizar la categoría";
            return View(categoria);
        }

        // GET: Categoria/Eliminar
        public IActionResult Eliminar(int? Id)
        {
            if (Id == null || Id == 0)
            {
                return NotFound();
            }
            var obj = _db.Categoria.Find(Id);
            if (obj == null)
            {
                return NotFound();
            }
            return View(obj);

        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Eliminar(Categoria categoria)
        {
            if (categoria == null)
            {
                TempData[WC.Error] = "Hubo un error al eliminar la categoría";

                return NotFound();
            }
            _db.Categoria.Remove(categoria);
            _db.SaveChanges();
            TempData[WC.Exitosa] = "Categoría Eliminada exitosamente";
            return RedirectToAction(nameof(Index));
        }
    }
}
