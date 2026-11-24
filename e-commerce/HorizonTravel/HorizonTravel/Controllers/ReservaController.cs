using System.Text.Json;
using HorizonTravel.Libraries.CarrinhoCompra;
using HorizonTravel.Libraries.Cookie;
using HorizonTravel.Libraries.Filtro;
using HorizonTravel.Libraries.Login;
using HorizonTravel.Models;
using HorizonTravel.Repository.Contract;
using HorizonTravel.Libraries.Promocao;
using Microsoft.AspNetCore.Mvc;

namespace HorizonTravel.Controllers
{
    [UsuarioAutorizacaoAtributte]
    public class ReservaController : Controller
    {
        private const string PassageirosSessionKey = "ReservaPassageiros";
        private const string PacoteSessionKey = "ReservaPacote";

        private readonly IReservaRepository _reservaRepository;
        private readonly IPacoteRepository _pacoteRepository;
        private readonly LoginUsuario _loginUsuario;
        private readonly IPromocaoRepository _promocaoRepository;
        private readonly CarrinhoCompra _carrinhoCompra;

        public ReservaController(
            IReservaRepository reservaRepository,
            IPacoteRepository pacoteRepository,
            LoginUsuario loginUsuario,
            IHttpContextAccessor context,
            IPromocaoRepository promocaoRepository)
        {
            _reservaRepository = reservaRepository;
            _pacoteRepository = pacoteRepository;
            _loginUsuario = loginUsuario;
            _promocaoRepository = promocaoRepository;
            _carrinhoCompra = new CarrinhoCompra(new Cookie(context));
        }

        [HttpGet]
        public IActionResult Passageiros()
        {
            var item = _carrinhoCompra.Consultar().FirstOrDefault();

            if (item == null)
            {
                return RedirectToAction("Index", "Carrinho");
            }

            var pacote = _pacoteRepository.ObterPacote(item.IdPacote);

            if (pacote == null || pacote.quantidadePac < pacote.quantidadePassageirosPac)
            {
                return RedirectToAction("Index", "Carrinho");
            }

            var modelo = new ReservaCheckout
            {
                IDPac = item.IdPacote,
                ValorTotal = ObterPrecoAtual(pacote) * pacote.quantidadePassageirosPac,
                Passageiros = Enumerable
                    .Range(0, pacote.quantidadePassageirosPac)
                    .Select(_ => new Passageiro())
                    .ToList()
            };

            return View(modelo);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Passageiros(ReservaCheckout modelo)
        {
            ModelState.Remove(nameof(modelo.metodoPag));

            var item = _carrinhoCompra.Consultar().FirstOrDefault();

            if (item == null || item.IdPacote != modelo.IDPac)
            {
                return RedirectToAction("Index", "Carrinho");
            }

            var pacote = _pacoteRepository.ObterPacote(item.IdPacote);

            if (pacote == null || modelo.Passageiros.Count != pacote.quantidadePassageirosPac)
            {
                ModelState.AddModelError(
                    string.Empty,
                    $"Cadastre exatamente {pacote?.quantidadePassageirosPac ?? 0} passageiros.");
            }

            if (!ModelState.IsValid || modelo.Passageiros.Count == 0)
            {
                return View(modelo);
            }

            HttpContext.Session.SetString(
                PassageirosSessionKey,
                JsonSerializer.Serialize(modelo.Passageiros));

            HttpContext.Session.SetInt32(PacoteSessionKey, item.IdPacote);

            return RedirectToAction(nameof(Pagamento));
        }

        [HttpGet]
        public IActionResult Pagamento()
        {
            int? idPacote = HttpContext.Session.GetInt32(PacoteSessionKey);
            var pacote = idPacote.HasValue
                ? _pacoteRepository.ObterPacote(idPacote.Value)
                : null;

            if (pacote == null)
            {
                return RedirectToAction(nameof(Passageiros));
            }

            var passageiros = ObterPassageirosDaSessao();

            var modelo = new ReservaCheckout
            {
                IDPac = pacote.IDPac,
                ValorTotal = ObterPrecoAtual(pacote) * passageiros.Count
            };

            return View(modelo);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Pagamento(ReservaCheckout modelo)
        {
            var usuario = _loginUsuario.GetUsuario();
            int? idPacote = HttpContext.Session.GetInt32(PacoteSessionKey);
            var passageiros = ObterPassageirosDaSessao();
            var pacote = idPacote.HasValue
                ? _pacoteRepository.ObterPacote(idPacote.Value)
                : null;

            if (usuario == null || pacote == null || passageiros.Count == 0)
            {
                return RedirectToAction(nameof(Passageiros));
            }

            modelo.IDPac = pacote.IDPac;
            modelo.ValorTotal = ObterPrecoAtual(pacote) * passageiros.Count;

            ValidarPagamento(modelo);

            if (!ModelState.IsValid)
            {
                return View(modelo);
            }

            var reserva = new Reserva
            {
                IDUsu = usuario.IDUsu,
                IDPac = pacote.IDPac,
                dataRes = DateTime.Today,
                statusRes = "Pendente"
            };

            try
            {
                int idReserva = _reservaRepository.FinalizarReserva(
                    reserva,
                    passageiros,
                    new Pagamento
                    {
                        valorPag = modelo.ValorTotal,
                        dataPag = DateTime.Today,
                        metodoPag = modelo.metodoPag,
                        statusPag = "Pendente"
                    });

                _carrinhoCompra.RemoverTodos();
                HttpContext.Session.Remove(PassageirosSessionKey);
                HttpContext.Session.Remove(PacoteSessionKey);

                return RedirectToAction(nameof(Confirmacao), new { id = idReserva });
            }
            catch (InvalidOperationException exception)
            {
                ModelState.AddModelError(string.Empty, exception.Message);
                return View(modelo);
            }
        }

        [HttpGet]
        public IActionResult Confirmacao(int id)
        {
            var detalhes = _reservaRepository.ObterDetalhesReserva(id);
            var usuario = _loginUsuario.GetUsuario();

            if (detalhes == null || usuario == null || detalhes.Reserva.IDUsu != usuario.IDUsu)
            {
                return NotFound();
            }

            return View(detalhes);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult ConfirmarPagamento(int id)
        {
            var detalhes = _reservaRepository.ObterDetalhesReserva(id);
            var usuario = _loginUsuario.GetUsuario();

            if (detalhes == null || usuario == null || detalhes.Reserva.IDUsu != usuario.IDUsu)
            {
                return NotFound();
            }

            _reservaRepository.AtualizarStatusPagamento(id, "Pago", null);
            return RedirectToAction(nameof(Confirmacao), new { id });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult CancelarPagamento(int id)
        {
            var detalhes = _reservaRepository.ObterDetalhesReserva(id);
            var usuario = _loginUsuario.GetUsuario();

            if (detalhes == null || usuario == null || detalhes.Reserva.IDUsu != usuario.IDUsu)
            {
                return NotFound();
            }

            _reservaRepository.AtualizarStatusPagamento(id, "Cancelado", null);
            return RedirectToAction(nameof(Confirmacao), new { id });
        }

        private List<Passageiro> ObterPassageirosDaSessao()
        {
            string? passageirosJson = HttpContext.Session.GetString(PassageirosSessionKey);

            return string.IsNullOrWhiteSpace(passageirosJson)
                ? new List<Passageiro>()
                : JsonSerializer.Deserialize<List<Passageiro>>(passageirosJson)
                    ?? new List<Passageiro>();
        }

        private decimal ObterPrecoAtual(Pacote pacote)
        {
            var promocao = PromocaoCalculadora.ObterMelhorPromocao(
                pacote,
                _promocaoRepository.ObterPromocoesAtivas());

            return PromocaoCalculadora.CalcularPreco(pacote, promocao);
        }

        private void ValidarPagamento(ReservaCheckout modelo)
        {
            string[] metodosPermitidos = ["PIX", "Cartão de crédito", "Cartão de débito"];

            if (!metodosPermitidos.Contains(modelo.metodoPag))
            {
                ModelState.AddModelError(nameof(modelo.metodoPag), "Selecione um método de pagamento válido.");
                return;
            }

            if (modelo.metodoPag == "PIX")
            {
                return;
            }

            if (string.IsNullOrWhiteSpace(modelo.nomeTitular) ||
                string.IsNullOrWhiteSpace(modelo.numeroCartao) ||
                string.IsNullOrWhiteSpace(modelo.validadeCartao) ||
                string.IsNullOrWhiteSpace(modelo.cvv))
            {
                ModelState.AddModelError(string.Empty, "Preencha todos os dados do cartão.");
                return;
            }

            string numeroCartao = new string(modelo.numeroCartao.Where(char.IsDigit).ToArray());

            if (numeroCartao.Length < 13 || numeroCartao.Length > 19)
            {
                ModelState.AddModelError(nameof(modelo.numeroCartao), "Informe um número de cartão válido.");
            }

            if (!ValidadeCartaoEhFutura(modelo.validadeCartao))
            {
                ModelState.AddModelError(nameof(modelo.validadeCartao), "Informe uma validade futura.");
            }
        }

        private static bool ValidadeCartaoEhFutura(string validade)
        {
            if (validade.Length != 5 || validade[2] != '/')
            {
                return false;
            }

            if (!int.TryParse(validade[..2], out int mes) ||
                !int.TryParse(validade[3..], out int ano))
            {
                return false;
            }

            if (mes is < 1 or > 12)
            {
                return false;
            }

            DateTime validadeCartao = new DateTime(2000 + ano, mes, 1).AddMonths(1).AddDays(-1);
            return validadeCartao >= DateTime.Today;
        }
    }
}
