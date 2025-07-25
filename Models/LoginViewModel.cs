using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;

namespace SistemaCondominios.Models
{
    public class LoginViewModel: Controller
    {
        [Required(ErrorMessage = "El correo es obligatorio")]
        [EmailAddress]
        public string Email { get; set; }

        [Required(ErrorMessage = "La contraseña es obligatoria")]
        [DataType(DataType.Password)]
        public string Password { get; set; }
    }
}
