using HorizonTravel.Models;

namespace HorizonTravel.Repository.Contract
{
    public interface IPacoteRepository
    {
        void Cadastrar(Pacote pacote);
        void Atualizar(Pacote pacote);
        Pacote ObterPacote(int id);
        IEnumerable<Pacote> ObterTodosPacotes();
        void AlterarSituacao(int id, string situacao);
    }
}
