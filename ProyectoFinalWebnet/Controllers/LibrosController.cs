using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using ProyectoFinalWebnet.Data;
using ProyectoFinalWebnet.Models;

namespace ProyectoFinalWebnet.Controllers
{
    public class LibrosController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public LibrosController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // GET: Libros
        public async Task<IActionResult> Index(string searchString)
        {
            ViewData["CurrentFilter"] = searchString;

            // Obtener todos los libros
            var libros = from l in _context.Libros select l;

            // Filtrar por título si hay una cadena de búsqueda
            if (!string.IsNullOrEmpty(searchString))
            {
                libros = libros.Where(l => l.Titulo.Contains(searchString));
            }

            var librosViewModel = new List<LibroViewModel>();

            foreach (var libro in await libros.ToListAsync())
            {
                var viewModel = new LibroViewModel
                {
                    Libro = libro,
                    SelectedCategoriaId = libro.Id_categoria.HasValue ? libro.Id_categoria.Value : 0,
                    Categorias = GetCategoriasSelectList(),
                    TienePrestamoActivo = await TienePrestamoActivoParaUsuario()
                };

                librosViewModel.Add(viewModel);
            }

            return View(librosViewModel);

           
        }

        // GET: Libros/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var libros = await _context.Libros
                .FirstOrDefaultAsync(m => m.ID == id);
            if (libros == null)
            {
                return NotFound();
            }

            return View(libros);
        }
            
        // GET: Libros/Create
        public IActionResult Create()
        {
            var viewModel = new LibroViewModel
            {
                Libro = new Libros(),
          
               Categorias = GetCategoriasSelectList()
            };

            return View(viewModel);
        }

        private List<SelectListItem> GetCategoriasSelectList()
        {
            return _context.Categorias.Select(c => new SelectListItem
            {
                Value = c.ID.ToString(),
                Text = c.Nombre
            }).ToList();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(LibroViewModel viewModel)
        {

           

            if (ModelState.IsValid)
            {
                var libro = new Libros
                {
                    Titulo = viewModel.Libro.Titulo,
                    Autor = viewModel.Libro.Autor,
                    Genero = viewModel.SelectedCategoriaId.ToString(),
                    ISBN = viewModel.Libro.ISBN,
                    FechaPublicacion = viewModel.Libro.FechaPublicacion,
                    Editorial = viewModel.Libro.Editorial,
                    Cantidad = viewModel.Libro.Cantidad,
                    TotalCopias = viewModel.Libro.TotalCopias,
                    Id_categoria = viewModel.SelectedCategoriaId
               
                };

                _context.Libros.Add(libro);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            // Volver a llenar la lista de categorías en caso de error
            viewModel.Categorias = GetCategoriasSelectList();

            return View(viewModel);
        }

        // GET: Libros/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var libro = await _context.Libros.FindAsync(id);
            if (libro == null)
            {
                return NotFound();
            }

            var viewModel = new LibroViewModel
            {
                Libro = libro,
                SelectedCategoriaId = libro.Id_categoria.Value,
                Categorias = GetCategoriasSelectList()
            };

            return View(viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, LibroViewModel viewModel)
        {
            if (id != viewModel.Libro.ID)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    var libro = viewModel.Libro;
                    libro.Id_categoria = viewModel.SelectedCategoriaId;

                    _context.Update(libro);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!LibrosExists(viewModel.Libro.ID))
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

            viewModel.Categorias = GetCategoriasSelectList();
            return View(viewModel);
        }

        // GET: Libros/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var libro = await _context.Libros
                .FirstOrDefaultAsync(m => m.ID == id);
            if (libro == null)
            {
                return NotFound();
            }

            return View(libro);
        }

        // POST: Libros/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var libro = await _context.Libros.FindAsync(id);
            if (libro != null)
            {
                _context.Libros.Remove(libro);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool LibrosExists(int id)
        {
            return _context.Libros.Any(e => e.ID == id);
        }

        private async Task<bool> TienePrestamoActivoParaUsuario()
        {
            var userId = _userManager.GetUserId(User);

            var prestamosActivos = await _context.Prestamos
                .AnyAsync(p => p.UsuarioID == userId && p.Estado == "Pendiente");

            return prestamosActivos;
        }










        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "User")]
        public async Task<IActionResult> PrestarLibro(int libroId)
        {
            var userId = _userManager.GetUserId(User);
            var username = _userManager.GetUserName(User);

            // Buscar el libro por ID
            var libro = await _context.Libros.FindAsync(libroId);

            if (libro == null)
            {
                return NotFound();
            }

            // Verificar si hay al menos un ejemplar disponible
            if (libro.Cantidad <= 0)
            {
                TempData["Error"] = $"No hay ejemplares disponibles de {libro.Titulo}.";
                return RedirectToAction("Index");
            }

            // Verificar si el usuario ya tiene un préstamo activo para este libro
             var prestamoExistente = await _context.Prestamos
            .FirstOrDefaultAsync(p => p.LibroID == libroId && p.UsuarioID == userId && p.DevolucionReal == null);
          


            if (prestamoExistente != null)
            {
                TempData["Error"] = $"Ya tiene un prestamo.";
                return RedirectToAction("Index");
            }

            // Crear un nuevo registro de préstamo
            var prestamo = new Prestamos
            {
                LibroID = libroId,
                UsuarioID = userId,
                FechaPrestamos = DateTime.Now,
                DevolucionReal = DateTime.Now ,
                Estado = "Pendiente",
                UserName = username
            };

            // Actualizar la cantidad de libros disponibles
            libro.Cantidad--;

            try
            {
                _context.Prestamos.Add(prestamo);
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                TempData["Error"] = "Hubo un error al procesar el préstamo.";
                return RedirectToAction("Index");
            }

            TempData["Exito"] = $"Se prestó el libro {libro.Titulo} correctamente.";
            return RedirectToAction("Index");
        }




    }
}
