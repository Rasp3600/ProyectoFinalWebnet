using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ProyectoFinalWebnet.Data;
using ProyectoFinalWebnet.Models;
using System.Linq;
using System.Threading.Tasks;

namespace ProyectoFinalWebnet.Controllers
{
    [Authorize]
    public class HistorialPrestamosController : Controller
    {
        private readonly ApplicationDbContext _context;

        public HistorialPrestamosController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> HistorialPrestamo(string searchString)
        {
            // Filtrar por nombre de usuario si se proporciona un valor de búsqueda
            if (!string.IsNullOrEmpty(searchString))
            {
                var historialPrestamos = await _context.HistorialPrestamos
                    .Where(p => p.UserName.Contains(searchString))
                    .ToListAsync();

                return View(historialPrestamos);
            }
            else
            {
                // Si no se proporciona searchString, mostrar todos los registros
                var historialPrestamos = await _context.HistorialPrestamos.ToListAsync();
                return View(historialPrestamos);
            }
        }
    }
}
