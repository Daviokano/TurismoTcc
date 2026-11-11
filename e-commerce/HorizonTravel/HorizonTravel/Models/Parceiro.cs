using System.ComponentModel.DataAnnotations;

namespace HorizonTravel.Models
{
    public class Parceiro
    {
        public int IDFor { get; set; }

        public string? situacaoFor { get; set; }

        [Display(Name = "CNPJ")]
        [Required(ErrorMessage = "O CNPJ é obrigatório.")]
        [RegularExpression("^\\d{2}\\.\\d{3}\\.\\d{3}/\\d{4}-\\d{2}$", ErrorMessage = "CNPJ inválido")]
        public string CNPJFor {  get; set; }

        [Display(Name = "Nome")]
        [Required(ErrorMessage = "O nome é obrigatório.")]
        public string nomeFor {  get; set; }

        [Display(Name = "Email")]
        [EmailAddress(ErrorMessage = "Email inválido.")]
        [RegularExpression(".+\\@.+\\..+", ErrorMessage = "Informe um email válido")]
        public string emailFor {  get; set; }
    }
}
