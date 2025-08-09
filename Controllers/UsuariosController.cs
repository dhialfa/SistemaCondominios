using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using SistemaCondominios.Data;
using SistemaCondominios.Models;
using BCrypt.Net;   // → NuGet BCrypt.Net-Next
using Microsoft.AspNetCore.Authorization;

namespace SistemaCondominios.Controllers
{
    [Authorize(Roles = "SuperAdministrador")]
    public class UsuariosController : Controller
    {
        private readonly ApplicationDbContext _context;

        public UsuariosController(ApplicationDbContext context)
        {
            _context = context;
        }

        /*----------------------------------------------------------
         *  MÉTODO AUXILIAR: carga la lista de roles y marca
         *  el elemento actualmente seleccionado (si se pasa).
         *---------------------------------------------------------*/
        private void CargarRoles(int? seleccionado = null)
        {
            var lista = _context.Roles
                                .AsNoTracking()
                                .OrderBy(r => r.Nombre)
                                .ToList();

            ViewBag.Roles = new SelectList(lista,       // datos
                                           "RolId",     // value
                                           "Nombre",    // texto visible
                                           seleccionado /* selected */);
        }

        /*-----------------------  INDEX  ------------------------*/
        public async Task<IActionResult> Index()
        {
            var usuarios = await _context.Usuarios
                                         .AsNoTracking()
                                         .Include(u => u.Rol)   // 👈 trae el nombre del rol
                                         .ToListAsync();

            return View(usuarios);
        }

        /*----------------------  DETAILS  -----------------------*/
        public async Task<IActionResult> Details(int? id)
        {
            if (id is null) return NotFound();

            var usuario = await _context.Usuarios
                                        .AsNoTracking()
                                        .Include(u => u.Rol)
                                        .FirstOrDefaultAsync(u => u.UsuarioId == id);

            return usuario is null ? NotFound() : View(usuario);
        }

        /*-----------------------  CREATE  -----------------------*/
        // GET: Usuarios/Create
        public IActionResult Create()
        {
            CargarRoles();                 // ← lista para el <select>
            return View();
        }

        // POST: Usuarios/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(
            [Bind("UsuarioId,Nombre,Email,Password,Telefono,Activo,RolId")] Usuario usuario)
        {
            CargarRoles(usuario.RolId);    // ← recarga por si hay que volver a la vista

            if (!ModelState.IsValid)
                return View(usuario);

            usuario.Password = BCrypt.Net.BCrypt.HashPassword(usuario.Password);

            _context.Add(usuario);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        /*------------------------  EDIT  ------------------------*/
        // GET: Usuarios/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id is null) return NotFound();

            var usuario = await _context.Usuarios.FindAsync(id);
            if (usuario is null) return NotFound();

            CargarRoles(usuario.RolId);    // ← lista + selección actual
            return View(usuario);
        }

        // POST: Usuarios/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(
            int id,
            [Bind("UsuarioId,Nombre,Email,Password,Telefono,Activo,RolId")] Usuario usuario)
        {
            if (id != usuario.UsuarioId) return NotFound();

            CargarRoles(usuario.RolId);    // ← necesario si la validación falla

            if (!ModelState.IsValid)
                return View(usuario);

            try
            {
                var original = await _context.Usuarios
                                             .AsNoTracking()
                                             .FirstOrDefaultAsync(u => u.UsuarioId == id);

                if (original is null) return NotFound();

                // Si la contraseña viene vacía, conservar la existente
                usuario.Password = string.IsNullOrWhiteSpace(usuario.Password)
                                   ? original.Password
                                   : BCrypt.Net.BCrypt.HashPassword(usuario.Password);

                _context.Update(usuario);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!UsuarioExists(usuario.UsuarioId))
                    return NotFound();
                throw;
            }
        }

        /*-----------------------  DELETE  -----------------------*/
        // GET: Usuarios/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id is null) return NotFound();

            var usuario = await _context.Usuarios
                                        .AsNoTracking()
                                        .Include(u => u.Rol)
                                        .FirstOrDefaultAsync(u => u.UsuarioId == id);

            return usuario is null ? NotFound() : View(usuario);
        }

        // POST: Usuarios/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var usuario = await _context.Usuarios.FindAsync(id);
            if (usuario != null)
            {
                _context.Usuarios.Remove(usuario);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index));
        }

        /*------------------  MÉTODO DE APOYO  -------------------*/
        private bool UsuarioExists(int id) =>
            _context.Usuarios.Any(u => u.UsuarioId == id);
    }
}
