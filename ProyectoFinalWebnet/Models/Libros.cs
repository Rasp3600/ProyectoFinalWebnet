using Microsoft.AspNetCore.Mvc.Rendering;

namespace ProyectoFinalWebnet.Models
{
    public class Libros
    {
        public int ID { get; set; }
        public string ? Titulo { get; set; }
        public string ? Autor { get; set; }
        public string ? Genero { get; set; }
        public string ? ISBN { get; set; }
        public  DateTime FechaPublicacion { get; set; }
        public string ? Editorial { get; set; }
        public int ? Cantidad { get; set; } 
        public int ?TotalCopias { get; set; }
        public int ? Id_categoria { get; set; } 
        public Categorias ? Categoria { get; set; } 

        
    }
}
