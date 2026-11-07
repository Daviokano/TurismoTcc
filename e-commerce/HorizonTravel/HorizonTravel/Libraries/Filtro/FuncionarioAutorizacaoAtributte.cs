using HorizonTravel.Libraries.Login;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace HorizonTravel.Libraries.Filtro
{
    public class FuncionarioAutorizacaoAtributte : Attribute, IAuthorizationFilter
    {
        public void OnAuthorization(AuthorizationFilterContext context)
        {
            var loginFuncionario = (LoginFuncionario)context.HttpContext.RequestServices.GetService(typeof(LoginFuncionario));

            if (loginFuncionario?.GetFuncionario() == null)
            {
                context.Result = new RedirectToActionResult(
                    "LoginFuncionario","Home", new { area = "Funcionario" });
            }
        }
    }
}
