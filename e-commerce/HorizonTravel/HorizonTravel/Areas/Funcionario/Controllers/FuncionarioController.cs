using HorizonTravel.Libraries.Filtro;
using HorizonTravel.Libraries.Login;
using HorizonTravel.Models.Constant;
using HorizonTravel.Repository.Contract;
using Microsoft.AspNetCore.Mvc;
using MySql.Data.MySqlClient;
using System; // Necessário para StringComparison
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

        // --- PROTEÇÃO: Apenas Administrador pode acessar a tela e cadastrar ---
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
            _funcionarioRepository.Cadastrar(funcionario);

            TempData["MSG_S"] = "Cadastro realizado com sucesso!";
            return RedirectToAction(nameof(Index));
        }

        // --- PROTEÇÃO: Apenas Administrador pode acessar a tela de atualizar ---
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
            if (ModelState.IsValid)
            {
                _funcionarioRepository.Atualizar(funcionario);
                TempData["MSG_S"] = "Cadastro atualizado com sucesso!";
                return RedirectToAction(nameof(Index));
            }

            return View(funcionario);
        }

        // O Excluir já estava protegido corretamente no seu código
        [HttpPost]
        [ValidateAntiForgeryToken]
        [FuncionarioAutorizacao(PermissaoFunConstant.Administrador)]
        public IActionResult Excluir(int Id)
        {
            var logado = _loginFuncionario.GetFuncionario();
            var alvo = _funcionarioRepository.ObterFuncionario(Id);

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