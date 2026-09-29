using HorizonTravel.Libraries.Filtro;
using HorizonTravel.Repository.Contract;
using Microsoft.AspNetCore.Mvc;

namespace HorizonTravel.Controllers
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
