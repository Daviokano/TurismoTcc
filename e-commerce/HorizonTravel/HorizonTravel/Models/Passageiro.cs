using System.ComponentModel.DataAnnotations;

namespace HorizonTravel.Models
{
    public class Passageiro
    {
        public int IDPassageiro { get; set; }

        public int IDRes { get; set; }

        [Required(ErrorMessage = "Informe o nome completo")]
        [StringLength(150)]
        public string nomeCompleto { get; set; }

        [Required(ErrorMessage = "Informe o CPF")]
        [StringLength(11)]
        public string CPF { get; set; }

        [Required(ErrorMessage = "Informe a data de nascimento")]
        [DataType(DataType.Date)]
        public DateTime dataNascimento { get; set; }

        [StringLength(30)]
        public string? passaporte { get; set; }
    }
}
