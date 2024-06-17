

using Microsoft.AspNetCore.Identity;

namespace ProyectoFinalWebnet.Models
{
    public class ApplicationUser : IdentityUser
    {
      
        public string Nombre { get; set; }
        public string Apellido { get; set; }
        public string Telefono { get; set; }
        public string Direccion { get; set; }
        
  
    }
}
