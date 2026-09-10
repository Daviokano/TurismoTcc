using System.ComponentModel.DataAnnotations;

namespace HorizonTravel.Models
{
    public class Endereco
    {
        public int IDEnd {  get; set; }

        [Display(Name = "Logradouro")]
        [EmailAddress(ErrorMessage = "O logradouro é obrigatório.")]
        public string logEnd { get; set; }

        [Display(Name = "Número")]
        [EmailAddress(ErrorMessage = "O número é obrigatório.")]
        public string numEnd { get; set; }

        [Display(Name = "Bairro")]
        [EmailAddress(ErrorMessage = "O bairro é obrigatório.")]
        public string bairroEnd { get; set; }

        [Display(Name = "Cidade")]
        [EmailAddress(ErrorMessage = "A cidade é obrigatória.")]
        public string cidEnd { get; set; }

        [Display(Name = "Estado")]
        [EmailAddress(ErrorMessage = "O estado é obrigatório.")]
        [StringLength(2)]
        public string estEnd { get; set; }

        [Display(Name = "CEP")]
        [EmailAddress(ErrorMessage = "O CEP é obrigatório.")]
        [RegularExpression(@"^\d{5}-?\d{3}$", ErrorMessage = "CEP inválido.")]
        public string CEPEnd { get; set; }
    }
}
