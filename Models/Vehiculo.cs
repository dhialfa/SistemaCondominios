using System.ComponentModel.DataAnnotations;

namespace SistemaCondominios.Models
{
    public class Vehiculo
    {
        [Key]
        public int VehiculoId { get; set; }

        [Required]
        public string Placa { get; set; }

        public string TipoPropietario { get; set; } // Condómino, Inquilino, Visitante

        public string NombrePropietario { get; set; }

        public DateTime? FechaAutorizacion { get; set; }
    }

}
