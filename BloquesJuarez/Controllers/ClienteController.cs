﻿using BloquesJuarez.Datos;
using BloquesJuarez.Models.ViewModels;
using BloquesJuarez.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace BloquesJuarez.Controllers
{
  
        public class ClienteController : Controller
        {
        private readonly AplicationDBContext _db;

        public ClienteController(AplicationDBContext db)
        {
            _db = db;
        }

        // GET: ClienteController
        public async Task<IActionResult> Index(string buscar, string ordenActual, int? numpag, string filtroActual)
            {
                IQueryable<Cliente> query = _db.Cliente.Include(p => p.Provincia).Include(c => c.CondIva);


                if (buscar != null)
                    numpag = 1;
                else
                    buscar = filtroActual;

                ViewData["OrdenActual"] = ordenActual;
                ViewData["FiltroActual"] = buscar;

                if (!String.IsNullOrEmpty(buscar))
                {
                    query = query.Where(p => p.NombreCliente.Contains(buscar));
                }

                ViewData["FiltroNombre"] = String.IsNullOrEmpty(ordenActual) ? "NombreDescendente" : "";

                switch (ordenActual)
                {
                    case "NombreDescendente":
                        query = query.OrderByDescending(cliente => cliente.NombreCliente);
                        break;
                    default:
                        query = query.OrderBy(cliente => cliente.NombreCliente);
                        break;
                }

                int cantidadregistros = 5;
                var paginacion = await Paginacion<Cliente>.CrearPaginacion(query, numpag ?? 1, cantidadregistros);
                return View(paginacion);
            }

        // GET: ClienteController/Crear
        public IActionResult Crear()
            {
                ClienteVM clienteVM = new()
                {
                    Cliente = new Cliente(),
                    ProvinciaLista = _db.Provincia.Select(p => new SelectListItem
                    {
                        Text = p.NombreProvincia,
                        Value = p.Id.ToString()
                    }),
                    CondIvaLista = _db.CondIva.Select(c => new SelectListItem
                    {
                        Text = c.Iva,
                        Value = c.Id.ToString()
                    })
                };
                return View(clienteVM);
            }

        // POST: ClienteController/Crear
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Crear(ClienteVM clienteVM)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    if (_db.Cliente.Any(c => c.NombreCliente == clienteVM.Cliente.NombreCliente))
                    {
                        TempData[WC.Error] = "El cliente ya existe!";

                        ModelState.AddModelError("Nombre", "El cliente ya existe!");
                        return View(clienteVM);
                    }
                    _db.Cliente.Add(clienteVM.Cliente);
                    _db.SaveChanges();
                    TempData[WC.Exitosa] = "El cliente se creó con éxito!";
                    return RedirectToAction(nameof(Index));
                }
                
            }
            catch (Exception ex)
            {
                
                Console.WriteLine($"Error inesperado: {ex.Message}");
                
            }

            return View(clienteVM);
        }


        // GET: ClienteController/Editar
        public async Task<ActionResult> Editar(int id)
            {
                var cliente = await _db.Cliente.FindAsync(id);
                if (cliente == null)
                {
                    return NotFound();
                }

                // Crear un objeto ClienteVM y asignar el cliente recuperado
                ClienteVM clienteVM = new ClienteVM()
                {
                    Cliente = cliente,
                    ProvinciaLista = _db.Provincia.Select(p => new SelectListItem
                    {
                        Text = p.NombreProvincia,
                        Value = p.Id.ToString()
                    }),
                    CondIvaLista = _db.CondIva.Select(c => new SelectListItem
                    {
                        Text = c.Iva,
                        Value = c.Id.ToString()
                    })
                };

                return View(clienteVM);
            }


            // POST: ClienteController/Editar
            [HttpPost]
            [ValidateAntiForgeryToken]
            public async Task<ActionResult> Editar(int id, ClienteVM clienteVM)
            {
                if (id != clienteVM.Cliente.Id)
                {
                    return NotFound();
                }

                if (ModelState.IsValid)
                {
                    try
                    {
                        // Actualizar los valores del cliente original con los valores del ViewModel
                        var clienteOriginal = await _db.Cliente.FindAsync(id);
                        if (clienteOriginal == null)
                        {
                            return NotFound();
                        }

                        clienteOriginal.NombreCliente = clienteVM.Cliente.NombreCliente;
                        clienteOriginal.Direccion = clienteVM.Cliente.Direccion;
                        clienteOriginal.Localidad = clienteVM.Cliente.Localidad;
                        clienteOriginal.ProvinciaId = clienteVM.Cliente.ProvinciaId;
                        clienteOriginal.Telefono = clienteVM.Cliente.Telefono;
                        clienteOriginal.CondIvaId = clienteVM.Cliente.CondIvaId;
                        clienteOriginal.Cuit = clienteVM.Cliente.Cuit;

                        _db.Entry(clienteOriginal).State = EntityState.Modified;
                        TempData[WC.Exitosa] = "El cliente se modificó correctamente.";
                        await _db.SaveChangesAsync();
                        return RedirectToAction(nameof(Index));
                    }
                    catch (DbUpdateConcurrencyException)
                    {
                        // Manejar excepciones de concurrencia si es necesario
                        throw;
                    }
                }

                // Si el modelo no es válido, retornar a la vista con el ViewModel
                clienteVM.ProvinciaLista = _db.Provincia.Select(p => new SelectListItem
                {
                    Text = p.NombreProvincia,
                    Value = p.Id.ToString()
                });
                clienteVM.CondIvaLista = _db.CondIva.Select(c => new SelectListItem
                {
                    Text = c.Iva,
                    Value = c.Id.ToString()
                });
            TempData[WC.Error] = "Error al modificar los datos del cliente.";
            return View(clienteVM);
            }


            // GET: ClienteController/Eliminar
            public async Task<ActionResult> Eliminar(int id)
            {
                var cliente = await _db.Cliente.FindAsync(id);
                if (cliente == null)
                {
                    return NotFound();
                }
                return View(cliente);
            }

            // POST: ClienteController/ConfirmarEliminar
            [HttpPost]
            [ValidateAntiForgeryToken]
            public async Task<ActionResult> ConfirmarEliminar(int id)
            {
                var cliente = await _db.Cliente.FindAsync(id);
                if (cliente == null)
                {
                    TempData[WC.Error] = "No se encontró el cliente a eliminar!";
                    return NotFound();
                }
                _db.Cliente.Remove(cliente);
                await _db.SaveChangesAsync();
            TempData[WC.Exitosa] = "El cliente se eliminó con éxito.";
            return RedirectToAction(nameof(Index));
            }
        }
    }
