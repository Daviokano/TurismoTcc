using System.ComponentModel.DataAnnotations;

namespace HorizonTravel.Models
{
    public class Parceiro
    {
        [Display(Name = "CNPJ")]
        [Required(ErrorMessage = "O CNPJ é obrigatório.")]
        [RegularExpression("^\\d{2}\\.\\d{3}\\.\\d{3}/\\d{4}-\\d{2}$", ErrorMessage = "CNPJ inválido")]
        public int CNPJFor {  get; set; }

        [Display(Name = "Nome")]
        [Required(ErrorMessage = "O nome é obrigatório.")]
        public string nomeFor {  get; set; }

        [Display(Name = "Endereço")]
        [Required(ErrorMessage = "O endereço é obrigatório.")]
        public Endereco enderecoFor {  get; set; }

        [Display(Name = "Email")]
        [EmailAddress(ErrorMessage = "Email inválido.")]
        [RegularExpression(".+\\@.+\\..+", ErrorMessage = "Informe um email válido")]
        public string emailfor {  get; set; }
    }
}
