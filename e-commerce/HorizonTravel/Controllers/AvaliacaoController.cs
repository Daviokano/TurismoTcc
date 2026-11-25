using HorizonTravel.Libraries.Filtro;
using HorizonTravel.Libraries.Login;
using HorizonTravel.Models;
using HorizonTravel.Repository.Contract;
using Microsoft.AspNetCore.Mvc;

namespace HorizonTravel.Controllers
{
    public class AvaliacaoController : Controller
    {
        private readonly IAvaliacaoRepository _avaliacaoRepository;
        private readonly LoginUsuario _loginUsuario;

        public AvaliacaoController(
            IAvaliacaoRepository avaliacaoRepository,
            LoginUsuario loginUsuario)
        {
            _avaliacaoRepository = avaliacaoRepository;
            _loginUsuario = loginUsuario;
        }

        [HttpGet]
        [UsuarioAutorizacaoAtributte] 
        public IActionResult Index(int id)
        {
            if (id == 0)
                return NotFound();

            var usuario = _loginUsuario.GetUsuario();

            var avaliacao = new Avaliacao
            {
                IDPac = id,
                IDUsu = usuario.IDUsu,
                dataAva = DateTime.Now,
                notaAva = null  
            };

            return View(avaliacao);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [UsuarioAutorizacaoAtributte]  
        public IActionResult Index(Avaliacao avaliacao)
        {
            var usuario = _loginUsuario.GetUsuario();

            // usando id da sessão para preencher o ID do usuário na avaliação
            avaliacao.IDUsu = usuario.IDUsu;

            if (!avaliacao.notaAva.HasValue)
            {
                ModelState.AddModelError(nameof(avaliacao.notaAva), "Selecione uma nota.");
            }

            if (ModelState.IsValid)
            {
                avaliacao.dataAva = DateTime.Now;

                _avaliacaoRepository.Cadastrar(avaliacao);

                return RedirectToAction("Index", "Pacotes");
            }

            return View(avaliacao);
        }
    }
}