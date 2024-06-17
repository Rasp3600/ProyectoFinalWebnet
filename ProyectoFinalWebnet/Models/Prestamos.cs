using Microsoft.AspNetCore.Identity;

namespace ProyectoFinalWebnet.Models
{
    public class Prestamos
    {
        public int ID { get; set; }
        public int LibroID { get; set; }
        public string UsuarioID { get; set; } 

        public DateTime FechaPrestamos { get; set; }
        public DateTime DevolucionReal { get; set; }

        public string Estado {  get; set; }
        public string UserName { get; set; }

        public Libros Libros { get; set; }
        
    }
}
