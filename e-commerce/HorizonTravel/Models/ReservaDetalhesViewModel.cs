namespace HorizonTravel.Models
{
    public class ReservaDetalhesViewModel
    {
        public ReservaResumo Reserva { get; set; } = new();

        public IEnumerable<Passageiro> Passageiros { get; set; } = Enumerable.Empty<Passageiro>();

        public IEnumerable<Historico> Historico { get; set; } = Enumerable.Empty<Historico>();
    }
}
