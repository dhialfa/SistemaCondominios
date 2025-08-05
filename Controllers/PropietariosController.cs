using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SistemaCondominios.Data;
using SistemaCondominios.Models;

namespace SistemaCondominios.Controllers
{
    public class PropietariosController : Controller
    {
        private readonly ApplicationDbContext _context;

        public PropietariosController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Propietarios
        public async Task<IActionResult> Index()
        {
            return View(await _context.Propietarios.ToListAsync());
        }

        // GET: Propietarios/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
                return NotFound();

            var propietario = await _context.Propietarios
                .FirstOrDefaultAsync(m => m.PropietarioId == id);

            if (propietario == null)
                return NotFound();

            return View(propietario);
        }

        // GET: Propietarios/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Propietarios/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("PropietarioId,NombreCompleto,Email,Telefono,Cedula,NumeroPropiedad,TorreOBloque,Estado")] Propietario propietario)
        {
            System.Diagnostics.Debug.WriteLine("🟢 Entró al POST de Crear Propietario");

            // Obtener el ID del usuario autenticado
            var userIdClaim = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;

            if (int.TryParse(userIdClaim, out int usuarioId))
            {
                propietario.UsuarioId = usuarioId;
            }
            else
            {
                System.Diagnostics.Debug.WriteLine("❌ No se pudo obtener el UsuarioId desde el Claim.");
                ModelState.AddModelError("", "Error interno al asociar el usuario actual.");
                return View(propietario);
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Add(propietario);
                    await _context.SaveChangesAsync();
                    System.Diagnostics.Debug.WriteLine("✅ Propietario guardado con éxito");
                    return RedirectToAction(nameof(Index));
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine($"❌ Error al guardar: {ex.Message}");
                    ModelState.AddModelError("", "Ocurrió un error al guardar el propietario.");
                }
            }
            else
            {
                System.Diagnostics.Debug.WriteLine("❗ ModelState inválido:");
                foreach (var entry in ModelState)
                {
                    foreach (var error in entry.Value.Errors)
                    {
                        System.Diagnostics.Debug.WriteLine($"❌ {entry.Key}: {error.ErrorMessage}");
                    }
                }
            }

            return View(propietario);
        }



        // GET: Propietarios/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
                return NotFound();

            var propietario = await _context.Propietarios.FindAsync(id);
            if (propietario == null)
                return NotFound();

            return View(propietario);
        }

        // POST: Propietarios/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("PropietarioId,NombreCompleto,Email,Telefono,Cedula,NumeroPropiedad,TorreOBloque,Estado")] Propietario propietario)
        {
            if (id != propietario.PropietarioId)
                return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(propietario);
                    await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(Index));
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!PropietarioExists(propietario.PropietarioId))
                        return NotFound();
                    else
                        throw;
                }
            }
            return View(propietario);
        }

        // GET: Propietarios/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
                return NotFound();

            var propietario = await _context.Propietarios
                .FirstOrDefaultAsync(m => m.PropietarioId == id);

            if (propietario == null)
                return NotFound();

            return View(propietario);
        }

        // POST: Propietarios/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var propietario = await _context.Propietarios.FindAsync(id);
            if (propietario != null)
                _context.Propietarios.Remove(propietario);

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool PropietarioExists(int id)
        {
            return _context.Propietarios.Any(e => e.PropietarioId == id);
        }

    }
}
