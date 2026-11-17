using Microsoft.AspNetCore.Mvc;
using HorizonTravel.Repository.Contract;
using HorizonTravel.Models;

namespace HorizonTravel.Controllers;

public class PacotesController : Controller
{
    private readonly IPacoteRepository _pacoteRepository;

    public PacotesController(IPacoteRepository pacoteRepository)
    {
        _pacoteRepository = pacoteRepository;
    }

    public IActionResult Index()
    {
        return View(_pacoteRepository.ObterTodosPacotes());
    }

    public IActionResult Detalhes(int id)
    {
        Pacote pacote = _pacoteRepository.ObterPacote(id);

        return pacote == null ? NotFound() : View(pacote);
    }
}
