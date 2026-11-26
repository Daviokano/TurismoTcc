using HorizonTravel.Libraries.Filtro;
using HorizonTravel.Models;
using HorizonTravel.Models.Constant;
using HorizonTravel.Repository.Contract;
using Microsoft.AspNetCore.Mvc;

namespace HorizonTravel.Areas.Funcionario.Controllers
{
    [Area("Funcionario")]
    [FuncionarioAutorizacao]
    public class PacoteController : Controller
    {
        private readonly IPacoteRepository _pacoteRepository;
        private readonly IFuncionarioRepositoty _funcionarioRepository;
        private readonly IParceiroRepository _parceiroRepository;
        private readonly IPromocaoRepository _promocaoRepository;

        public PacoteController(
            IPacoteRepository pacoteRepository,
            IFuncionarioRepositoty funcionarioRepository,
            IParceiroRepository parceiroRepository,
            IPromocaoRepository promocaoRepository)
        {
            _pacoteRepository = pacoteRepository;
            _funcionarioRepository = funcionarioRepository;
            _parceiroRepository = parceiroRepository;
            _promocaoRepository = promocaoRepository;
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
            ViewBag.Promocoes = _promocaoRepository.ObterPromocoesAtivas();
            return View(new Pacote { dataPac = DateTime.Today });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Cadastrar(Pacote pacote)
        {
            pacote.CNPJFor = RemoverCaracteresNaoNumericos(pacote.CNPJFor);
            pacote.situacaoPac = SituacaoPacoteConstant.Ativo;
            ModelState.Remove(nameof(pacote.CNPJFor));
            ValidarRelacionamentos(pacote);

            if (!ModelState.IsValid)
            {
                ViewBag.Promocoes = _promocaoRepository.ObterPromocoesAtivas();
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

            if (pacote == null)
            {
                return NotFound();
            }

            ViewBag.Promocoes = _promocaoRepository.ObterPromocoesAtivas();
            return View(pacote);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Atualizar(Pacote pacote)
        {
            pacote.CNPJFor = RemoverCaracteresNaoNumericos(pacote.CNPJFor);
            ModelState.Remove(nameof(pacote.CNPJFor));
            ValidarRelacionamentos(pacote);

            if (!ModelState.IsValid)
            {
                ViewBag.Promocoes = _promocaoRepository.ObterPromocoesAtivas();
                return View(pacote);
            }

            var pacoteAtual = _pacoteRepository.ObterPacote(pacote.IDPac);

            if (pacoteAtual == null)
            {
                return NotFound();
            }

            pacote.situacaoPac = pacoteAtual.situacaoPac;

            _pacoteRepository.Atualizar(pacote);
            TempData["MSG_S"] = "Pacote atualizado com sucesso!";
            return RedirectToAction(nameof(Index));
        }

        public IActionResult Detalhes(int id)
        {
            var pacote = _pacoteRepository.ObterPacote(id);

            return pacote == null ? NotFound() : View(pacote);
        }

        private void ValidarRelacionamentos(Pacote pacote)
        {
            if (pacote.CNPJFor.Length != 14)
            {
                ModelState.AddModelError(nameof(pacote.CNPJFor), "Informe um CNPJ válido.");
                return;
            }

            if (_funcionarioRepository.ObterFuncionario(pacote.IDFun).IDFun == 0)
            {
                ModelState.AddModelError(nameof(pacote.IDFun), "Informe um funcionário cadastrado.");
            }

            var parceiro = _parceiroRepository.ObterParceiro(pacote.CNPJFor);

            if (string.IsNullOrWhiteSpace(parceiro.CNPJFor))
            {
                ModelState.AddModelError(nameof(pacote.CNPJFor), "Informe um parceiro cadastrado.");
            }
            else if (parceiro.situacaoFor != SituacaoParceiroConstant.Ativo)
            {
                ModelState.AddModelError(nameof(pacote.CNPJFor), "O parceiro informado está inativo.");
            }
        }

        private static string RemoverCaracteresNaoNumericos(string? valor)
        {
            return new string((valor ?? string.Empty).Where(char.IsDigit).ToArray());
        }
    }
}
