using HorizonTravel.Libraries.CarrinhoCompra;
using HorizonTravel.Libraries.Cookie;
using HorizonTravel.Libraries.Filtro;
using HorizonTravel.Models;
using Microsoft.AspNetCore.Mvc;

namespace HorizonTravel.Controllers;

[UsuarioAutorizacaoAtributte]
public class CarrinhoController : Controller
{
    private CarrinhoCompra _carrinhoCompra;

    public CarrinhoController(IHttpContextAccessor context)
    {
        _carrinhoCompra = new CarrinhoCompra(
            new HorizonTravel.Libraries.Cookie.Cookie(context));
    }

    public IActionResult Index()
    {
        return View(_carrinhoCompra.Consultar());
    }

    [HttpPost]
    public IActionResult Adicionar(ItemCarrinho item)
    {
        _carrinhoCompra.Cadastrar(item);

        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    public IActionResult Atualizar(int idPacote, int quantidade)
    {
        _carrinhoCompra.Atualizar(new ItemCarrinho
        {
            IdPacote = idPacote,
            Quantidade = quantidade
        });

        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    public IActionResult Remover(int idPacote)
    {
        _carrinhoCompra.Remover(new ItemCarrinho { IdPacote = idPacote });

        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    public IActionResult Limpar()
    {
        _carrinhoCompra.RemoverTodos();

        return RedirectToAction(nameof(Index));
    }
}
