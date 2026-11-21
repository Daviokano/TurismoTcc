using HorizonTravel.Models;
using HorizonTravel.Repository.Contract; // Ou o seu Contexto do banco
using Microsoft.AspNetCore.Mvc;

namespace HorizonTravel.Controllers
{
    public class PacotesController : Controller
    {
        // Se você usa Repository:
        private readonly IPacoteRepository _pacoteRepository;

        public PacotesController(IPacoteRepository pacoteRepository)
        {
            _pacoteRepository = pacoteRepository;
        }

        // A URL acessada será: localhost:porta/Pacotes
        public IActionResult Index()
        {
            // Busca os pacotes reais do banco
            var pacotes = _pacoteRepository
                .ObterTodosPacotes()
                .Where(pacote => pacote.quantidadePac > 0)
                .ToList();

            // Retorna a View "Index.cshtml" que está em Views/Pacotes/
            return View(pacotes);
        }

        // Action de detalhes que você já deve ter
        public IActionResult Detalhes(int id)
        {
            var pacote = _pacoteRepository.ObterPacote(id);

            if (pacote == null)
                return NotFound();

            return View(pacote);
        }
    }
}
