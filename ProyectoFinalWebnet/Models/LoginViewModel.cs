using System.ComponentModel.DataAnnotations;

namespace ProyectoFinalWebnet.Models
{
    public class LoginViewModel
    {
        [Required(ErrorMessage = "El campo Usuario es requerido.")]
        public string UserName { get; set; }

        [Required(ErrorMessage = "El campo Contraseña es requerido.")]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [Display(Name = "Recordarme")]
        public bool RememberMe { get; set; }
    }
}
