using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace HorizonTravel.Models
{
    public class Promocao
    {
       public int IDPromo { get; set; }

       [Required(ErrorMessage = "Informe a porcentagem de desconto.")]
        //eese aqui é o check do banco de dados, para garantir que o valor esteja entre 0 e 100
        [Range(typeof(decimal), "0", "100", ErrorMessage = "O desconto deve estar entre 0 e 100.")]
       [DisplayName("Desconto (%)")]
       public decimal porcentagemDesconto { get; set; }

       [Required(ErrorMessage = "Informe a data de validade.")]
       [DataType(DataType.Date)]
       [DisplayName("Data de validade")]
       public DateTime dataValidade { get; set; }

    }
}
