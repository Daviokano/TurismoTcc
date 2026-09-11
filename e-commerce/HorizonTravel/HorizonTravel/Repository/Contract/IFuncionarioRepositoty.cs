using HorizonTravel.Models;

namespace HorizonTravel.Repository.Contract
{
    public interface IFuncionarioRepositoty
    {
        //Login do funcionário
        Funcionario Login(string Email, string Senha);

        //CRUD
        void Cadastrar(Funcionario funcionario);
        void Atualizar(Funcionario funcionario);
        void Excluir(int Id);
        Funcionario ObterFuncionario(int Id);
        IEnumerable<Funcionario> ObterTodosFuncionarios();
    }
}
