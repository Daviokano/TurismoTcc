using HorizonTravel.Libraries.Filtro;
using HorizonTravel.Libraries.Login;
using HorizonTravel.Models.Constant;
using HorizonTravel.Repository.Contract;
using Microsoft.AspNetCore.Mvc;

namespace HorizonTravel.Areas.Funcionario.Controllers
{
    [Area("Funcionario")]
    [FuncionarioAutorizacao]
    public class ReservaController : Controller
    {
        private readonly IReservaRepository _reservaRepository;
        private readonly LoginFuncionario _loginFuncionario;

        public ReservaController(
            IReservaRepository reservaRepository,
            LoginFuncionario loginFuncionario)
        {
            _reservaRepository = reservaRepository;
            _loginFuncionario = loginFuncionario;
        }

        [HttpGet]
        public IActionResult Detalhes(int id)
        {
            var detalhes = _reservaRepository.ObterDetalhesReserva(id);

            return detalhes == null ? NotFound() : View(detalhes);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult AtualizarStatus(int id, string statusRes)
        {
            string[] statusPermitidos =
            [
                StatusReservaConstant.Pendente,
                StatusReservaConstant.Confirmada,
                StatusReservaConstant.Cancelada
            ];

            if (!statusPermitidos.Contains(statusRes))
            {
                return BadRequest();
            }

            int idFuncionario = _loginFuncionario.GetFuncionario().IDFun;

            if (statusRes == StatusReservaConstant.Confirmada)
            {
                _reservaRepository.AtualizarStatusPagamento(id, StatusPagamentoConstant.Pago, idFuncionario);
            }
            else if (statusRes == StatusReservaConstant.Cancelada)
            {
                _reservaRepository.AtualizarStatusPagamento(id, StatusPagamentoConstant.Cancelado, idFuncionario);
            }
            else
            {
                _reservaRepository.AtualizarStatusReserva(id, statusRes, idFuncionario);
            }

            TempData["MSG_S"] = "Status da reserva atualizado com sucesso.";
            return RedirectToAction(nameof(Detalhes), new { id });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult AtualizarPagamento(int id, string statusPag)
        {
            string[] statusPermitidos =
            [
                StatusPagamentoConstant.Pendente,
                StatusPagamentoConstant.Pago,
                StatusPagamentoConstant.Cancelado
            ];

            if (!statusPermitidos.Contains(statusPag))
            {
                return BadRequest();
            }

            _reservaRepository.AtualizarStatusPagamento(
                id,
                statusPag,
                _loginFuncionario.GetFuncionario().IDFun);

            TempData["MSG_S"] = "Status do pagamento atualizado com sucesso.";
            return RedirectToAction(nameof(Detalhes), new { id });
        }
    }
}
