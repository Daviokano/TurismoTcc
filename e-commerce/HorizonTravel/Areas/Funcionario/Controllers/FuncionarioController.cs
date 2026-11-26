using HorizonTravel.Libraries.Filtro;
using HorizonTravel.Libraries.Login;
using HorizonTravel.Models.Constant;
using HorizonTravel.Repository.Contract;
using Microsoft.AspNetCore.Mvc;
using MySql.Data.MySqlClient;
using System; 
using System.Linq;

namespace HorizonTravel.Areas.Funcionario.Controllers
{
    [Area("Funcionario")]
    [FuncionarioAutorizacao]
    public class FuncionarioController : Controller
    {
        private IFuncionarioRepositoty _funcionarioRepository;
        private LoginFuncionario _loginFuncionario;

        public FuncionarioController(IFuncionarioRepositoty funcionarioRepository, LoginFuncionario loginFuncionario)
        {
            _funcionarioRepository = funcionarioRepository;
            _loginFuncionario = loginFuncionario;
        }

        public IActionResult Index(string busca)
        {
            var funcionarios = _funcionarioRepository.ObterTodosFuncionarios();

            if (!string.IsNullOrWhiteSpace(busca))
            {
                funcionarios = funcionarios.Where(f =>
                    (f.nomeFun != null && f.nomeFun.Contains(busca, StringComparison.OrdinalIgnoreCase)) ||
                    (f.CPFFun != null && f.CPFFun.Contains(busca)));
            }

            return View(funcionarios);
        }

        // restrição para admin
        [HttpGet]
        [FuncionarioAutorizacao(PermissaoFunConstant.Administrador)]
        public IActionResult Cadastrar()
        {
            return View();
        }

        [HttpPost]
        [FuncionarioAutorizacao(PermissaoFunConstant.Administrador)]
        public IActionResult Cadastrar(Models.Funcionario funcionario)
        {
            funcionario.CPFFun = new string(funcionario.CPFFun.Where(char.IsDigit).ToArray());
            funcionario.situacaoFun = "A"; // novo funcionário já nasce ativo
            _funcionarioRepository.Cadastrar(funcionario);

            TempData["MSG_S"] = "Cadastro realizado com sucesso!";
            return RedirectToAction(nameof(Index));
        }


        // restriçaõ para dmin
        [HttpGet]
        [FuncionarioAutorizacao(PermissaoFunConstant.Administrador)]
        public IActionResult Atualizar(int Id)
        {
            Models.Funcionario funcionario = _funcionarioRepository.ObterFuncionario(Id);
            return View(funcionario);
        }

        [HttpPost]
        [FuncionarioAutorizacao(PermissaoFunConstant.Administrador)]
        public IActionResult Atualizar([FromForm] Models.Funcionario funcionario)
        {
            var funcionarioBanco = _funcionarioRepository.ObterFuncionario(funcionario.IDFun);

            if (funcionarioBanco.IDFun == 0)
            {
                TempData["MSG_E"] = "Funcionário não encontrado.";
                return RedirectToAction(nameof(Index));
            }

            // se a senha continuar vazia, mantém a do banco
            if (string.IsNullOrWhiteSpace(funcionario.senhaFun))
            {
                funcionario.senhaFun = funcionarioBanco.senhaFun;
            }

            // se o cargo vier vazio, mantém o do banco
            if (string.IsNullOrWhiteSpace(funcionario.permissaoFun))
            {
                funcionario.permissaoFun = funcionarioBanco.permissaoFun;
            }
            else if (funcionario.permissaoFun != PermissaoFunConstant.Administrador &&
                     funcionario.permissaoFun != PermissaoFunConstant.Funcionario)
            {
                TempData["MSG_E"] = "Selecione um cargo válido.";
                return RedirectToAction(nameof(Index));
            }

            // impede rebaixar o único administrador ativo
            if (funcionarioBanco.permissaoFun == PermissaoFunConstant.Administrador
                && funcionario.permissaoFun != PermissaoFunConstant.Administrador
                && funcionarioBanco.situacaoFun == SituacaoFunConstant.Ativo
                && _funcionarioRepository.ContarAdministradoresAtivos() <= 1)
            {
                TempData["MSG_E"] = "Não é possível rebaixar o único administrador ativo.";
                return RedirectToAction(nameof(Index));
            }

            // senha e cargo são opcionais na tela
            ModelState.Remove(nameof(funcionario.senhaFun));
            ModelState.Remove(nameof(funcionario.permissaoFun));

            if (ModelState.IsValid)
            {
                _funcionarioRepository.Atualizar(funcionario);
                TempData["MSG_S"] = "Cadastro atualizado com sucesso!";
                return RedirectToAction(nameof(Index));
            }

            return View(funcionario);
        }

        // algumas restrições para admin
        [HttpPost]
        [ValidateAntiForgeryToken]
        [FuncionarioAutorizacao(PermissaoFunConstant.Administrador)]
        public IActionResult Desativar(int Id)
        {
            var logado = _loginFuncionario.GetFuncionario();
            var alvo = _funcionarioRepository.ObterFuncionario(Id);

            if (alvo == null)
            {
                TempData["MSG_E"] = "Funcionário não encontrado.";
                return RedirectToAction(nameof(Index));
            }

            // impede ficar sem nenhum administrador ativo
            if (alvo.permissaoFun == PermissaoFunConstant.Administrador && alvo.situacaoFun == "A"
                && _funcionarioRepository.ContarAdministradoresAtivos() <= 1)
            {
                TempData["MSG_E"] = "Não é possível desativar o único administrador ativo.";
                return RedirectToAction(nameof(Index));
            }

            _funcionarioRepository.AlterarSituacao(Id, "D");

            // se o admin desativou a si mesmo, encerra a sessão dele
            if (Id == logado.IDFun)
            {
                _loginFuncionario.Logout();
                TempData["MSG_E"] = "Sua conta foi desativada.";
                return RedirectToAction("LoginFuncionario", "Home", new { area = "Funcionario" });
            }

            TempData["MSG_S"] = "Funcionário desativado com sucesso!";
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [FuncionarioAutorizacao(PermissaoFunConstant.Administrador)]
        public IActionResult Ativar(int Id)
        {
            _funcionarioRepository.AlterarSituacao(Id, "A");
            TempData["MSG_S"] = "Funcionário ativado com sucesso!";
            return RedirectToAction(nameof(Index));
        }


        // excluir definitivamente (só funcionário comum e nunca a própria conta)
        [HttpPost]
        [ValidateAntiForgeryToken]
        [FuncionarioAutorizacao(PermissaoFunConstant.Administrador)]
        public IActionResult Excluir(int Id)
        {
            var logado = _loginFuncionario.GetFuncionario();
            var alvo = _funcionarioRepository.ObterFuncionario(Id);

            if (alvo.IDFun == 0)
            {
                TempData["MSG_E"] = "Funcionário não encontrado.";
                return RedirectToAction(nameof(Index));
            }

            if (Id == logado.IDFun)
            {
                TempData["MSG_E"] = "Você não pode excluir a própria conta.";
                return RedirectToAction(nameof(Index));
            }

            if (alvo.permissaoFun == PermissaoFunConstant.Administrador)
            {
                TempData["MSG_E"] = "Não é possível excluir o administrador.";
                return RedirectToAction(nameof(Index));
            }

            try
            {
                _funcionarioRepository.Excluir(Id);
                TempData["MSG_S"] = "Cadastro excluído com sucesso!";
            }
            catch (MySqlException)
            {
                TempData["MSG_E"] = "Não foi possível excluir: este funcionário possui pacotes vinculados.";
            }

            return RedirectToAction(nameof(Index));
        }
    }
}