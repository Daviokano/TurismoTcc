using System.Diagnostics;
using HorizonTravel.Libraries.Filtro;
using HorizonTravel.Libraries.Login;
using HorizonTravel.Models;
using HorizonTravel.Models.Constant;
using HorizonTravel.Repository.Contract;
using Microsoft.AspNetCore.Mvc;


namespace HorizonTravel.Controllers
{
    public class HomeController : Controller
    {
        //injeção de dependencia
        private IUsuarioRepository _usuarioRepository;
        private LoginUsuario _usuarioLogin;
        private IReservaRepository _reservaRepository;

        public HomeController(
            IUsuarioRepository usuarioRepository,
            LoginUsuario loginUsuario,
            IReservaRepository reservaRepository)
        {
            _usuarioRepository = usuarioRepository;
            _usuarioLogin = loginUsuario;
            _reservaRepository = reservaRepository;
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

        [UsuarioAutorizacaoAtributte]
        public IActionResult PainelUsuario()
        {
            Usuario usuario = _usuarioLogin.GetUsuario();

            var modelo = new MinhasReservasViewModel
            {
                Usuario = usuario,
                Reservas = _reservaRepository.ObterReservasPorUsuario(usuario.IDUsu)
            };

            return View("Historico", modelo);
        }
        [UsuarioAutorizacaoAtributte]
        public IActionResult MeuPerfil()
        {
            return View(_usuarioLogin.GetUsuario());
        }
        [HttpPost]
        [UsuarioAutorizacaoAtributte]
        public IActionResult MeuPerfil([FromForm] Usuario usuario)
        {
            Usuario usuarioLogado = _usuarioLogin.GetUsuario();

            usuario.IDUsu = usuarioLogado.IDUsu;
            usuario.senhaUsu = usuarioLogado.senhaUsu;
            usuario.situacaoUsu = usuarioLogado.situacaoUsu;

            if (ModelState.IsValid)
            {
                _usuarioRepository.Atualizar(usuario);
                _usuarioLogin.Login(usuario);

                TempData["MSG_S"] = "Informações atualizadas com sucesso!";
                return RedirectToAction(nameof(MeuPerfil));
            }

            return View(usuario);
        }

        [UsuarioAutorizacaoAtributte]
        public IActionResult MinhasReservas()
        {
            Usuario usuario = _usuarioLogin.GetUsuario();

            var modelo = new MinhasReservasViewModel
            {
                Usuario = usuario,
                Reservas = _reservaRepository.ObterReservasPorUsuario(usuario.IDUsu)
            };

            return View(modelo);
        }

        [UsuarioAutorizacaoAtributte]
        public IActionResult LogoutUsuario()
        {
            _usuarioLogin.Logout();
            return RedirectToAction(nameof(Index));
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        public IActionResult Cadastrar()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Cadastrar([FromForm] Usuario usuario)
        {
            usuario.situacaoUsu = SituacaoUsuConstant.Ativo;
            var CPFExistente = _usuarioRepository.BuscaCpfUsuario(usuario.CPFUsu).CPFUsu;
            var EmailExistente = _usuarioRepository.BuscaEmailUsuario(usuario.emailUsu).emailUsu;

            if (!string.IsNullOrWhiteSpace(CPFExistente))
            {
                //cpf cadastrardo
                ViewData["MSG_E"] = "Este CPF j� est� cadastrado!!";
                return View();
            }
            else if (!string.IsNullOrWhiteSpace(EmailExistente))
            {
                //email cadastrardo
                ViewData["MSG_E"] = "Este email j� est� cadastrado!!";
                return View();
            }
            else if(ModelState.IsValid)
            {
                _usuarioRepository.Cadastrar(usuario);
                return RedirectToAction(nameof(Login));
            }
            return View();
        }
    }
}
