using HorizonTravel.Libraries.Filtro;
using HorizonTravel.Models;
using HorizonTravel.Repository.Contract;
using Microsoft.AspNetCore.Mvc;

namespace HorizonTravel.Areas.Funcionario.Controllers
{
    [Area("Funcionario")]
    [FuncionarioAutorizacaoAtributte]
    public class PacoteController : Controller
    {
        private readonly IPacoteRepository _pacoteRepository;

        public PacoteController(IPacoteRepository pacoteRepository)
        {
            _pacoteRepository = pacoteRepository;
        }

        public IActionResult Index(string busca)
        {
            var pacotes = _pacoteRepository.ObterTodosPacotes();

            if (!string.IsNullOrWhiteSpace(busca))
            {
                pacotes = pacotes.Where(p =>
                    p.destinoPac.Contains(busca, StringComparison.OrdinalIgnoreCase) ||
                    p.descricaoPac.Contains(busca, StringComparison.OrdinalIgnoreCase));
            }

            ViewBag.Busca = busca;
            return View(pacotes);
        }

        [HttpGet]
        public IActionResult Cadastrar()
        {
            return View(new Pacote { dataPac = DateTime.Today });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Cadastrar(Pacote pacote)
        {
            if (!ModelState.IsValid)
            {
                return View(pacote);
            }

            _pacoteRepository.Cadastrar(pacote);
            TempData["MSG_S"] = "Pacote cadastrado com sucesso!";
            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public IActionResult Atualizar(int id)
        {
            var pacote = _pacoteRepository.ObterPacote(id);

            return pacote == null ? NotFound() : View(pacote);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Atualizar(Pacote pacote)
        {
            if (!ModelState.IsValid)
            {
                return View(pacote);
            }

            _pacoteRepository.Atualizar(pacote);
            TempData["MSG_S"] = "Pacote atualizado com sucesso!";
            return RedirectToAction(nameof(Index));
        }

        public IActionResult Detalhes(int id)
        {
            var pacote = _pacoteRepository.ObterPacote(id);

            return pacote == null ? NotFound() : View(pacote);
        }
    }
}
