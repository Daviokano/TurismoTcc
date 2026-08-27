using System.ComponentModel.DataAnnotations;

namespace HorizonTravel.Models
{
    public class Endereco
    {
        public int enderecoId { get; set; }
        public string Rua { get; set; }
        public string Numero { get; set; }
        public string Bairro { get; set; }
        public string Cidade { get; set; }

        [StringLength(2)]
        public string Estado { get; set; }

        [RegularExpression(@"^\d{5}-?\d{3}$", ErrorMessage = "CEP inválido.")]
        public string CEP { get; set; }

		public virtual ICollection<Usuario> Usuario { get; set; }
	}
}
