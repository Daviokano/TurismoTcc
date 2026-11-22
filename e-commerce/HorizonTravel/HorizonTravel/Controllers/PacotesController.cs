using HorizonTravel.Models;
using HorizonTravel.Libraries.Promocao;
using HorizonTravel.Repository.Contract; // Ou o seu Contexto do banco
using Microsoft.AspNetCore.Mvc;

namespace HorizonTravel.Controllers
{
    public class PacotesController : Controller
    {
        // Se você usa Repository:
        private readonly IPacoteRepository _pacoteRepository;
        private readonly IPromocaoRepository _promocaoRepository;

        public PacotesController(IPacoteRepository pacoteRepository, IPromocaoRepository promocaoRepository)
        {
            _pacoteRepository = pacoteRepository;
            _promocaoRepository = promocaoRepository;
        }

        // A URL acessada será: localhost:porta/Pacotes
        public IActionResult Index()
        {
            // Busca os pacotes reais do banco
            var pacotes = _pacoteRepository
                .ObterTodosPacotes()
                .Where(pacote => pacote.quantidadePac > 0)
                .ToList();

            ViewBag.Promocoes = _promocaoRepository.ObterPromocoesAtivas()
                .ToDictionary(promocao => promocao.IDPromo);

            return View(pacotes);
        }

        // Action de detalhes que você já deve ter
        public IActionResult Detalhes(int id)
        {
            var pacote = _pacoteRepository.ObterPacote(id);

            if (pacote == null)
                return NotFound();

            ViewBag.Promocao = pacote.IDPromo.HasValue
                ? _promocaoRepository.ObterPromocao(pacote.IDPromo.Value)
                : null;

            return View(pacote);
        }
    }
}
