using HorizonTravel.Models;

namespace HorizonTravel.Repository.Contract
{
    public interface IParceiroRepository
    {
        void Cadastrar(Parceiro parceiro);
        void Atualizar(Parceiro parceiro);
        void Excluir(string cnpj);
        Parceiro ObterParceiro(string cnpj);
        IEnumerable<Parceiro> ObterTodosParceiros();

        void Ativar(string cnpj);
        void Desativar(string cnpj);
    }
}
