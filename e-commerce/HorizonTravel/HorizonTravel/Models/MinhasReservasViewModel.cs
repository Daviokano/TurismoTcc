namespace HorizonTravel.Models
{
    public class MinhasReservasViewModel
    {
        public Usuario Usuario { get; set; } = new();

        public IEnumerable<ReservaResumo> Reservas { get; set; } = Enumerable.Empty<ReservaResumo>();
    }
}
