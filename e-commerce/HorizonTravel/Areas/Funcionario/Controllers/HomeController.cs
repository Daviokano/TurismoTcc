using HorizonTravel.Libraries.Filtro;
using HorizonTravel.Libraries.Login;
using HorizonTravel.Repository;
using HorizonTravel.Repository.Contract;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.Extensions.Configuration;
using MySql.Data.MySqlClient;   // use o mesmo namespace dos seus repositories (pode ser MySqlConnector)

namespace HorizonTravel.Areas.Funcionario.Controllers
{
    [Area("Funcionario")]
    public class HomeController : Controller
    {
        private IFuncionarioRepositoty _funcionarioRepository;
        private LoginFuncionario _loginFuncionario;
        private IConfiguration _configuration;

        public HomeController(IFuncionarioRepositoty funcionarioRepositoty, LoginFuncionario loginFuncionario, IConfiguration configuration)
        {
            _funcionarioRepository = funcionarioRepositoty;
            _loginFuncionario = loginFuncionario;
            _configuration = configuration;
        }

        [HttpGet]
        public IActionResult LoginFuncionario()
        {
            return View();
        }

        [HttpPost]
        public IActionResult LoginFuncionario([FromForm] Models.Funcionario funcionario)
        {
            //para conferir se emailFun e senhaFun chegaram preenchidos
            Console.WriteLine($"Email: [{funcionario.emailFun}] Senha: [{funcionario.senhaFun}]");

            Models.Funcionario funcionarioDB = _funcionarioRepository.Login(funcionario.emailFun, funcionario.senhaFun);

            if (funcionarioDB.emailFun != null && funcionarioDB.senhaFun != null)
            {
                _loginFuncionario.Login(funcionarioDB);
                return RedirectToAction(nameof(PainelFuncionario));
            }
            else
            {
                ViewData["MSG_E"] = "Funcionário não encontrado, por favor verifique email e senha digitados.";
                return View();
            }
        }


        [FuncionarioAutorizacao]
        public IActionResult PainelFuncionario()
        {
            var funcionario = _loginFuncionario.GetFuncionario();

            // dados que já existiam
            ViewBag.nomeFun = funcionario.nomeFun;
            ViewBag.emailFun = funcionario.emailFun;
            ViewBag.CPFFun = funcionario.CPFFun;

            // novos dados do perfil
            ViewBag.telefoneFun = funcionario.telefoneFun;
            ViewBag.permissaoFun = funcionario.permissaoFun;

            // cards de resumo
            using var conexao = new MySqlConnection(_configuration.GetConnectionString("ConexaoMySQL"));
            conexao.Open();

            ViewBag.reservasMes = Contar(conexao,
                "SELECT COUNT(*) FROM Reserva WHERE MONTH(dataRes) = MONTH(CURDATE()) AND YEAR(dataRes) = YEAR(CURDATE())");
            ViewBag.totalClientes = Contar(conexao, "SELECT COUNT(*) FROM Usuario");
            ViewBag.totalPacotes = Contar(conexao, "SELECT COUNT(*) FROM Pacote");
            ViewBag.promocoesAtivas = Contar(conexao, "SELECT COUNT(*) FROM Promocao");   // coloque seu filtro de ativa, se tiver
            ViewBag.parceirosAtivos = Contar(conexao, "SELECT COUNT(*) FROM Parceiro WHERE situacaoFor = 'A'");

            return View();
        }

        [FuncionarioAutorizacao]
        public IActionResult Painel()
        {
            return View();
        }

        //verificar se deve ser criada outra index que mostre os dados dos funcionarios, como atualização
        [FuncionarioAutorizacao]
        public IActionResult Index()
        {
            return View();
        }

        [FuncionarioAutorizacao]
        public IActionResult Logout()
        {
            _loginFuncionario.Logout();
            // Redireciona para a action Login desta controller/área
            return RedirectToAction(nameof(LoginFuncionario));
        }

        // Executa uma consulta COUNT e devolve o número
        private int Contar(MySqlConnection conexao, string sql)
        {
            using var cmd = new MySqlCommand(sql, conexao);
            return Convert.ToInt32(cmd.ExecuteScalar());
        }

    }
}