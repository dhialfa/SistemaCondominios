using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SistemaCondominios.Data;
using SistemaCondominios.Models;

namespace SistemaCondominios.Controllers
{
    [Authorize(Roles = "Guarda,SuperAdministrador")]
    public class VehiculosController : Controller
    {
        private readonly ApplicationDbContext _context;

        public VehiculosController(ApplicationDbContext context)
        {
            _context = context;
        }

        // INDEX: listado + búsqueda
        public async Task<IActionResult> Index(string placa)
        {
            var vehiculos = from v in _context.Vehiculos select v;

            if (!string.IsNullOrEmpty(placa))
            {
                vehiculos = vehiculos.Where(v => v.Placa.Contains(placa));

                var resultados = await vehiculos.ToListAsync();

                if (resultados.Count == 0)
                {
                    TempData["MensajeError"] = $"No se encontraron vehículos con la placa: {placa}";
                }
                else
                {
                    TempData["MensajeExito"] = $"Vehículo encontrado: {resultados.First().Placa} - {resultados.First().NombrePropietario}";
                }

                return View(resultados);
            }

            return View(await vehiculos.ToListAsync());
        }


        // DETAILS
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();

            var vehiculo = await _context.Vehiculos.FindAsync(id);
            if (vehiculo == null)
            {
                TempData["Error"] = "Vehículo no encontrado.";
                return RedirectToAction(nameof(Index));
            }

            return View(vehiculo);
        }

        // CREATE
        public IActionResult Create()
        {
            return View("Create", new Vehiculo());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Vehiculo vehiculo)
        {
            if (ModelState.IsValid)
            {
                _context.Add(vehiculo);
                await _context.SaveChangesAsync();
                TempData["Success"] = "Vehículo registrado correctamente.";
                return RedirectToAction(nameof(Index));
            }

            TempData["Error"] = "Por favor corrija los errores del formulario.";
            return View("Create", vehiculo);
        }

        // EDIT
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var vehiculo = await _context.Vehiculos.FindAsync(id);
            if (vehiculo == null)
            {
                TempData["Error"] = "Vehículo no encontrado.";
                return RedirectToAction(nameof(Index));
            }

            return View("Edit", vehiculo);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Vehiculo vehiculo)
        {
            if (id != vehiculo.VehiculoId) return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(vehiculo);
                    await _context.SaveChangesAsync();
                    TempData["Success"] = "Vehículo actualizado correctamente.";
                    return RedirectToAction(nameof(Index));
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!VehiculoExists(vehiculo.VehiculoId))
                    {
                        TempData["Error"] = "El vehículo ya no existe.";
                        return RedirectToAction(nameof(Index));
                    }
                    throw;
                }
            }

            TempData["Error"] = "No se pudo guardar la edición.";
            return View("Edit", vehiculo);
        }

        // DELETE
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();

            var vehiculo = await _context.Vehiculos.FindAsync(id);
            if (vehiculo == null)
            {
                TempData["Error"] = "Vehículo no encontrado.";
                return RedirectToAction(nameof(Index));
            }

            return View(vehiculo);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var vehiculo = await _context.Vehiculos.FindAsync(id);
            if (vehiculo != null)
            {
                _context.Vehiculos.Remove(vehiculo);
                await _context.SaveChangesAsync();
                TempData["Success"] = "Vehículo eliminado correctamente.";
            }
            else
            {
                TempData["Error"] = "No se pudo eliminar. Vehículo no encontrado.";
            }

            return RedirectToAction(nameof(Index));
        }

        private bool VehiculoExists(int id)
        {
            return _context.Vehiculos.Any(e => e.VehiculoId == id);
        }
    }
}
