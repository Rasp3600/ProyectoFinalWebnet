using Microsoft.AspNetCore.Mvc.Rendering;

namespace ProyectoFinalWebnet.Models
{
    public class LibroViewModel
    {
        
        public Libros Libro { get; set; }
        public int SelectedCategoriaId { get; set; }

        public IEnumerable<SelectListItem>? Categorias { get; set; }

     

        public bool TienePrestamoActivo { get; set; }

       // 
    }
}
