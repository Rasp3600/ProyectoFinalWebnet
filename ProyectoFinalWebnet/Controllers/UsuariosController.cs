using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using ProyectoFinalWebnet.Data;
using ProyectoFinalWebnet.Models;

namespace ProyectoFinalWebnet.Views
{
    public class UsuariosController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public UsuariosController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // GET: Usuarios
        [Authorize(Roles = "Manager")]
        public async Task<IActionResult> Index()
        {
            return View(await _context.Usuarios.ToListAsync());
        }

        // GET: Usuarios/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var usuarios = await _context.Usuarios
                .FirstOrDefaultAsync(m => m.ID == id);
            if (usuarios == null)
            {
                return NotFound();
            }

            return View(usuarios);
        }

        public async Task<IActionResult> Detailuser()
        {
            var identityId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(identityId))
            {
                return NotFound();
            }

            var usuario = await _context.Usuarios
                .FirstOrDefaultAsync(m => m.IdentityId == identityId);
            if (usuario == null)
            {
                return NotFound();
            }
            ViewBag.IsUser = true;

            return View("Details", usuario); 
        }



        // GET: Usuarios/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Usuarios/Create
       
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("ID,Nombre,Apellido,Email,Telefono,Direccion,Rol")] Usuarios usuarios)
        {
            if (ModelState.IsValid)
            {
                _context.Add(usuarios);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(usuarios);
        }

        // GET: Usuarios/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var usuarios = await _context.Usuarios.FindAsync(id);
            if (usuarios == null)
            {
                return NotFound();
            }
            return View(usuarios);
        }

        // POST: Usuarios/Edit/5

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("ID,Nombre,Apellido,Email,Telefono,Direccion,Rol,UserName,IdentityId")] Usuarios usuarios)
        {
          
            Console.WriteLine(usuarios.ID);
            if (id != usuarios.ID)
            {
                return NotFound();
            }
            if (!ModelState.IsValid)
            {
                foreach (var error in ModelState.Values.SelectMany(v => v.Errors))
                {
                    Console.WriteLine(error.ErrorMessage);
                }
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(usuarios);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!UsuariosExists(usuarios.ID))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(usuarios);
        }

        // GET: Usuarios/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var usuarios = await _context.Usuarios
                .FirstOrDefaultAsync(m => m.ID == id);
            if (usuarios == null)
            {
                return NotFound();
            }

            return View(usuarios);
        }

        // POST: Usuarios/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            // Buscar el usuario en tu tabla Usuarios
            var usuario = await _context.Usuarios.FindAsync(id);

            if (usuario != null)
            {
                // Guardar el IdentityId antes de eliminar el usuario
                var identityId = usuario.IdentityId;

             
                var prestamosActivos = await _context.Prestamos
                    .AnyAsync(p => p.UsuarioID == identityId ); 

                if (prestamosActivos)
                {

                    TempData["ErrorMessage"] = "No se puede eliminar el usuario porque tiene préstamos activos 2.";
                    return RedirectToAction(nameof(Index)); 
                }

                // Eliminar el usuario de tu tabla Usuarios
                _context.Usuarios.Remove(usuario);

                // Guardar los cambios en la base de datos
                await _context.SaveChangesAsync();

                // Buscar el usuario en la tabla de Identity (AspNetUsers) y eliminarlo
                var identityUser = await _userManager.FindByIdAsync(identityId);
                if (identityUser != null)
                {
                    var result = await _userManager.DeleteAsync(identityUser);
                    if (!result.Succeeded)
                    {
                        // Manejar los errores si la eliminación del usuario de Identity falla
                        foreach (var error in result.Errors)
                        {
                            ModelState.AddModelError(string.Empty, error.Description);
                        }
                       
                        return RedirectToAction(nameof(Index)); 
                    }
                }
            }

            // Redirigir al índice después de eliminar
            return RedirectToAction(nameof(Index));
        }



        private bool UsuariosExists(int id)
        {
            return _context.Usuarios.Any(e => e.ID == id);
        }
    }
}
