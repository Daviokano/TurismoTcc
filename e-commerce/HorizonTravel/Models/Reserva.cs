using System.ComponentModel.DataAnnotations;

namespace HorizonTravel.Models
{
    public class Reserva
    {
        public int IDRes { get; set; }

        [Required(ErrorMessage = "Informe a data da reserva")]
        [DataType(DataType.Date)]
        public DateTime dataRes { get; set; }

        [Required(ErrorMessage = "Informe o status da reserva")]
        [StringLength(40)]
        public string statusRes { get; set; }

        // fks
        public int IDUsu { get; set; }

        public int IDPac { get; set; }
    }
}
