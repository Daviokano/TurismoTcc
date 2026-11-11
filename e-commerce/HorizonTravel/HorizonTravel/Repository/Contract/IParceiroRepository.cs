using HorizonTravel.Models;

namespace HorizonTravel.Repository.Contract
{
    public interface IParceiroRepository
    {
        void Cadastrar(Parceiro parceiro);
        void Atualizar(Parceiro parceiro);
        void Excluir(int Id);
        Parceiro ObterParceiro(int Id);
        IEnumerable<Parceiro> ObterTodosParceiros();
    }
}
