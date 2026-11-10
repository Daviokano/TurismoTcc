namespace HorizonTravel.Models
{
    public class ItemCarrinho
    {
        public int IdPacote { get; set; }
        public string NomePacote { get; set; } = string.Empty;
        public string Destino { get; set; } = string.Empty;
        public decimal Preco { get; set; }
        public int Quantidade { get; set; }
        public string Imagem { get; set; } = string.Empty;
    }
}
