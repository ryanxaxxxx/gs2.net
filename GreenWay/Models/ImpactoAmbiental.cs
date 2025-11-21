using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GreenWay.Models
{
    /// <summary>
    /// Representa o cálculo de impacto ambiental evitado (CO₂ poupado, km ecológicos, etc.).
    /// </summary>
    public class ImpactoAmbiental
    {
        [Key]
        public int Id { get; set; }

        [ForeignKey("Colaborador")]
        public int? ColaboradorId { get; set; } // Opcional: pode ser geral ou por colaborador

        public Colaborador? Colaborador { get; set; }

        [ForeignKey("Carona")]
        public int? CaronaId { get; set; } // Opcional: pode ser de uma carona específica

        public Carona? Carona { get; set; }

        [Required]
        public DateTime DataRegistro { get; set; }

        [Required]
        [StringLength(50)]
        public string TipoTransporte { get; set; } = string.Empty; // Carona, Bicicleta, Transporte Público

        [Required]
        public double DistanciaKm { get; set; } // Distância percorrida em km

        [Required]
        public double Co2PoupadoKg { get; set; } // CO₂ poupado em kg (comparado com carro individual)

        [Required]
        public double KmEcologicos { get; set; } // Quilômetros ecológicos (equivalente)

        [StringLength(255)]
        public string? Descricao { get; set; } // Descrição do impacto
    }
}
