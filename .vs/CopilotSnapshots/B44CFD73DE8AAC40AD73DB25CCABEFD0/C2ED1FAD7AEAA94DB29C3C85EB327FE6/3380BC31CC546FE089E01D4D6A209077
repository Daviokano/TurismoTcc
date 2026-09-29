using HorizonTravel.Libraries.Filtro;
using HorizonTravel.Libraries.Login;
using HorizonTravel.Repository;
using HorizonTravel.Repository.Contract;
using Microsoft.AspNetCore.Mvc;

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

        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Login([FromForm] Models.Funcionario funcionario)
        {
            Models.Funcionario funcionarioDB = _funcionarioRepository.Login(funcionario.emailFun, funcionario.senhaFun);

            if(funcionarioDB.emailFun != null && funcionarioDB.senhaFun != null)
            {
                _loginFuncionario.Login(funcionarioDB);
                return new RedirectResult(Url.Action(nameof(PainelFuncionario)));
            }
            else
            {
                ViewData["MSG_E"] = "Funcioário não encontrado, por favor verifique email e senha digitados";
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

        [FuncionarioAutorizacaoAtributte]
        public IActionResult Index()
        {
            return View();
        }

        [FuncionarioAutorizacaoAtributte]
        public IActionResult Logout()
        {
            _loginFuncionario.Logout();
            return RedirectToAction("LoginFuncionario", "Home");
        }
    }
}
