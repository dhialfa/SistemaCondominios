using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace SistemaCondominios.Models
{
    public class Usuario
    {
        [Key]
        public int UsuarioId { get; set; }

        [Required(ErrorMessage = "El nombre es obligatorio")]
        [StringLength(100)]
        public string Nombre { get; set; } = string.Empty;

        [Required(ErrorMessage = "El correo es obligatorio")]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "La contraseña es obligatoria")]
        [DataType(DataType.Password)]
        public string Password { get; set; } = string.Empty;

        [Phone]
        public string? Telefono { get; set; }

        public bool Activo { get; set; } = true;

        // ← sigue siendo obligatorio seleccionar un RolId
        [Required(ErrorMessage = "El rol es obligatorio")]
        [Range(1, int.MaxValue, ErrorMessage = "Debe seleccionar un rol válido")]
        public int RolId { get; set; }

        /*--- Propiedad de navegación, NO obligatoria ---*/
        [BindNever]                // o [ValidateNever]
        [ForeignKey(nameof(RolId))]
        public Rol? Rol { get; set; }   // ← nullable
    }
}
