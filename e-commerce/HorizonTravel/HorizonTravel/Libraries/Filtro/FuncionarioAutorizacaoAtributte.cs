using HorizonTravel.Libraries.Login;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace HorizonTravel.Libraries.Filtro
{
    public class FuncionarioAutorizacaoAtributte : Attribute, IAuthorizationFilter
    {
        LoginFuncionario _loginFuncionario;
        public void OnAuthorization(AuthorizationFilterContext context)
        {
            _loginFuncionario = (LoginFuncionario)context.HttpContext.RequestServices.GetService(typeof(LoginFuncionario));
            if (_loginFuncionario.GetFuncionario() == null)
            {
                context.Result = new RedirectToActionResult("Index", "Home", null);
            }
        }
    }
}
