using System.ComponentModel.DataAnnotations;

namespace HorizonTravel.Models
{
    public class ReservaCheckout
    {
        public int IDPac { get; set; }

        public decimal ValorTotal { get; set; }

        [Required(ErrorMessage = "Informe o método de pagamento")]
        public string metodoPag { get; set; }

        [StringLength(100)]
        public string? nomeTitular { get; set; }

        [StringLength(19)]
        public string? numeroCartao { get; set; }

        [RegularExpression(@"^(0[1-9]|1[0-2])\/\d{2}$", ErrorMessage = "Informe a validade no formato MM/AA")]
        public string? validadeCartao { get; set; }

        [RegularExpression(@"^\d{3,4}$", ErrorMessage = "Informe um CVV válido")]
        public string? cvv { get; set; }

        public List<Passageiro> Passageiros { get; set; } = new();
    }
}
