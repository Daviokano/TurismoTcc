using HorizonTravel.Models;
using Newtonsoft.Json;

namespace HorizonTravel.Libraries.Login
{
    public class LoginUsuario
    {
        private string Key = "Login.Usuario";
        private Sessao.Sessao _sessao;

        public LoginUsuario(Sessao.Sessao sessao)
        {
            _sessao = sessao;
        }

        public void Login(Usuario usuario)
        {
            //serializar
            string usuarioJSONString = JsonConvert.SerializeObject(usuario);
            _sessao.Cadastrar(Key, usuarioJSONString);
        }

        public Usuario GetUsuario()
        {
            //deserializar
            if (_sessao.Existe(Key))
            {
                string clienteJSONString = _sessao.Consultar(Key);
                return JsonConvert.DeserializeObject<Usuario>(clienteJSONString);
            }
            else
            {
                return null;
            }
        }

        public void Logout()
        {
            _sessao.RemoverTodos();
        }
    }
}
