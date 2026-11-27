using HorizonTravel.Models;
using HorizonTravel.Models.Constant;
using HorizonTravel.Libraries.Promocao;
using HorizonTravel.Repository.Contract; // Ou o seu Contexto do banco
using Microsoft.AspNetCore.Mvc;
using System.Globalization;

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
        public IActionResult Index(
            string? origem,
            string? destino,
            string? data,
            string? pessoas,
            string? ordenacao)
        {
            DateTime? dataFiltro = ConverterData(data);
            string origemFiltro = origem?.Trim() ?? string.Empty;
            string destinoFiltro = destino?.Trim() ?? string.Empty;
            int? pessoasFiltro = ConverterQuantidadePessoas(pessoas);

            // Busca os pacotes reais do banco
            var pacotes = _pacoteRepository
                .ObterTodosPacotes()
                .Where(pacote => pacote.situacaoPac == SituacaoPacoteConstant.Ativo)
                .Where(pacote => pacote.quantidadePac > 0)
                .Where(pacote => string.IsNullOrWhiteSpace(origemFiltro)
                    || pacote.origemPac != null
                    && pacote.origemPac.Contains(origemFiltro, StringComparison.OrdinalIgnoreCase))
                .Where(pacote => string.IsNullOrWhiteSpace(destinoFiltro)
                    || pacote.destinoPac != null
                    && pacote.destinoPac.Contains(destinoFiltro, StringComparison.OrdinalIgnoreCase))
                .Where(pacote => !dataFiltro.HasValue || pacote.dataPac.Date == dataFiltro.Value.Date)
                .Where(pacote => !pessoasFiltro.HasValue
                    || pacote.quantidadePassageirosPac == pessoasFiltro.Value)
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

        private static DateTime? ConverterData(string? data)
        {
            if (string.IsNullOrWhiteSpace(data))
            {
                return null;
            }

            string[] formatos = { "dd/MM/yyyy", "yyyy-MM-dd" };

            return DateTime.TryParseExact(
                data,
                formatos,
                CultureInfo.InvariantCulture,
                DateTimeStyles.None,
                out DateTime dataConvertida)
                ? dataConvertida
                : null;
        }

        private static int? ConverterQuantidadePessoas(string? pessoas)
        {
            return int.TryParse(pessoas, out int quantidade) && quantidade > 0
                ? quantidade
                : null;
        }

        // Action de detalhes que você já deve ter
        public IActionResult Detalhes(int id)
        {
            var pacote = _pacoteRepository.ObterPacote(id);

            if (pacote == null || pacote.situacaoPac != SituacaoPacoteConstant.Ativo)
                return NotFound();

            ViewBag.Promocoes = _promocaoRepository.ObterPromocoesAtivas().ToList();

            return View(pacote);
        }
    }
}
