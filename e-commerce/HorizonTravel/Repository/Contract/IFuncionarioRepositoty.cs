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

        void Ativar(int Id);
        void Desativar(int Id);

        void AlterarSituacao(int id, string situacao);
        int ContarAdministradoresAtivos();
    }
}
