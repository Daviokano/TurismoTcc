using Microsoft.AspNetCore.Mvc;

namespace HorizonTravel.Controllers;

public class PacotesController : Controller
{
    public IActionResult Index() => View();
    public IActionResult Detalhes() => View();
}
