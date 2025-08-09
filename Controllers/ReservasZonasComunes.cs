using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using SistemaCondominios.Data;
using SistemaCondominios.Models;


namespace SistemaCondominios.Controllers
{
    [Authorize(Roles = "SuperAdministrador,AdministradorCondominal,Condominal")]
    public class ReservasZonasComunesController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ReservasZonasComunesController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: ReservasZonasComunes
        public async Task<IActionResult> Index()
        {
            // Incluimos las relaciones para mostrar en la vista
            var reservas = await _context.ReservasZonasComunes
                .Include(r => r.Propietario)
                .Include(r => r.ZonaComun)
                .ToListAsync();

            // Actualiza el estado automáticamente a "Finalizado" si la fecha y hora han pasado
            var ahora = DateTime.Now;
            bool huboCambios = false;

            foreach (var reserva in reservas)
            {
                var fechaHoraFin = reserva.FechaReserva.Add(reserva.HoraFin);
                if (fechaHoraFin <= ahora && reserva.Estado != "Finalizado")
                {
                    reserva.Estado = "Finalizado";
                    _context.Update(reserva);
                    huboCambios = true;
                }
            }

            if (huboCambios)
                await _context.SaveChangesAsync();

            return View(reservas);
        }

        // GET: ReservasZonasComunes/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
                return NotFound();

            var reserva = await _context.ReservasZonasComunes
                .Include(r => r.Propietario)
                .Include(r => r.ZonaComun)
                .FirstOrDefaultAsync(m => m.ReservaId == id);

            if (reserva == null)
                return NotFound();

            // Actualiza el estado en el detalle también
            var ahora = DateTime.Now;
            var fechaHoraFin = reserva.FechaReserva.Add(reserva.HoraFin);

            if (fechaHoraFin <= ahora && reserva.Estado != "Finalizado")
            {
                reserva.Estado = "Finalizado";
                _context.Update(reserva);
                await _context.SaveChangesAsync();
            }

            return View(reserva);
        }

        // GET: ReservasZonasComunes/Create
        public IActionResult Create()
        {
            var usuario = _context.Usuarios
                .Include(u => u.Rol)
                .FirstOrDefault(u => u.Email == User.Identity.Name);

            if (usuario?.Rol?.Nombre == "Condómino")
            {
                var propietario = _context.Propietarios
                    .FirstOrDefault(p => p.UsuarioId == usuario.UsuarioId);

                ViewBag.PropietarioId = propietario?.PropietarioId ?? 0; // usar en el input hidden
            }
            else
            {
                ViewData["PropietarioId"] = new SelectList(_context.Propietarios, "PropietarioId", "NombreCompleto");
            }

            ViewData["ZonaComunId"] = new SelectList(_context.ZonasComunes, "ZonaComunId", "Nombre");
            return View();
        }


        // POST: ReservasZonasComunes/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("ReservaId,PropietarioId,ZonaComunId,FechaReserva,HoraInicio,HoraFin,Notas,Estado")] ReservaZonaComun reserva)
        {
            var usuario = _context.Usuarios
                .Include(u => u.Rol)
                .FirstOrDefault(u => u.Email == User.Identity.Name);

            // Asignar automáticamente PropietarioId si es condómino
            if (usuario?.Rol?.Nombre == "Condómino")
            {
                var propietario = _context.Propietarios
                    .FirstOrDefault(p => p.UsuarioId == usuario.UsuarioId);

                if (propietario != null)
                    reserva.PropietarioId = propietario.PropietarioId;
            }

            // Verificar conflictos de horario
            var existeConflicto = await _context.ReservasZonasComunes
                .AnyAsync(r => r.ZonaComunId == reserva.ZonaComunId &&
                               r.FechaReserva == reserva.FechaReserva &&
                               (
                                   (reserva.HoraInicio >= r.HoraInicio && reserva.HoraInicio < r.HoraFin) ||
                                   (reserva.HoraFin > r.HoraInicio && reserva.HoraFin <= r.HoraFin) ||
                                   (reserva.HoraInicio <= r.HoraInicio && reserva.HoraFin >= r.HoraFin)
                               ) &&
                               r.Estado != "Cancelada" &&
                               r.ReservaId != reserva.ReservaId);

            if (existeConflicto)
                ModelState.AddModelError("", "La zona ya está reservada en este horario.");

            if (ModelState.IsValid)
            {
                _context.Add(reserva);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            // Volver a cargar selects si hay error
            if (usuario?.Rol?.Nombre != "Condómino")
            {
                ViewData["PropietarioId"] = new SelectList(_context.Propietarios, "PropietarioId", "NombreCompleto", reserva.PropietarioId);
            }

            ViewData["ZonaComunId"] = new SelectList(_context.ZonasComunes, "ZonaComunId", "Nombre", reserva.ZonaComunId);
            return View(reserva);
        }


        // GET: ReservasZonasComunes/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
                return NotFound();

            var reserva = await _context.ReservasZonasComunes.FindAsync(id);
            if (reserva == null)
                return NotFound();

            ViewData["PropietarioId"] = new SelectList(_context.Propietarios, "PropietarioId", "NombreCompleto", reserva.PropietarioId);
            ViewData["ZonaComunId"] = new SelectList(_context.ZonasComunes, "ZonaComunId", "Nombre", reserva.ZonaComunId);
            return View(reserva);
        }

        // POST: ReservasZonasComunes/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("ReservaId,PropietarioId,ZonaComunId,FechaReserva,HoraInicio,HoraFin,Notas,Estado")] ReservaZonaComun reserva)
        {
            if (id != reserva.ReservaId)
                return NotFound();

            // Cambia automáticamente a "Finalizado" si la fecha/hora ya pasaron
            var ahora = DateTime.Now;
            var fechaHoraFin = reserva.FechaReserva.Add(reserva.HoraFin);
            if (fechaHoraFin <= ahora)
                reserva.Estado = "Finalizado";

            // Verifica solapamiento al editar (excluyendo la reserva actual)
            var existeConflicto = await _context.ReservasZonasComunes
                .AnyAsync(r => r.ZonaComunId == reserva.ZonaComunId &&
                               r.FechaReserva == reserva.FechaReserva &&
                               (
                                   (reserva.HoraInicio >= r.HoraInicio && reserva.HoraInicio < r.HoraFin) ||
                                   (reserva.HoraFin > r.HoraInicio && reserva.HoraFin <= r.HoraFin) ||
                                   (reserva.HoraInicio <= r.HoraInicio && reserva.HoraFin >= r.HoraFin)
                               ) &&
                               r.Estado != "Cancelada" &&
                               r.ReservaId != reserva.ReservaId);

            if (existeConflicto)
                ModelState.AddModelError("", "La zona ya está reservada en este horario.");

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(reserva);
                    await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(Index));
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ReservaExists(reserva.ReservaId))
                        return NotFound();
                    else
                        throw;
                }
            }

            ViewData["PropietarioId"] = new SelectList(_context.Propietarios, "PropietarioId", "NombreCompleto", reserva.PropietarioId);
            ViewData["ZonaComunId"] = new SelectList(_context.ZonasComunes, "ZonaComunId", "Nombre", reserva.ZonaComunId);
            return View(reserva);
        }

        // GET: ReservasZonasComunes/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
                return NotFound();

            var reserva = await _context.ReservasZonasComunes
                .Include(r => r.Propietario)
                .Include(r => r.ZonaComun)
                .FirstOrDefaultAsync(m => m.ReservaId == id);

            if (reserva == null)
                return NotFound();

            return View(reserva);
        }

        // POST: ReservasZonasComunes/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var reserva = await _context.ReservasZonasComunes.FindAsync(id);
            if (reserva != null)
                _context.ReservasZonasComunes.Remove(reserva);

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ReservaExists(int id)
        {
            return _context.ReservasZonasComunes.Any(e => e.ReservaId == id);
        }
    }
}
