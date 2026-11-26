using HorizonTravel.Libraries.Filtro;
using HorizonTravel.Repository;
using HorizonTravel.Repository.Contract;
using HorizonTravel.Models.Constant;
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
            parceiro.situacaoFor = SituacaoParceiroConstant.Ativo;
            parceiro.CNPJFor = new string(parceiro.CNPJFor.Where(char.IsDigit).ToArray());
            _parceiroRepository.Cadastrar(parceiro);
            TempData["MSG_S"] = "Cadastro realizado com sucesso!";
            return RedirectToAction(nameof(Index));
        }


        [HttpGet]
        public IActionResult Atualizar(string id)
        {
            Models.Parceiro parceiro = _parceiroRepository.ObterParceiro(id);

            if (parceiro != null && !string.IsNullOrWhiteSpace(parceiro.CNPJFor))
            {
                parceiro.CNPJFor = FormatarCnpj(parceiro.CNPJFor);
            }

            return View(parceiro);
        }

        [HttpPost]
        public IActionResult Atualizar([FromForm] Models.Parceiro parceiro, [FromForm] string cnpjOriginal)
        {
            cnpjOriginal = new string((cnpjOriginal ?? string.Empty)
                .Where(char.IsDigit)
                .ToArray());
            parceiro.CNPJFor = new string((parceiro.CNPJFor ?? string.Empty)
                .Where(char.IsDigit)
                .ToArray());

            // O model binder valida antes de entrar nesta action. Como o CNPJ
            // pode chegar mascarado ou sem máscara, valida-se o valor normalizado.
            ModelState.Remove(nameof(Models.Parceiro.CNPJFor));

            if (ModelState.IsValid)
            {
                _parceiroRepository.Atualizar(parceiro, cnpjOriginal);
                TempData["MSG_S"] = "Cadastro atualizado com sucesso!";
                return RedirectToAction(nameof(Index));
            }

            return View(parceiro);
        }

        private static string FormatarCnpj(string cnpj)
        {
            string numeros = new string(cnpj.Where(char.IsDigit).ToArray());

            return numeros.Length == 14
                ? Convert.ToUInt64(numeros).ToString(@"00\.000\.000\/0000-00")
                : cnpj;
        }

        [ValidateHttpReferer]
        public IActionResult Ativar(string id)
        {
            _parceiroRepository.Ativar(id);
            return RedirectToAction(nameof(Index));
        }

        [ValidateHttpReferer]
        public IActionResult Desativar(string id)
        {
            _parceiroRepository.Desativar(id);
            return RedirectToAction(nameof(Index));
        }
    }
}
