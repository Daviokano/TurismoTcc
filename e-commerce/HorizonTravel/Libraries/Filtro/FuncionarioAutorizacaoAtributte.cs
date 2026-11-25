using HorizonTravel.Libraries.Login;
using HorizonTravel.Models.Constant;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace HorizonTravel.Libraries.Filtro
{
    public class FuncionarioAutorizacaoAttribute : Attribute, IAuthorizationFilter
    {
        private readonly string _permissaoExigida;

        // Sem parâmetro: basta estar logado. Com parâmetro: precisa ter aquela permissão.
        public FuncionarioAutorizacaoAttribute(string permissaoExigida = null)
        {
            _permissaoExigida = permissaoExigida;
        }

        public void OnAuthorization(AuthorizationFilterContext context)
        {
            var loginFuncionario = (LoginFuncionario)context.HttpContext.RequestServices.GetService(typeof(LoginFuncionario));
            var funcionario = loginFuncionario?.GetFuncionario();

            if (funcionario == null)
            {
                context.Result = new RedirectToActionResult(
                    "LoginFuncionario", "Home", new { area = "Funcionario" });
                return;
            }

            if (_permissaoExigida == PermissaoFunConstant.Administrador
                && funcionario.permissaoFun != PermissaoFunConstant.Administrador)
            {
                context.Result = new ContentResult
                {
                    StatusCode = 403,
                    Content = "Acesso negado: apenas o administrador pode fazer isso."
                };
            }
        }
    }
}