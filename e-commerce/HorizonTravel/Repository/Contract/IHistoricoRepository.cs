using HorizonTravel.Models;

namespace HorizonTravel.Repository.Contract
{
    public interface IHistoricoRepository
    {
        IEnumerable<Historico> BuscarHistoricoPorCliente(int idUsuario);

        IEnumerable<Historico> ObterHistorico(int idReserva);

        void RegistrarAlteracao(
            int idReserva,
            int? idFuncionario,
            string? statusAnterior,
            string statusNovo);
    }
}
