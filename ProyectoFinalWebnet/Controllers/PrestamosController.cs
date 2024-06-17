using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ProyectoFinalWebnet.Data; 
using ProyectoFinalWebnet.Models; 
using System;
using System.Security.Claims;
using System.Threading.Tasks;

namespace ProyectoFinalWebnet.Controllers
{
    public class PrestamosController : Controller
    {
        private readonly ApplicationDbContext _context;

        public PrestamosController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Prestamos(string userName)
        {
            IQueryable<Prestamos> prestamosQuery = _context.Prestamos.Include(p => p.Libros);

            if (!string.IsNullOrEmpty(userName))
            {
                prestamosQuery = prestamosQuery.Where(p => p.UserName == userName);
            }

            var prestamos = await prestamosQuery.ToListAsync();

            return View(prestamos);
        }
        [Authorize(Roles = "User")] 
 
        public async Task<IActionResult> MiPrestamo()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            // Verifica que userId tenga un valor válido
            if (string.IsNullOrEmpty(userId))
            {
                // Manejar el caso donde userId no está disponible
                return RedirectToAction("Index", "Libros");
            }

            var miPrestamo = await _context.Prestamos
                .Include(p => p.Libros)
                .FirstOrDefaultAsync(p => p.UsuarioID == userId );

            if (miPrestamo == null)
            {
             
                return RedirectToAction("Index", "Libros");
            }

            return View(miPrestamo);
        }



        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Manager")] 
        public async Task<IActionResult> DevolverLibro(int prestamoId)
        {
            // Buscar el préstamo por su ID
            var prestamo = await _context.Prestamos.FindAsync(prestamoId);

            if (prestamo == null)
            {
                TempData["Error"] = "El préstamo no existe.";
                return RedirectToAction("Prestamos");
            }

            try
            {
                // Guardar los datos en HistorialPrestamos
                var historialPrestamo = new HistorialPrestamos
                {
                    LibroID = prestamo.LibroID,
                    UsuarioID = prestamo.UsuarioID,
                    FechaPrestamos = prestamo.FechaPrestamos,
                    DevolucionReal = DateTime.Now, // Fecha de devolución real
                    Estado = "Devuelto",
                    UserName = prestamo.UserName,
                };

                _context.HistorialPrestamos.Add(historialPrestamo);
                await _context.SaveChangesAsync();

                // Eliminar el préstamo de la tabla de Prestamos
                _context.Prestamos.Remove(prestamo);
                await _context.SaveChangesAsync();

                // Incrementar la cantidad de libros disponibles en la tabla Libros
                var libro = await _context.Libros.FindAsync(prestamo.LibroID);
                if (libro != null)
                {
                    libro.Cantidad=libro.Cantidad+1; // Incrementa en 1 la cantidad de libros disponibles
                    _context.Libros.Update(libro);
                    await _context.SaveChangesAsync();
                }

                TempData["Exito"] = "El libro ha sido devuelto correctamente.";
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Hubo un error al procesar la devolución del libro.";
           
            }

            return RedirectToAction("Prestamos");
        }
    }
}
