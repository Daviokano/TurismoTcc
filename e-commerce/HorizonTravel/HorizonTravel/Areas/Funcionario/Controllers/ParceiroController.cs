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

        [HttpGet]
        public IActionResult Cadastrar()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Cadastrar(Models.Parceiro parceiro)
        {
            parceiro.situacaoFor = "A";
            parceiro.CNPJFor = new string(parceiro.CNPJFor.Where(char.IsDigit).ToArray());
            _parceiroRepository.Cadastrar(parceiro);
            TempData["MSG_S"] = "Cadastro realizado com sucesso!";
            return RedirectToAction(nameof(Index));
        }


        [HttpGet]
        public IActionResult Atualizar(int Id)
        {
            Models.Parceiro parceiro = _parceiroRepository.ObterParceiro(Id);
            return View(parceiro);
        }

        [HttpPost]
        public IActionResult Atualizar([FromForm] Models.Parceiro parceiro)
        {
            parceiro.CNPJFor = new string(parceiro.CNPJFor.Where(char.IsDigit).ToArray());

            if (ModelState.IsValid)
            {
                _parceiroRepository.Atualizar(parceiro);
                TempData["MSG_S"] = "Cadastro atualizado com sucesso!";
                return RedirectToAction(nameof(Index));
            }

            return View(parceiro);
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
