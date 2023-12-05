using BloquesJuarez.Datos;
using BloquesJuarez.Models;
using BloquesJuarez.Models.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace BloquesJuarez.Controllers
{
    public class RemitoController : Controller
    {
        private readonly AplicationDBContext _db;

        public RemitoController(AplicationDBContext db)
        {
            _db = db;
        }

        public async Task<IActionResult> Index(string buscar, string ordenActual, int? numpag, string filtroActual)
        {
            IQueryable<Remito> query = _db.Remito.Include(r => r.Cliente);

            if (buscar != null)
            {
                numpag = 1;
            }
            else
            {
                buscar = filtroActual;
            }

            ViewData["OrdenActual"] = ordenActual;
            ViewData["FiltroActual"] = buscar;

            if (!string.IsNullOrEmpty(buscar))
            {
                query = query.Where(r => r.NroRemito.ToString().Contains(buscar) || r.Cliente.NombreCliente.Contains(buscar));
            }

            if (ordenActual == "nroremito")
            {
                query = query.OrderBy(r => r.NroRemito);
            }
            else
            {
                query = query.OrderBy(r => r.Id); // Orden predeterminado
            }

            int cantidadregistros = 5; 
            var paginacion = await Paginacion<Remito>.CrearPaginacion(query, numpag ?? 1, cantidadregistros);

            return View(paginacion);
        }
        public IActionResult Crear()
        {
            // Llama a la función para obtener el último número de orden
            var ultimoNumeroOrden = ObtenerUltimoNumeroOrden();
            var remitoVM = new RemitoVM
            {
                ClienteLista = _db.Cliente
                .Select(p => new SelectListItem
                {
                    Value = p.Id.ToString(),
                    Text = p.NombreCliente,

                }),
                ProductoLista =_db.Producto
                .Select(p => new SelectListItem
                {
                    Value = p.NombreProducto,
                    Text = p.NombreProducto
                }),

                FechaActual = DateTime.Today,

                Remito = new Remito { NroRemito = ultimoNumeroOrden, LugarEntrega = "En la dirección del cliente" },
                // Inicializa la lista de detalles como una lista vacía
                RemitoDetalle = new List<RemitoDetalle>()
            };

           

            return View(remitoVM);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Crear(RemitoVM remitoVM)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    // 1. Crear una nueva orden de compra
                    var remito = new Remito
                    {
                        NroRemito = remitoVM.Remito.NroRemito,
                        Fecha = remitoVM.FechaActual,
                        ClienteId = remitoVM.Remito.ClienteId,
                        Observaciones = remitoVM.Remito.Observaciones,
                        LugarEntrega = remitoVM.Remito.LugarEntrega,
                        Estado = remitoVM.Remito.Estado

                    };

                    // 2. Agregar la orden de compra al contexto
                    _db.Add(remito);
                    await _db.SaveChangesAsync();

                    var IdDeRemito = remito.Id;

                    // 3. Agregar detalles de la orden de compra
                    foreach (RemitoDetalle detalle in remitoVM.RemitoDetalle)
                    {
                        string codigoSeleccionado = detalle.Nombre;
                        var productoIdSeleccionado = ObtenerProductoIdPorNombre(codigoSeleccionado);

                        if (productoIdSeleccionado.HasValue)
                        {
                            var remitoDetalle = new RemitoDetalle
                            {
                                RemitoId = IdDeRemito,
                                ProductoId = productoIdSeleccionado.Value,
                                Nombre = detalle.Nombre,
                                Cantidad = detalle.Cantidad
                            };

                            _db.Add(remitoDetalle);
                        }
                        else
                        {
                            // Manejar el caso en el que no se encontró el ProductoId
                            // Puedes mostrar un mensaje de error o tomar una acción adecuada
                        }
                    }

                    

                    // 5. Guardar los cambios en la base de datos
                    await _db.SaveChangesAsync();

                    return RedirectToAction("Ver", new { id = IdDeRemito });
                }
                catch (Exception ex)
                {
                    // Manejar excepciones aquí, realizar rollback si es necesario
                    ModelState.AddModelError(string.Empty, "Ocurrió un error al procesar la orden de compra.");
                }
            }

            // Si llegamos aquí, significa que hubo un error, volvemos a mostrar el formulario con los errores.
            remitoVM.ClienteLista = _db.Cliente
                .Select(p => new SelectListItem
                {
                    Value = p.Id.ToString(),
                    Text = p.NombreCliente
                });

            remitoVM.ProductoLista = _db.Producto
                .Select(p => new SelectListItem
                {
                    Value = p.NombreProducto,
                    Text = p.NombreProducto
                });

            return View(remitoVM);
        }

        public async Task<ActionResult> Ver(int id)
        {
            var remito = await _db.Remito
                                .Include(r => r.Cliente)
                                .Include(r => r.Detalles)
                                    .ThenInclude(rd => rd.Producto)
                                .FirstOrDefaultAsync(r => r.Id == id);

            if (remito == null)
            {
                return NotFound();
            }

            // Mapear los datos al objeto RemitoVM
            var remitoVM = new RemitoVM()
            {
                Remito = remito,
                RemitoDetalle = remito.Detalles,
               
            };

            return View(remitoVM);
        }

        public async Task<IActionResult> Eliminar(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var remito = await _db.Remito
                .Include(r => r.Cliente) // Incluye la información del cliente
                .FirstOrDefaultAsync(r => r.Id == id);

            if (remito == null)
            {
                return NotFound();
            }


            return View(remito);
        }



        // POST: Remito/Eliminar/{id}
        [HttpPost, ActionName("Eliminar")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Eliminar(int id)
        {
            var remito = await _db.Remito.FindAsync(id);
            var remitoId = remito.Id;
            if (remito == null)
            {
                return NotFound();
            }

            // Elimina remito
            _db.Remito.Remove(remito);

            // Elimina las filas de RemitoDetalle que coincidan con el id
            var remitoDetalles = await _db.RemitoDetalle
                .Where(d => d.RemitoId == remitoId)
                .ToListAsync();

            _db.RemitoDetalle.RemoveRange(remitoDetalles);

            await _db.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        //public IActionResult ClientesConPendientes()
        //{
        //    IQueryable<Remito> query = _db.Remito.Include(r => r.Cliente);

        //    // Filtrar por remitos pendientes
        //    query = query.Where(r => r.Estado == EstadoRemito.Pendiente);

        //    var clientesConPendientes = query
        //        .Select(r => new ClienteConPendientesVM
        //        {
        //            Id = r.Cliente.Id,
        //            NombreCliente = r.Cliente.NombreCliente,
        //            Telefono = r.Cliente.Telefono,
        //            RemitosPendientes = _db.Remito.Count(subR => subR.ClienteId == r.ClienteId && subR.Estado == EstadoRemito.Pendiente)
        //        })
        //        .Distinct()
        //        .OrderBy(c => c.NombreCliente)
        //        .ToList();

        //    return View(clientesConPendientes);
        //}

        public async Task<IActionResult> ClientesConPendientes(string buscar, string ordenActual, int? numpag, string filtroActual)
        {
            IQueryable<Remito> query = _db.Remito.Include(r => r.Cliente);

            // Filtrar por remitos pendientes
            query = query.Where(r => r.Estado == EstadoRemito.Pendiente);

            if (buscar != null)
            {
                numpag = 1;
            }
            else
            {
                buscar = filtroActual;
            }

            ViewData["OrdenActual"] = ordenActual;
            ViewData["FiltroActual"] = buscar;

            if (!string.IsNullOrEmpty(buscar))
            {
                query = query.Where(r => r.Cliente.NombreCliente.Contains(buscar));
            }

            if (ordenActual == "nombrecliente")
            {
                query = query.OrderBy(r => r.Cliente.NombreCliente);
            }
            else
            {
                query = query.OrderBy(r => r.Cliente.Id); // Orden predeterminado
            }

            int cantidadregistros = 5; // Puedes ajustar la cantidad de registros por página según tus necesidades
            var paginacion = await Paginacion<ClienteConPendientesVM>.CrearPaginacion(
               query.Select(r => new ClienteConPendientesVM
               {
                   Id = r.Cliente.Id,
                   NombreCliente = r.Cliente.NombreCliente,
                   Telefono = r.Cliente.Telefono,
                   RemitosPendientes = _db.Remito.Count(subR => subR.ClienteId == r.ClienteId && subR.Estado == EstadoRemito.Pendiente)
               })
               .Distinct()
               .OrderBy(c => c.NombreCliente)
               .AsQueryable(), numpag ?? 1, cantidadregistros);

            return View(paginacion);
        }

        public IActionResult VerRemitosCliente(int clienteId)
        {
            // Obtener cliente de la base de datos
            var cliente = _db.Cliente.Find(clienteId);

            // Obtener remitos pendientes para el cliente con detalles
            var remitosPendientes = _db.Remito
                .Where(r => r.ClienteId == clienteId && r.Estado == EstadoRemito.Pendiente)
                .Select(r => new RemitoPendienteVM
                {
                    NroRemito = r.NroRemito,
                    Fecha = r.Fecha,
                    Detalles = r.Detalles.Select(d => new RemitoDetalleVM
                    {
                        NombreProducto = d.Producto.NombreProducto,
                        Cantidad = d.Cantidad,
                        PrecioUnitario = d.Producto.Precio
                    }).ToList()
                })
                .ToList();

            // Construir el ViewModel
            var clienteConPendientesVM = new ClienteConPendientesVM
            {
                Id = cliente.Id,
                NombreCliente = cliente.NombreCliente,
                Telefono = cliente.Telefono,
                ListaRemitosPendientes = remitosPendientes
            };

            // Pasar el ViewModel a la vista
            return View(clienteConPendientesVM);
        }


        [HttpPost]
        public IActionResult ConfirmarRemitos(List<int> remitos)
        {
            try
            {
                // Lógica para actualizar el estado de los remitos en la base de datos a "Pagado"
                foreach (var remitoId in remitos)
                {
                    var remito = _db.Remito.FirstOrDefault(r => r.NroRemito == remitoId);
                    if (remito != null)
                    {
                        remito.Estado = EstadoRemito.Pagado;
                    }
                }
                _db.SaveChanges();

                return Json(new { success = true });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Error al confirmar remitos." });
            }
        }

       


        // Función para obtener el último número de orden
        private int ObtenerUltimoNumeroOrden()
        {
            var ultimoOrden = _db.Remito
                .OrderByDescending(r => r.NroRemito)
                .FirstOrDefault();

            if (ultimoOrden != null)
            {
                return ultimoOrden.NroRemito + 1;
            }

            // Si no hay órdenes en la base de datos, inicia en 1
            return 1;
        }
        public int? ObtenerProductoIdPorNombre(string codigo)
        {

            var listaDeProductos = _db.Producto;

            // Buscar el ProductoId por código usando LINQ
            var producto = listaDeProductos.FirstOrDefault(p => p.NombreProducto == codigo);

            if (producto != null)
            {
                return producto.Id;
            }

            return null; // Manejar el caso en el que no se encontró el ProductoId
        }
    }
}
