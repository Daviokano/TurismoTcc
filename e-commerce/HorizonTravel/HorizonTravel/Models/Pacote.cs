using System.ComponentModel.DataAnnotations;

namespace HorizonTravel.Models
{
    public class Pacote
    {
        public int IDPac { get; set; }

        [Required(ErrorMessage = "Informe o preço do pacote")]
        [Range(0.01, double.MaxValue, ErrorMessage = "O preço deve ser maior que zero")]
        public decimal precoPac { get; set; }

        [Required(ErrorMessage = "Informe a quantidade")]
        [Range(0, int.MaxValue, ErrorMessage = "A quantidade não pode ser negativa")]
        public int quantidadePac { get; set; }

        [Required(ErrorMessage = "Informe a descrição")]
        public string descricaoPac { get; set; }

        [Range(0, 5, ErrorMessage = "A avaliação deve ser entre 0 e 5")]
        public decimal? avaliacaoPac { get; set; }

        [Required(ErrorMessage = "Informe o caminho da imagem")]
        [StringLength(255)]
        public string fotoPac { get; set; } = string.Empty;

        [Required(ErrorMessage = "Informe o destino")]
        [StringLength(100)]
        public string destinoPac { get; set; }

        [Required(ErrorMessage = "Informe a origem")]
        [StringLength(200)]
        public string origemPac { get; set; } = string.Empty;

        [Required(ErrorMessage = "Informe a data")]
        [DataType(DataType.Date)]
        public DateTime dataPac { get; set; }

        // Fks
        [Range(1, int.MaxValue, ErrorMessage = "Informe um funcionário válido")]
        public int IDFun { get; set; }

        [Required]
        [StringLength(14)]
        public string CNPJFor { get; set; }

        public int? IDPromo { get; set; }   // opcional (pode ser NULL no banco)

        [Required]
        [StringLength(1)]
        public string situacaoPac { get; set; } = "A";
    }
}
