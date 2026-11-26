using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace HorizonTravel.Models
{
    public class Promocao
    {
        public int IDPromo { get; set; }

        [Required(ErrorMessage = "Informe o nome da promoção.")]
        [StringLength(100, ErrorMessage = "O nome da promoção deve ter no máximo 100 caracteres.")]
        [DisplayName("Nome da promoção")]
        public string nomePromocao { get; set; } = string.Empty;

        [Required(ErrorMessage = "Informe a abrangência da promoção.")]
        [StringLength(100, ErrorMessage = "A abrangência deve ter no máximo 100 caracteres.")]
        [DisplayName("Abrangência")]
        public string abrangenciaPromocao { get; set; } = "Todos";

        [Required(ErrorMessage = "Informe a porcentagem de desconto.")]
        //eese aqui é o check do banco de dados, para garantir que o valor esteja entre 0 e 100
        [Range(typeof(decimal), "0", "100", ErrorMessage = "O desconto deve estar entre 0 e 100.")]
        [DisplayName("Desconto (%)")]
        public decimal porcentagemDesconto { get; set; }

        [Required(ErrorMessage = "Informe a data de validade.")]
        [DataType(DataType.Date)]
        [DisplayName("Data de validade")]
        public DateTime dataValidade { get; set; }

        [Required]
        [StringLength(1)]
        public string situacaoPromocao { get; set; } = Constant.SituacaoPromocaoConstant.Ativo;

    }
}
