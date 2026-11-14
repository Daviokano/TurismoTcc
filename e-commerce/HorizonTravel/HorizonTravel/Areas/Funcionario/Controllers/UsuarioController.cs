using HorizonTravel.Libraries.Filtro;
using HorizonTravel.Models;
using HorizonTravel.Repository.Contract;
using Microsoft.AspNetCore.Mvc;

namespace HorizonTravel.Areas.Funcionario.Controllers
{
    [Area("Funcionario")]
    public class UsuarioController : Controller
    {
        private IUsuarioRepository _usuarioRepository;

        public UsuarioController(IUsuarioRepository usuarioRepository)
        {
            _usuarioRepository = usuarioRepository;
        }

        public IActionResult Index()
        {
            return View(_usuarioRepository.ObterTodosUsuarios());
        }

        [HttpGet]
        public IActionResult Cadastrar()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Cadastrar(Models.Usuario usuario)
        {
            usuario.situacaoUsu = "A";
            _usuarioRepository.Cadastrar(usuario);
            usuario.CPFUsu = new string(usuario.CPFUsu.Where(char.IsDigit).ToArray());

            TempData["MSG_S"] = "Cadastro realizado com sucesso!";
            return RedirectToAction(nameof(Index));
        }

        [ValidateHttpReferer]
        public IActionResult Ativar(int Id)
        {
            _usuarioRepository.Ativar(Id);
            return RedirectToAction(nameof(Index));
        }

        [ValidateHttpReferer]
        public IActionResult Desativar(int Id)
        {
            _usuarioRepository.Desativar(Id);
            return RedirectToAction(nameof(Index));
        }
    }
}
