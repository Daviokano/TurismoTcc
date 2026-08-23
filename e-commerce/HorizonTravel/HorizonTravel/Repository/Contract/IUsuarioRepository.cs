using HorizonTravel.Models;

namespace HorizonTravel.Repository.Contract
{
    public interface IUsuarioRepository
    {
        //Login do usuário
        Usuario Login(string Email, string Senha);

        //CRUD
        void Cadastrar(Usuario usuario);
        void Atualizar(Usuario usuario);
        void Excluir(Usuario usuario);
        Usuario ObterUsuario(int Id);
        IEnumerable<Usuario> ObterTodosUsuarios();
    }
}
