using HorizonTravel.Libraries.Login;
using HorizonTravel.Models.Constant;
using HorizonTravel.Repository.Contract;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace HorizonTravel.Libraries.Filtro
{
    public class FuncionarioAutorizacaoAttribute : Attribute, IAuthorizationFilter
    {
        private readonly string _permissaoExigida;

        // Sem parâmetro: basta estar logado e ativo. Com parâmetro: precisa ter aquela permissão.
        public FuncionarioAutorizacaoAttribute(string permissaoExigida = null)
        {
            _permissaoExigida = permissaoExigida;
        }

        public void OnAuthorization(AuthorizationFilterContext context)
        {
            var _loginFuncionario = (LoginFuncionario)context.HttpContext.RequestServices.GetService(typeof(LoginFuncionario));
            var _funcionarioRepository = (IFuncionarioRepositoty)context.HttpContext.RequestServices.GetService(typeof(IFuncionarioRepositoty));

            var funcionario = _loginFuncionario?.GetFuncionario();

            // não está logado
            if (funcionario == null)
            {
                context.Result = new RedirectToActionResult(
                    "LoginFuncionario", "Home", new { area = "Funcionario" });
                return;
            }

            // confere no banco (e não na sessão) para pegar desativações feitas por outro admin
            var funcionarioAtual = _funcionarioRepository.ObterFuncionario(funcionario.IDFun);

            // ObterFuncionario devolve objeto vazio (IDFun = 0) quando não encontra
            if (funcionarioAtual.IDFun == 0 || funcionarioAtual.situacaoFun != SituacaoFunConstant.Ativo)
            {
                _loginFuncionario.Logout();
                context.Result = new RedirectToActionResult(
                    "LoginFuncionario", "Home", new { area = "Funcionario" });
                return;
            }

            if (_permissaoExigida == PermissaoFunConstant.Administrador
                && funcionarioAtual.permissaoFun != PermissaoFunConstant.Administrador)
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