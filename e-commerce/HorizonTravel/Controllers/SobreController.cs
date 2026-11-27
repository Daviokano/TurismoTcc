using Microsoft.AspNetCore.Mvc;

namespace HorizonTravel.Controllers;

public class SobreController : Controller
{
    public IActionResult Index() => View();

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult EnviarMensagem(
        string nome,
        string email,
        string telefone,
        string assunto,
        string mensagem)
    {
        if (string.IsNullOrWhiteSpace(nome)
            || string.IsNullOrWhiteSpace(email)
            || string.IsNullOrWhiteSpace(assunto)
            || string.IsNullOrWhiteSpace(mensagem))
        {
            TempData["MSG_E"] = "Preencha nome, e-mail, assunto e mensagem.";
            return RedirectToAction(nameof(Index));
        }

        TempData["MSG_S"] = "Mensagem enviada com sucesso para HorizonTravel@gmail.com. (Simulação)";
        return RedirectToAction(nameof(Index));
    }
}
