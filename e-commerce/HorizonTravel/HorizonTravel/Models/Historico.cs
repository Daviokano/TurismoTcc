using System.ComponentModel.DataAnnotations;

namespace HorizonTravel.Models
{
    public class Historico
    {
        public int IDHist { get; set; }

        [StringLength(40)]
        public string? statusAnterior { get; set; }

        [Required(ErrorMessage = "Informe a data da modificação")]
        [DataType(DataType.DateTime)]
        public DateTime dataModificacao { get; set; }

        // Fk
        public int IDRes { get; set; }
    }
}
