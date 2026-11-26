using HorizonTravel.Libraries.Login;
using HorizonTravel.Models;
using HorizonTravel.Models.Constant;
using HorizonTravel.Repository.Contract;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace HorizonTravel.Libraries.Filtro
{
    public class UsuarioAutorizacaoAtributte : Attribute, IAuthorizationFilter
    {
        LoginUsuario _loginUsuario;
        IUsuarioRepository _usuarioRepository;

        public void OnAuthorization(AuthorizationFilterContext context)
        {
            _loginUsuario = (LoginUsuario)context.HttpContext.RequestServices.GetService(typeof(LoginUsuario));
            _usuarioRepository = (IUsuarioRepository)context.HttpContext.RequestServices.GetService(typeof(IUsuarioRepository));

            Usuario usuario = _loginUsuario.GetUsuario();

            // não está logado
            if (usuario == null)
            {
                context.Result = new RedirectToActionResult(
                    "Login",
                    "Home",
                    new { returnUrl = context.HttpContext.Request.Path });
                return;
            }

            // confere no banco (e não na sessão) para pegar desativações feitas depois do login
            Usuario usuarioAtual = _usuarioRepository.ObterUsuario(usuario.IDUsu);

            // ObterUsuario devolve objeto vazio (IDUsu = 0) quando não encontra
            if (usuarioAtual.IDUsu == 0 || usuarioAtual.situacaoUsu != SituacaoUsuConstant.Ativo)
            {
                _loginUsuario.Logout();
                context.Result = new RedirectToActionResult("Login", "Home", null);
            }
        }
    }
}