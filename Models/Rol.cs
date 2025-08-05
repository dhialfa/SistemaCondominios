using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace SistemaCondominios.Models
{
    public class Rol
    {
        [Key]
        public int RolId { get; set; }

        [Required]
        [StringLength(50)]
        public string Nombre { get; set; }

        // Relación con usuarios
        public ICollection<Usuario> Usuarios { get; set; }
    }
}
