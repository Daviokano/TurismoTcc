using HorizonTravel.Repository.Contract;
using Microsoft.AspNetCore.Mvc;

namespace HorizonTravel.Areas.Funcionario.Controllers
{
    [Area("Funcionario")]
    public class FuncionarioController : Controller
    {
        private IFuncionarioRepositoty _funcionarioRepository;
        public FuncionarioController(IFuncionarioRepositoty funcionarioRepository)
        {
            _funcionarioRepository = funcionarioRepository;
        }
        public IActionResult Index()
        {
            return View(_funcionarioRepository.ObterTodosFuncionarios());
        }

        [HttpGet]
        public IActionResult Cadastrar()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Cadastrar(Models.Funcionario funcionario)
        {
            _funcionarioRepository.Cadastrar(funcionario);
            TempData["MSG_S"] = "Cadastro realizado com sucesso!";
            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public IActionResult Atualizar(int Id)
        {
            Models.Funcionario funcionario = _funcionarioRepository.ObterFuncionario(Id);
            return View(funcionario);
        }

        [HttpPost]
        public IActionResult Atualizar([FromForm] Models.Funcionario funcionario)
        {
            if (ModelState.IsValid)
            {
                _funcionarioRepository.Atualizar(funcionario);
                TempData["MSG_S"] = "Cadastro atualizado com sucesso!";
                return RedirectToAction(nameof(Index));
            }
            return View();
        }

        [ValidateAntiForgeryToken]
        public IActionResult Excluir(int Id)
        {
            _funcionarioRepository.Excluir(Id);
            TempData["MSG_S"] = "Cadastro excluído com sucesso!";
            return RedirectToAction(nameof(Index));
        }
    }
}
