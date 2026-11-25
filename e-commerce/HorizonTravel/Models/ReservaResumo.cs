namespace HorizonTravel.Models
{
    public class ReservaResumo
    {
        public int IDRes { get; set; }

        public int IDUsu { get; set; }

        public int IDPac { get; set; }

        public string nomeUsu { get; set; } = string.Empty;

        public string emailUsu { get; set; } = string.Empty;

        public string telefoneUsu { get; set; } = string.Empty;

        public string destinoPac { get; set; } = string.Empty;

        public DateTime dataPac { get; set; }

        public DateTime dataRes { get; set; }

        public decimal valorPag { get; set; }

        public string statusRes { get; set; } = string.Empty;

        public string statusPag { get; set; } = string.Empty;

        public string metodoPag { get; set; } = string.Empty;

        public int quantidadePassageiros { get; set; }
    }
}
