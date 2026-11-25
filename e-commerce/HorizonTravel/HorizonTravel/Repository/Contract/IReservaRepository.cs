using HorizonTravel.Models;

namespace HorizonTravel.Repository.Contract
{
    public interface IReservaRepository
    {
        int CadastrarReserva(Reserva reserva);

        void CadastrarPassageiros(
            int idReserva,
            IEnumerable<Passageiro> passageiros);

        void CadastrarPagamento(Pagamento pagamento);

        int FinalizarReserva(
            Reserva reserva,
            IEnumerable<Passageiro> passageiros,
            Pagamento pagamento);

        IEnumerable<ReservaResumo> ObterReservasPorUsuario(int idUsuario);

        IEnumerable<Historico> BuscarHistoricoPorCliente(int idUsuario);

        void RegistrarAlteracao(
            int idReserva,
            int? idFuncionario,
            string? statusAnterior,
            string statusNovo);

        ReservaDetalhesViewModel? ObterDetalhesReserva(int idReserva);

        void AtualizarStatusPagamento(int idReserva, string statusPagamento, int? idFuncionario);

        void AtualizarStatusReserva(int idReserva, string statusReserva, int? idFuncionario);
    }
}
