using HorizonTravel.Libraries.Filtro;
using HorizonTravel.Repository;
using HorizonTravel.Repository.Contract;
using Microsoft.AspNetCore.Mvc;

namespace HorizonTravel.Areas.Funcionario.Controllers
{
    [Area("Funcionario")]
    public class ParceiroController : Controller
    {
        private IParceiroRepository _parceiroRepository;

        public ParceiroController(IParceiroRepository parceiroRepository)
        {
            _parceiroRepository = parceiroRepository;
        }

        public IActionResult Index()
        {
            return View(_parceiroRepository.ObterTodosParceiros());
        }

        [ValidateHttpReferer]
        public IActionResult Ativar(int Id)
        {
            _parceiroRepository.Ativar(Id);
            return RedirectToAction(nameof(Index));
        }

        [ValidateHttpReferer]
        public IActionResult Desativar(int Id)
        {
            _parceiroRepository.Desativar(Id);
            return RedirectToAction(nameof(Index));
        }
    }
}
