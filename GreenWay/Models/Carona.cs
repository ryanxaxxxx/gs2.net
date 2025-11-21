using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GreenWay.Models
{
    /// <summary>
    /// Representa uma carona organizada entre colaboradores.
    /// </summary>
    public class Carona
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [ForeignKey("Motorista")]
        public int MotoristaId { get; set; }

        public Colaborador? Motorista { get; set; }

        [Required]
        [ForeignKey("Passageiro")]
        public int PassageiroId { get; set; }

        public Colaborador? Passageiro { get; set; }

        [Required]
        public DateTime DataCarona { get; set; }

        [Required]
        [StringLength(10)]
        public string Horario { get; set; } = string.Empty; // Horário da carona

        [Required]
        [StringLength(255)]
        public string Origem { get; set; } = string.Empty; // Ponto de origem

        [Required]
        [StringLength(255)]
        public string Destino { get; set; } = string.Empty; // Ponto de destino

        [Required]
        [StringLength(50)]
        public string Status { get; set; } = "Agendada"; // Agendada, Em Andamento, Concluída, Cancelada

        public double? DistanciaKm { get; set; } // Distância percorrida em km

        public string? Observacoes { get; set; }
    }
}
