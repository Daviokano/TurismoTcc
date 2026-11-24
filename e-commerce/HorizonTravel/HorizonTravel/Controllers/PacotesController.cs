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
        public IActionResult Index(string? origem, string? destino, DateTime? data, int? pessoas, string? ordenacao)
        {
            // Busca os pacotes reais do banco
            var pacotes = _pacoteRepository
                .ObterTodosPacotes()
                .Where(pacote => pacote.quantidadePac > 0)
                .Where(pacote => string.IsNullOrWhiteSpace(origem)
                    || pacote.origemPac.Contains(origem, StringComparison.OrdinalIgnoreCase))
                .Where(pacote => string.IsNullOrWhiteSpace(destino)
                    || pacote.destinoPac.Contains(destino, StringComparison.OrdinalIgnoreCase))
                .Where(pacote => !data.HasValue || pacote.dataPac.Date == data.Value.Date)
                .Where(pacote => !pessoas.HasValue || pacote.quantidadePac >= pessoas.Value)
                .ToList();

            pacotes = ordenacao switch
            {
                "menor-preco" => pacotes.OrderBy(pacote => pacote.precoPac).ToList(),
                "maior-preco" => pacotes.OrderByDescending(pacote => pacote.precoPac).ToList(),
                _ => pacotes
            };

            ViewBag.Promocoes = _promocaoRepository.ObterPromocoesAtivas().ToList();

            return View(pacotes);
        }

        // Action de detalhes que você já deve ter
        public IActionResult Detalhes(int id)
        {
            var pacote = _pacoteRepository.ObterPacote(id);

            if (pacote == null)
                return NotFound();

            ViewBag.Promocoes = _promocaoRepository.ObterPromocoesAtivas().ToList();

            return View(pacote);
        }
    }
}
