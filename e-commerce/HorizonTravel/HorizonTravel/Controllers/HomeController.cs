using System.Diagnostics;
using HorizonTravel.Libraries.Login;
using HorizonTravel.Models;
using HorizonTravel.Repository.Contract;
using Microsoft.AspNetCore.Mvc;


namespace HorizonTravel.Controllers
{
    public class HomeController : Controller
    {
        //injeção de dependência
        private IUsuarioRepository _usuarioRepository;
        private LoginUsuario _usuarioLogin;

        public HomeController(IUsuarioRepository usuarioRepository, LoginUsuario loginUsuario)
        {
            _usuarioRepository = usuarioRepository;
            _usuarioLogin = loginUsuario;
        }

        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Login([FromForm] Usuario usuario)
        {
            Usuario usuarioDB = _usuarioRepository.Login(usuario.emailUsu, usuario.senhaUsu);

            if (usuarioDB.emailUsu != null && usuarioDB.senhaUsu != null)
            {
                _usuarioLogin.Login(usuarioDB);
                return new RedirectResult(Url.Action(nameof(PainelUsuario)));
            }
            else
            {
                ViewData["MSG_E"] = "O usuário não foi localizado, por fazer verifique o email e senha digitados!";
                return View();
            }
        }

        public IActionResult PainelUsuario()
        {
            ViewBag.nomeUsu = _usuarioLogin.GetUsuario().nomeUsu;
            ViewBag.CPFUsu = _usuarioLogin.GetUsuario().CPFUsu;
            ViewBag.emailUsu = _usuarioLogin.GetUsuario().emailUsu;
            return View();
        }

        public IActionResult LogoutUsuario()
        {
            _usuarioLogin.Logout();
            return RedirectToAction(nameof(PainelUsuario));
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }
    }
}
