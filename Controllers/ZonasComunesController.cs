using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SistemaCondominios.Data;
using SistemaCondominios.Models;

namespace SistemaCondominios.Controllers
{
    public class ZonasComunesController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ZonasComunesController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: ZonasComunes
        public async Task<IActionResult> Index()
        {
            return View(await _context.ZonasComunes.ToListAsync());
        }

        // GET: ZonasComunes/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
                return NotFound();

            var zona = await _context.ZonasComunes
                .FirstOrDefaultAsync(m => m.ZonaComunId == id);

            if (zona == null)
                return NotFound();

            return View(zona);
        }

        // GET: ZonasComunes/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: ZonasComunes/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("ZonaComunId,Nombre,Descripcion,Capacidad,Ubicacion,EstaDisponible")] ZonaComun zonaComun)
        {
            if (ModelState.IsValid)
            {
                _context.Add(zonaComun);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(zonaComun);
        }

        // GET: ZonasComunes/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
                return NotFound();

            var zona = await _context.ZonasComunes.FindAsync(id);
            if (zona == null)
                return NotFound();

            return View(zona);
        }

        // POST: ZonasComunes/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("ZonaComunId,Nombre,Descripcion,Capacidad,Ubicacion,EstaDisponible")] ZonaComun zonaComun)
        {
            if (id != zonaComun.ZonaComunId)
                return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(zonaComun);
                    await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(Index));
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ZonaComunExists(zonaComun.ZonaComunId))
                        return NotFound();
                    else
                        throw;
                }
            }
            return View(zonaComun);
        }

        // GET: ZonasComunes/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
                return NotFound();

            var zona = await _context.ZonasComunes
                .FirstOrDefaultAsync(m => m.ZonaComunId == id);

            if (zona == null)
                return NotFound();

            return View(zona);
        }

        // POST: ZonasComunes/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var zona = await _context.ZonasComunes.FindAsync(id);
            if (zona != null)
                _context.ZonasComunes.Remove(zona);

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ZonaComunExists(int id)
        {
            return _context.ZonasComunes.Any(e => e.ZonaComunId == id);
        }
    }
}
