using System.ComponentModel.DataAnnotations;

namespace HorizonTravel.Models
{
    public class Usuario
    {
        [Display(Name = "Código de usuário")]
        [Required(ErrorMessage = "O código é obrigatório.")]
        public int IDUsu {get; set;}

        [Display(Name = "CPF")]
        [Required(ErrorMessage = "O CPF é obrigatório.")]
        public string CPFUsu {get; set;}

        [Display(Name = "Nome Completo")]
        [Required(ErrorMessage = "O nome é obrigatório.")]
        public string nomeUsu {get; set;}

        [Display(Name = "Email")]
        [EmailAddress(ErrorMessage = "Email inválido.")]
        [RegularExpression(".+\\@.+\\..+", ErrorMessage = "Informe um email válido")]
        public string emailUsu {get; set;}

        [Display(Name = "Telefone")]
        [Required(ErrorMessage = "O Telefone é obrigatório.")]
        public string telefoneUsu {get; set;}

        [Display(Name = "Endereço")]
        [Required(ErrorMessage = "O endereço é obrigatório.")]
        public Endereco enderecoUsu {get; set;}

        [Display(Name = "Data de nascimento")]
        [Required(ErrorMessage = "A data de nascimento é obrigatória.")]
        public DateOnly dataNascismento {get; set;}

        [Display(Name = "Senha do usuário")]
        [Required(ErrorMessage = "A senha é obrigatória.")]
        [DataType(DataType.Password)]
        [StringLength(8, ErrorMessage = "A senha deve ter 8 caracteres.")]
        public string senhaUsu {get; set;}
    }
}
