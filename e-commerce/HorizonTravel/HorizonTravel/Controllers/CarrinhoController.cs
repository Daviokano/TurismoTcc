using HorizonTravel.Libraries.CarrinhoCompra;
using HorizonTravel.Libraries.Cookie;
using HorizonTravel.Models;
using HorizonTravel.Repository.Contract;
using HorizonTravel.Libraries.Promocao;
using Microsoft.AspNetCore.Mvc;

namespace HorizonTravel.Controllers;

public class CarrinhoController : Controller
{
    private CarrinhoCompra _carrinhoCompra;
    private readonly IPacoteRepository _pacoteRepository;
    private readonly IPromocaoRepository _promocaoRepository;

    public CarrinhoController(
        IHttpContextAccessor context,
        IPacoteRepository pacoteRepository,
        IPromocaoRepository promocaoRepository)
    {
        _carrinhoCompra = new CarrinhoCompra(
            new HorizonTravel.Libraries.Cookie.Cookie(context));
        _pacoteRepository = pacoteRepository;
        _promocaoRepository = promocaoRepository;
    }

    public IActionResult Index()
    {
        var itens = _carrinhoCompra.Consultar();

        foreach (var item in itens)
        {
            var pacote = _pacoteRepository.ObterPacote(item.IdPacote);

            if (pacote == null)
            {
                continue;
            }

            var promocao = PromocaoCalculadora.ObterMelhorPromocao(
                pacote,
                _promocaoRepository.ObterPromocoesAtivas());

            item.Preco = PromocaoCalculadora.CalcularPreco(pacote, promocao);
        }

        _carrinhoCompra.Salvar(itens);
        return View(itens);
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
        var promocao = PromocaoCalculadora.ObterMelhorPromocao(
            pacote,
            _promocaoRepository.ObterPromocoesAtivas());

        item.Preco = PromocaoCalculadora.CalcularPreco(pacote, promocao);
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
