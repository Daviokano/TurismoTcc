using HorizonTravel.Models;
using HorizonTravel.Repository.Contract;
using Microsoft.AspNetCore.Mvc;

namespace HorizonTravel.Controllers
{
    public class AvaliacaoController : Controller
    {
        private readonly IAvaliacaoRepository _avaliacaoRepository;

        public AvaliacaoController(IAvaliacaoRepository avaliacaoRepository)
        {
            _avaliacaoRepository = avaliacaoRepository;
        }

        // GET: /Avaliacao?id=5
        [HttpGet]
        public IActionResult Index(int id)
        {
            if (id == 0)
            {
                return NotFound();
            }

            var avaliacao = new Avaliacao
            {
                IDPac = id,
                dataAva = DateTime.Now
            };

            return View(avaliacao);
        }

        // POST: /Avaliacao
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Index(Avaliacao avaliacao)
        {
            if (ModelState.IsValid)
            {
                // SUBSTITUA PELA SUA LÓGICA DE SESSÃO DE USUÁRIO LOGADO (Mudar Depois)
                avaliacao.IDUsu = 1;
                avaliacao.dataAva = DateTime.Now;

                _avaliacaoRepository.Cadastrar(avaliacao);

                // Redireciona de volta para a tela de pacotes
                return RedirectToAction("Index", "Pacotes");
            }

            return View(avaliacao);
        }
    }
}