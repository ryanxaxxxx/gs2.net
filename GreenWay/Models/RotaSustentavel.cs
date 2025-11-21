using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GreenWay.Models
{
    /// <summary>
    /// Representa uma rota sustentável sugerida (transporte público ou bicicleta).
    /// </summary>
    public class RotaSustentavel
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [ForeignKey("Colaborador")]
        public int ColaboradorId { get; set; }

        public Colaborador? Colaborador { get; set; }

        [Required]
        [StringLength(255)]
        public string Origem { get; set; } = string.Empty;

        [Required]
        [StringLength(255)]
        public string Destino { get; set; } = string.Empty;

        [Required]
        [StringLength(50)]
        public string TipoRota { get; set; } = string.Empty; // Transporte Público, Bicicleta, Caminhada

        [Required]
        public double DistanciaKm { get; set; }

        [Required]
        [StringLength(20)]
        public string TempoEstimado { get; set; } = string.Empty; // Ex: "45 min"

        [Required]
        public double Co2PoupadoKg { get; set; } // CO₂ que seria poupado usando esta rota

        [StringLength(500)]
        public string? DescricaoRota { get; set; } // Descrição detalhada da rota

        [StringLength(50)]
        public string Status { get; set; } = "Sugerida"; // Sugerida, Aceita, Rejeitada

        public DateTime DataSugestao { get; set; } = DateTime.Now;
    }
}
