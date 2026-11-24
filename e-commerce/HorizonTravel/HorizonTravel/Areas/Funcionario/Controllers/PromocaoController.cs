using HorizonTravel.Libraries.Filtro;
using HorizonTravel.Models;
using HorizonTravel.Repository.Contract;
using Microsoft.AspNetCore.Mvc;

namespace HorizonTravel.Areas.Funcionario.Controllers
{
    [Area("Funcionario")]
    [FuncionarioAutorizacao]
    public class PromocaoController : Controller
    {
        private readonly IPromocaoRepository _promocaoRepository;

        public PromocaoController(IPromocaoRepository promocaoRepository)
        {
            _promocaoRepository = promocaoRepository;
        }

        public IActionResult Index()
        {
            return View(_promocaoRepository.ObterTodasPromocoes());
        }

        [HttpGet]
        public IActionResult Cadastrar()
        {
            return View(new Promocao { dataValidade = DateTime.Today });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Cadastrar(Promocao promocao)
        {
            ValidarData(promocao);

            if (!ModelState.IsValid)
            {
                return View(promocao);
            }

            _promocaoRepository.Cadastrar(promocao);
            TempData["MSG_S"] = "Promoção cadastrada com sucesso!";
            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public IActionResult Atualizar(int id)
        {
            var promocao = _promocaoRepository.ObterPromocao(id);
            return promocao == null ? NotFound() : View(promocao);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Atualizar(Promocao promocao)
        {
            ValidarData(promocao);

            if (!ModelState.IsValid)
            {
                return View(promocao);
            }

            if (_promocaoRepository.ObterPromocao(promocao.IDPromo) == null)
            {
                return NotFound();
            }

            _promocaoRepository.Atualizar(promocao);
            TempData["MSG_S"] = "Promoção atualizada com sucesso!";
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Remover(int id)
        {
            if (_promocaoRepository.ObterPromocao(id) == null)
            {
                return NotFound();
            }

            _promocaoRepository.Remover(id);
            TempData["MSG_S"] = "Promoção removida com sucesso!";
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult AlterarSituacao(int id)
        {
            var promocao = _promocaoRepository.ObterPromocao(id);

            if (promocao == null)
            {
                return NotFound();
            }

            string novaSituacao = promocao.situacaoPromocao == "A" ? "I" : "A";
            _promocaoRepository.AlterarSituacao(id, novaSituacao);
            TempData["MSG_S"] = novaSituacao == "A"
                ? "Promoção ativada com sucesso!"
                : "Promoção desativada com sucesso!";

            return RedirectToAction(nameof(Index));
        }

        private void ValidarData(Promocao promocao)
        {
            if (promocao.dataValidade.Date < DateTime.Today)
            {
                ModelState.AddModelError(
                    nameof(promocao.dataValidade),
                    "A data limite não pode ser anterior à data de hoje.");
            }
        }
    }
}
