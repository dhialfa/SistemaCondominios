using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SistemaCondominios.Models
{
    public class Propietario
    {
        [Key]
        public int PropietarioId { get; set; }

        [Required(ErrorMessage = "El nombre completo es obligatorio")]
        [StringLength(150)]
        public string NombreCompleto { get; set; }

        [Required(ErrorMessage = "El correo electrónico es obligatorio")]
        [EmailAddress]
        [StringLength(100)]
        public string Email { get; set; }

        [Phone]
        [StringLength(20)]
        public string Telefono { get; set; }

        [StringLength(20)]
        public string Cedula { get; set; }


        [Required(ErrorMessage = "El número de la propiedad es obligatorio")]
        [StringLength(10)]
        public string NumeroPropiedad { get; set; }

        [StringLength(100)]
        public string TorreOBloque { get; set; }

        [StringLength(50)]
        public string Estado { get; set; } = "Activo";
        public int? UsuarioId { get; set; }

        [ForeignKey("UsuarioId")]
        public Usuario? Usuario { get; set; }


    }
}
