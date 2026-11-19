namespace HorizonTravel.Models
{
    public class HistoricoClienteViewModel
    {
        public string? Busca { get; set; }

        public Usuario? Cliente { get; set; }

        public IEnumerable<Usuario> Clientes { get; set; } = Enumerable.Empty<Usuario>();

        public IEnumerable<Usuario> ResultadosBusca { get; set; } = Enumerable.Empty<Usuario>();

        public IEnumerable<ReservaResumo> Reservas { get; set; } = Enumerable.Empty<ReservaResumo>();

        public int TotalReservas => Reservas.Count();

        public decimal ValorTotal => Reservas.Sum(reserva => reserva.valorPag);
    }
}
