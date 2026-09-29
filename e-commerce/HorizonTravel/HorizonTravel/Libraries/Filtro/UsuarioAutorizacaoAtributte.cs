using HorizonTravel.Libraries.Login;
using HorizonTravel.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace HorizonTravel.Libraries.Filtro
{
    public class UsuarioAutorizacaoAtributte : Attribute, IAuthorizationFilter
    {
        LoginUsuario _loginUsuario;
        public void OnAuthorization(AuthorizationFilterContext context)
        {
            _loginUsuario = (LoginUsuario)context.HttpContext.RequestServices.GetService(typeof(LoginUsuario));
            Usuario usuario = _loginUsuario.GetUsuario();
            if (usuario == null)
            {
                context.Result = new ContentResult()
                {
                    Content = "Acesso negado."
                };
            }
        }
    }
}
