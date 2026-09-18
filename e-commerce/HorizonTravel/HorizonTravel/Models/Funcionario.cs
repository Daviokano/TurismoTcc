using System.ComponentModel.DataAnnotations;

namespace HorizonTravel.Models
{
    public class Funcionario
    {
        [Display(Name = "Código de Funcionário")]
        [Required(ErrorMessage = "O código é obrigatório.")]
        public int IDFun { get; set; }

        [Display(Name = "CPF")]
        [Required(ErrorMessage = "O CPF é obrigatório.")]
        [RegularExpression("^\\d{3}\\.\\d{3}\\.\\d{3}-\\d{2}$", ErrorMessage = "CPF inválido.")]
        public string CPFFun { get; set; }

        [Display(Name = "Nome Completo")]
        [Required(ErrorMessage = "O nome é obrigatório.")]
        public string nomeFun { get; set; }

        [Display(Name = "Email")]
        [EmailAddress(ErrorMessage = "Email inválido.")]
        [RegularExpression(".+\\@.+\\..+", ErrorMessage = "Informe um email válido.")]
        public string emailFun { get; set; }

        [Display(Name = "Telefone")]
        [Required(ErrorMessage = "O Telefone é obrigatório.")]
        public string telefoneFun { get; set; }

        [Display(Name = "Data de nascimento")]
        [Required(ErrorMessage = "A data de nascimento é obrigatória.")]
        public DateTime dataNascimentoFun { get; set; }

        [Display(Name = "Senha do funcionário")]
        [Required(ErrorMessage = "A senha é obrigatória.")]
        [DataType(DataType.Password)]
        [StringLength(8, ErrorMessage = "A senha deve ter 8 caracteres.")]
        public string senhaFun { get; set; }
    }
}
