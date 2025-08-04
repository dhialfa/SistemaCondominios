using System.ComponentModel.DataAnnotations;

namespace SistemaCondominios.Models
{
    public class ZonaComun
    {
        [Key]
        public int ZonaComunId { get; set; }

        [Required(ErrorMessage = "El nombre es obligatorio")]
        [StringLength(100)]
        public string Nombre { get; set; }

        [StringLength(250)]
        public string Descripcion { get; set; }

        [Required(ErrorMessage = "La capacidad es obligatoria")]
        public int Capacidad { get; set; }

        [StringLength(100)]
        public string Ubicacion { get; set; }

        [Display(Name = "Disponible")]
        public bool EstaDisponible { get; set; } = true;
    }
}
