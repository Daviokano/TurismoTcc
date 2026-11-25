using HorizonTravel.Models;

namespace HorizonTravel.Repository.Contract
{
    public interface IPagamentoRepository
    {
        Pagamento? ObterPagamentoPorReserva(int idReserva);
    }
}
