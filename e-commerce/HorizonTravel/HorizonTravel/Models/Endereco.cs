using System.ComponentModel.DataAnnotations;

namespace HorizonTravel.Models
{
    public class Endereco
    {
<<<<<<< HEAD
        public int IDEnd {  get; set; }
=======
        public int enderecoId { get; set; }
        public string Rua { get; set; }
        public string Numero { get; set; }
        public string Bairro { get; set; }
        public string Cidade { get; set; }
>>>>>>> a5f08bc4d6687722b9189c5ad78a9a3155867564

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
<<<<<<< HEAD
        public string CEPEnd { get; set; }
    }
=======
        public string CEP { get; set; }

		public virtual ICollection<Usuario> Usuario { get; set; }
	}
>>>>>>> a5f08bc4d6687722b9189c5ad78a9a3155867564
}
