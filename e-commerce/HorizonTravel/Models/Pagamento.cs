using System.ComponentModel.DataAnnotations;

namespace HorizonTravel.Models
{
    public class Pagamento
    {
        public int IDPag { get; set; }

        [Required(ErrorMessage = "Informe o valor do pagamento")]
        [Range(0.01, double.MaxValue, ErrorMessage = "O valor deve ser maior que zero")]
        public decimal valorPag { get; set; }

        [Required(ErrorMessage = "Informe a data do pagamento")]
        [DataType(DataType.Date)]
        public DateTime dataPag { get; set; }

        [Required(ErrorMessage = "Informe o método de pagamento")]
        [StringLength(50)]
        public string metodoPag { get; set; }

        [Required(ErrorMessage = "Informe o status do pagamento")]
        [StringLength(30)]
        public string statusPag { get; set; }

        // Fk
        public int IDRes { get; set; }
    }
}
