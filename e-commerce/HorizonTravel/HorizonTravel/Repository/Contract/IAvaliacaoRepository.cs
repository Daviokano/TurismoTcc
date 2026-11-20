using HorizonTravel.Models;

namespace HorizonTravel.Repository.Contract
{
    public interface IAvaliacaoRepository
    {
        IEnumerable<Avaliacao> ObterTodasAvaliacoes();
        IEnumerable<Avaliacao> ListarPorPacote(int idPac);
        IEnumerable<Avaliacao> ListarPorUsuario(int idUsu);
        Avaliacao? ObterAvaliacao(int idAvaliacao);
        void Cadastrar(Avaliacao avaliacao);
        void Atualizar(Avaliacao avaliacao);
        void Excluir(int idAvaliacao);
    }
}
