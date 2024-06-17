using System.ComponentModel.DataAnnotations;

namespace ProyectoFinalWebnet.Models
{
    public class Usuarios
    {
        public int ID { get; set; }
        public string IdentityId { get; set; }

        public string UserName { get; set; }
        public string Nombre { get; set; }
        public string Apellido { get; set; }
        public string Email { get; set; }

        [Display(Name = "Teléfono")]
        [RegularExpression(@"^\d{4}-\d{4}$", ErrorMessage = "El campo Teléfono debe tener un formato válido. Ejemplo: 1234-5678")]
        public string Telefono { get; set; }
        public string Direccion { get; set; }

        public int Rol { get; set; }

       
    }
}
