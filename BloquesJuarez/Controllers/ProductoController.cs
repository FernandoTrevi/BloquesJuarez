﻿using BloquesJuarez.Datos;
using BloquesJuarez.Models;
using BloquesJuarez.Models.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace BloquesJuarez.Controllers
{
    public class ProductoController : Controller
    {
        private readonly AplicationDBContext _db;

        public ProductoController(AplicationDBContext db)
        {
            _db = db;
        }
        // GET: ProductoController
        public async Task<IActionResult> Index(string buscar, string ordenActual, int? numpag, string filtroActual)
        {
            IQueryable<Producto> query = _db.Producto.Include(p => p.Categoria);

            if (buscar != null)
                numpag = 1;
            else
                buscar = filtroActual;

            ViewData["OrdenActual"] = ordenActual;
            ViewData["FiltroActual"] = buscar;

            if (!String.IsNullOrEmpty(buscar))
            {
                query = query.Where(p => p.NombreProducto.Contains(buscar));
            }

            ViewData["FiltroNombre"] = String.IsNullOrEmpty(ordenActual) ? "NombreDescendente" : "";

            switch (ordenActual)
            {
                case "NombreDescendente":
                    query = query.OrderByDescending(producto => producto.NombreProducto);
                    break;
                default:
                    query = query.OrderBy(producto => producto.NombreProducto);
                    break;
            }
            
            // Obtener las categorías y almacenarlas en el ViewBag
            var categorias = await _db.Categoria.ToListAsync();
            ViewBag.Categorias = categorias;

            int cantidadregistros = 5;
            var paginacion = await Paginacion<Producto>.CrearPaginacion(query, numpag ?? 1, cantidadregistros);
            return View(paginacion);
        }
        // GET: ProductoController/Crear
        public IActionResult Crear()
        {
            ProductoVM productoVM = new()
            {
                Producto = new Producto(),
                CategoriaLista = _db.Categoria.Select(c => new SelectListItem
                {
                    Text = c.NombreCategoria,
                    Value = c.Id.ToString()
                })
            };
            return View(productoVM);
        }

        [HttpPost]
        public IActionResult ActualizarPrecios(int categoria, int porcentaje, string direccion)
        {
            try
            {
                IQueryable<Producto> productosToUpdate;

                if (categoria == 0) // Supongamos que 0 representa la categoría "todos"
                {
                    productosToUpdate = _db.Producto;
                }
                else
                {
                    productosToUpdate = _db.Producto.Where(p => p.CategoriaId == categoria);
                }

                foreach (var producto in productosToUpdate)
                {
                    decimal factor = (direccion.ToLower() == "subir") ? (1 + porcentaje / 100M) : (1 - porcentaje / 100M);
                    producto.Precio = producto.Precio * (double)factor;
                }

                _db.SaveChanges();

                TempData[WC.Exitosa] = "Precios actualizados correctamente.";
            }
            catch (Exception ex)
            {
                TempData[WC.Error] = "Error al actualizar los precios.";
                Console.WriteLine($"Error inesperado: {ex.Message}");
            }

            return RedirectToAction(nameof(Index));
        }

        // POST: ProductoController/Crear
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Crear(ProductoVM productoVM)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    if (_db.Producto.Any(c => c.NombreProducto == productoVM.Producto.NombreProducto))
                    {
                        TempData[WC.Error] = "El producto ya existe!";

                        ModelState.AddModelError("Nombre", "El producto ya existe!");
                        return RedirectToAction(nameof(Index));
                    }
                    // Agrega el producto al contexto de la base de datos
                    _db.Producto.Add(productoVM.Producto);
                    _db.SaveChanges();
                    TempData[WC.Exitosa] = "El producto se creó correctamente!";
                    // Redirige a la acción "Index" después de crear el producto
                    return RedirectToAction(nameof(Index));
                }
                catch (Exception ex)
                {
                    TempData[WC.Error] = "Error al crear el producto";
                    Console.WriteLine($"Error inesperado: {ex.Message}");
                    // Puedes manejar errores inesperados aquí
                    // Redirige a la vista de creación en caso de excepción
                    return View(productoVM);
                }
            }
            else
            {
                // Si el modelo no es válido, imprime los errores de validación en la consola
                foreach (var modelError in ModelState.Values.SelectMany(v => v.Errors))
                {
                    Console.WriteLine($"Error de validación: {modelError.ErrorMessage}");
                }

                // Vuelve a mostrar la vista de creación con los errores de validación
                productoVM.CategoriaLista = _db.Categoria.Select(c => new SelectListItem
                {
                    Text = c.NombreCategoria,
                    Value = c.Id.ToString()
                });
                TempData[WC.Error] = "Error al crear el producto";

                return View(productoVM);
            }
        }


        // GET: ProductoController/Editar
        public async Task<ActionResult> Editar(int id)
        {
            var producto = await _db.Producto.FindAsync(id);
            if (producto == null)
            {
                return NotFound();
            }

            // Crear un objeto ProductoVM y asignar el producto recuperado
            ProductoVM productoVM = new ProductoVM()
            {
                Producto = producto,
                CategoriaLista = _db.Categoria.Select(c => new SelectListItem
                {
                    Text = c.NombreCategoria,
                    Value = c.Id.ToString()
                })
            };

            return View(productoVM);
        }

        // POST: ProductoController/Editar
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Editar(int id, ProductoVM productoVM)
        {
            if (id != productoVM.Producto.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    // Actualizar los valores del producto original con los valores del ViewModel
                    var productoOriginal = await _db.Producto.FindAsync(id);
                    if (productoOriginal == null)
                    {
                        return NotFound();
                    }

                    productoOriginal.NombreProducto = productoVM.Producto.NombreProducto;
                    productoOriginal.CategoriaId = productoVM.Producto.CategoriaId;
                    productoOriginal.UniDeMedida = productoVM.Producto.UniDeMedida;
                    productoOriginal.Precio = productoVM.Producto.Precio;

                    _db.Entry(productoOriginal).State = EntityState.Modified;
                    TempData[WC.Exitosa] = "El producto se modificó correctamente!";

                    await _db.SaveChangesAsync();
                    return RedirectToAction(nameof(Index));
                }
                catch (DbUpdateConcurrencyException)
                {
                    TempData[WC.Error] = "Hubo un error al modificar el producto.";
                    throw;
                }
            }

            // Si el modelo no es válido, retornar a la vista con el ViewModel
            productoVM.CategoriaLista = _db.Categoria.Select(c => new SelectListItem
            {
                Text = c.NombreCategoria,
                Value = c.Id.ToString()
            });
            TempData[WC.Error] = "Hubo un error. Modelo inválido.";

            return View(productoVM);
        }

        // GET: ProductoController/Eliminar
        public async Task<ActionResult> Eliminar(int id)
        {
            var producto = await _db.Producto.FindAsync(id);
            if (producto == null)
            {
                TempData[WC.Error] = "No se encontró el producto a eliminar.";

                return NotFound();
            }
            return View(producto);
        }

        // POST: ProductoController/ConfirmarEliminar
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ConfirmarEliminar(int id)
        {
            var producto = await _db.Producto.FindAsync(id);
            if (producto == null)
            {
                TempData[WC.Error] = "No se encontró el producto a eliminar.";
                return NotFound();
            }
            _db.Producto.Remove(producto);
            await _db.SaveChangesAsync();
            TempData[WC.Exitosa] = "Producto eliminado con éxito.";

            return RedirectToAction(nameof(Index));
        }

    }
}
