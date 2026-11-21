using HorizonTravel.Libraries.CarrinhoCompra;
using HorizonTravel.Libraries.Cookie;
using HorizonTravel.Models;
using HorizonTravel.Repository.Contract;
using Microsoft.AspNetCore.Mvc;

namespace HorizonTravel.Controllers;

public class CarrinhoController : Controller
{
    private CarrinhoCompra _carrinhoCompra;
    private readonly IPacoteRepository _pacoteRepository;

    public CarrinhoController(
        IHttpContextAccessor context,
        IPacoteRepository pacoteRepository)
    {
        _carrinhoCompra = new CarrinhoCompra(
            new HorizonTravel.Libraries.Cookie.Cookie(context));
        _pacoteRepository = pacoteRepository;
    }

    public IActionResult Index()
    {
        return View(_carrinhoCompra.Consultar());
    }

    [HttpPost]
    public IActionResult Adicionar(ItemCarrinho item)
    {
        var pacote = _pacoteRepository.ObterPacote(item.IdPacote);

        if (pacote == null || pacote.quantidadePac <= 0)
        {
            return RedirectToAction(nameof(Index));
        }

        item.NomePacote = $"Pacote {pacote.destinoPac}";
        item.Destino = pacote.destinoPac;
        item.Preco = pacote.precoPac;
        item.Imagem = pacote.fotoPac;

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
