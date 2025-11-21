using System.ComponentModel.DataAnnotations;

namespace GreenWay.Models
{
    /// <summary>
    /// Representa um colaborador da empresa que utiliza o sistema de mobilidade sustentável.
    /// </summary>
    public class Colaborador
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [StringLength(100)]
        public string Nome { get; set; } = string.Empty;

        [Required]
        [EmailAddress]
        [StringLength(255)]
        public string Email { get; set; } = string.Empty; // Email corporativo

        [Required]
        [StringLength(255)]
        public string Endereco { get; set; } = string.Empty; // Endereço aproximado

        [Required]
        [StringLength(50)]
        public string MeioTransporte { get; set; } = string.Empty; // Carro, Bicicleta, Transporte Público, etc.

        [Required]
        [StringLength(10)]
        public string HorarioEntrada { get; set; } = string.Empty; // Ex: "08:00"

        [Required]
        [StringLength(10)]
        public string HorarioSaida { get; set; } = string.Empty; // Ex: "18:00"

        public bool DisponivelCaronas { get; set; } = true; // Se está disponível para oferecer/receber caronas

        public string? Observacoes { get; set; } // Observações adicionais sobre rotas ou preferências
    }
}
