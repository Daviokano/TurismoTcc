using System.ComponentModel.DataAnnotations;

namespace HorizonTravel.Models
{
    public class Avaliacao
    {
        public int IDAvaliacao { get; set; }

        [Required(ErrorMessage = "Informe a nota")]
        [Range(0, 5, ErrorMessage = "A nota deve ser entre 0 e 5")]
        public decimal notaAva { get; set; }

        public string? comentarioAva { get; set; }

        [Required(ErrorMessage = "Informe a data da avaliação")]
        [DataType(DataType.Date)]
        public DateTime dataAva { get; set; }

        // Fks
        public int IDUsu { get; set; }

        public int IDPac { get; set; }
    }
}
