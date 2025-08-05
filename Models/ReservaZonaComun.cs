using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SistemaCondominios.Models
{
    [Table("ReservasZonasComunes")]
    public class ReservaZonaComun
    {
        [Key]
        public int ReservaId { get; set; }

        [Required]
        public int PropietarioId { get; set; }

        [ForeignKey("PropietarioId")]
        public Propietario? Propietario { get; set; }

        [Required]
        public int ZonaComunId { get; set; }

        [ForeignKey("ZonaComunId")]
        public ZonaComun? ZonaComun { get; set; }

        [Required(ErrorMessage = "La fecha de la reserva es obligatoria")]
        [DataType(DataType.Date)]
        public DateTime FechaReserva { get; set; }

        [Required(ErrorMessage = "La hora de inicio es obligatoria")]
        [DataType(DataType.Time)]
        public TimeSpan HoraInicio { get; set; }

        [Required(ErrorMessage = "La hora de fin es obligatoria")]
        [DataType(DataType.Time)]
        public TimeSpan HoraFin { get; set; }

        [StringLength(200)]
        public string Notas { get; set; }

        [Required(ErrorMessage = "El estado es obligatorio")]
        [StringLength(50)]
        public string Estado { get; set; } // Pendiente, Confirmada, Cancelada
    }
}

