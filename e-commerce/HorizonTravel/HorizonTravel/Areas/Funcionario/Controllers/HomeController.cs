using HorizonTravel.Libraries.Filtro;
using HorizonTravel.Libraries.Login;
using HorizonTravel.Repository;
using HorizonTravel.Repository.Contract;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Metadata.Internal;

namespace HorizonTravel.Areas.Funcionario.Controllers
{
    [Area("Funcionario")]
    public class HomeController : Controller
    {
        private IFuncionarioRepositoty _funcionarioRepository;
        private LoginFuncionario _loginFuncionario;

        public HomeController(IFuncionarioRepositoty funcionarioRepositoty, LoginFuncionario loginFuncionario)
        {
            _funcionarioRepository = funcionarioRepositoty;
            _loginFuncionario = loginFuncionario;
        }

        [HttpGet]
        public IActionResult LoginFuncionario()
        {
            return View();
        }

        [HttpPost]
        public IActionResult LoginFuncionario([FromForm] Models.Funcionario funcionario)
        {
            Models.Funcionario funcionarioDB = _funcionarioRepository.Login(funcionario.emailFun, funcionario.senhaFun);

            if (funcionarioDB.emailFun != null && funcionarioDB.senhaFun != null)
            {
                _loginFuncionario.Login(funcionarioDB);
                return RedirectToAction(nameof(PainelFuncionario));
            }
            else
            {
                ViewData["MSG_E"] = "Funcionário não encontrado, por favor verifique email e senha digitados";
                return View();
            }
        }

        

        public IActionResult PainelFuncionario()
        {
            ViewBag.nomeFun = _loginFuncionario.GetFuncionario().nomeFun;
            ViewBag.emailFun = _loginFuncionario.GetFuncionario().emailFun;
            ViewBag.CPFFun = _loginFuncionario.GetFuncionario().CPFFun;
            return View();
        }

        [FuncionarioAutorizacaoAtributte]
        public IActionResult Painel()
        {
            return View();
        }

        //verificar se deve ser criada outra index que mostre os dados dos funcionarios, como atualização
        [FuncionarioAutorizacaoAtributte]
        public IActionResult Index()
        {
            return View();
        }

        [FuncionarioAutorizacaoAtributte]
        public IActionResult Logout()
        {
            _loginFuncionario.Logout();
            // Redireciona para a action Login desta controller/área
            return RedirectToAction(nameof(LoginFuncionario));
        }

    }
}
