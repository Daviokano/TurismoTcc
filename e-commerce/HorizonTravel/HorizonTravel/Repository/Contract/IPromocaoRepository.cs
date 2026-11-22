using HorizonTravel.Models;

namespace HorizonTravel.Repository.Contract
{
    public interface IPromocaoRepository
    {
        void Cadastrar(Promocao promocao);
        void Atualizar(Promocao promocao);
        void Remover(int id);
        Promocao? ObterPromocao(int id);
        IEnumerable<Promocao> ObterTodasPromocoes();
        IEnumerable<Promocao> ObterPromocoesAtivas();
    }
}
