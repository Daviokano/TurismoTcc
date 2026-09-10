using System.ComponentModel.DataAnnotations;

namespace HorizonTravel.Models
{
    public class Usuario
    {
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

<<<<<<< HEAD
        public int IDEnd { get; set; }

        [Display(Name = "Endereço")]
=======
		public int enderecoId { get; set; }

		[Display(Name = "Endereço")]
>>>>>>> a5f08bc4d6687722b9189c5ad78a9a3155867564
        [Required(ErrorMessage = "O endereço é obrigatório.")]
        public Endereco Endereco {get; set;}

        [Display(Name = "Data de nascimento")]
        [Required(ErrorMessage = "A data de nascimento é obrigatória.")]
<<<<<<< HEAD
        public DateTime dataNascismentoUsu {get; set;}
=======
        public DateTime dataNascismento {get; set;}
>>>>>>> a5f08bc4d6687722b9189c5ad78a9a3155867564

        [Display(Name = "Senha do usuário")]
        [Required(ErrorMessage = "A senha é obrigatória.")]
        [DataType(DataType.Password)]
        [StringLength(8, ErrorMessage = "A senha deve ter 8 caracteres.")]
        public string senhaUsu {get; set;}
    }
}
