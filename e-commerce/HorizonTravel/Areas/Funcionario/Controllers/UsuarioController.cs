using HorizonTravel.Libraries.Filtro;
using HorizonTravel.Models;
using HorizonTravel.Repository.Contract;
using Microsoft.AspNetCore.Mvc;

namespace HorizonTravel.Areas.Funcionario.Controllers
{
    [Area("Funcionario")]
    [FuncionarioAutorizacao]
    public class UsuarioController : Controller
    {
        private IUsuarioRepository _usuarioRepository;
        private IReservaRepository _reservaRepository;

        public UsuarioController(
            IUsuarioRepository usuarioRepository,
            IReservaRepository reservaRepository)
        {
            _usuarioRepository = usuarioRepository;
            _reservaRepository = reservaRepository;
        }

        public IActionResult Index()
        {
            return View(_usuarioRepository.ObterTodosUsuarios());
        }

        [HttpGet]
        public IActionResult Historico(string? busca, int? id)
        {
            var usuarios = _usuarioRepository.ObterTodosUsuarios().ToList();
            var modelo = new HistoricoClienteViewModel
            {
                Busca = busca
            };

            if (!string.IsNullOrWhiteSpace(busca))
            {
                modelo.ResultadosBusca = usuarios.Where(usuario =>
                    usuario.nomeUsu.Contains(busca, StringComparison.OrdinalIgnoreCase) ||
                    usuario.CPFUsu.Contains(busca, StringComparison.OrdinalIgnoreCase))
                    .ToList();

                if (!id.HasValue && modelo.ResultadosBusca.Count() == 1)
                {
                    id = modelo.ResultadosBusca.Single().IDUsu;
                }
            }
            else
            {
                modelo.ResultadosBusca = usuarios;
            }

            modelo.Clientes = modelo.ResultadosBusca;

            if (id.HasValue)
            {
                var cliente = _usuarioRepository.ObterUsuario(id.Value);

                if (cliente.IDUsu == 0)
                {
                    return NotFound();
                }

                modelo.Cliente = cliente;
                modelo.Reservas = _reservaRepository.ObterReservasPorUsuario(cliente.IDUsu);
                modelo.Historicos = _reservaRepository.BuscarHistoricoPorCliente(cliente.IDUsu);
            }

            return View(modelo);
        }

        //Para cadastro de novos clientes
        [HttpGet]
        public IActionResult Cadastrar()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Cadastrar(Models.Usuario usuario)
        {
            usuario.situacaoUsu = "A";
            _usuarioRepository.Cadastrar(usuario);
            usuario.CPFUsu = new string(usuario.CPFUsu.Where(char.IsDigit).ToArray());

            TempData["MSG_S"] = "Cadastro realizado com sucesso!";
            return RedirectToAction(nameof(Index));
        }


        // para atualizar dados de clientes
        [HttpGet]
        public IActionResult Atualizar(int Id)
        {
            Models.Usuario usuario = _usuarioRepository.ObterUsuario(Id);
            return View(usuario);
        }

        [HttpPost]
        public IActionResult Atualizar([FromForm] Models.Usuario usuario)
        {
            usuario.CPFUsu = new string(usuario.CPFUsu.Where(char.IsDigit).ToArray());

            // se o campo continuar vazio, mantém a senha atual cadastrada no banco.
            if (string.IsNullOrWhiteSpace(usuario.senhaUsu))
            {
                var usuarioBanco = _usuarioRepository.ObterUsuario(usuario.IDUsu);
                if (usuarioBanco != null)
                {
                    usuario.senhaUsu = usuarioBanco.senhaUsu;
                }
            }

            // removendo a validação do ModelState para o campo senha, já que ele agora é opcional na tela.
            ModelState.Remove(nameof(usuario.senhaUsu));

            if (ModelState.IsValid)
            {
                _usuarioRepository.Atualizar(usuario);
                TempData["MSG_S"] = "Cadastro atualizado com sucesso!";
                return RedirectToAction(nameof(Index));
            }

            return View(usuario);
        }


        //Para ativar e desativar clientes
        [ValidateHttpReferer]
        public IActionResult Ativar(int Id)
        {
            _usuarioRepository.Ativar(Id);
            return RedirectToAction(nameof(Index));
        }

        [ValidateHttpReferer]
        public IActionResult Desativar(int Id)
        {
            _usuarioRepository.Desativar(Id);
            return RedirectToAction(nameof(Index));
        }
    }
}