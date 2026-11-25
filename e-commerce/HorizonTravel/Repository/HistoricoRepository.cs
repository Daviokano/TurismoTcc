using HorizonTravel.Models;
using HorizonTravel.Repository.Contract;
using MySql.Data.MySqlClient;

namespace HorizonTravel.Repository
{
    public class HistoricoRepository : IHistoricoRepository
    {
        private readonly string _conexaoMySQL;

        public HistoricoRepository(IConfiguration configuration)
        {
            _conexaoMySQL = configuration.GetConnectionString("ConexaoMySQL");
        }

        public IEnumerable<Historico> BuscarHistoricoPorCliente(int idUsuario)
        {
            using var conexao = new MySqlConnection(_conexaoMySQL);
            conexao.Open();

            const string sql = @"
                SELECT h.IDHist, h.IDRes, h.IDFun, h.statusAnterior,
                       h.statusNovo, h.dataModificacao, f.nomeFun
                FROM Historico h
                INNER JOIN Reserva r ON r.IDRes = h.IDRes
                LEFT JOIN Funcionario f ON f.IDFun = h.IDFun
                WHERE r.IDUsu = @IDUsu
                ORDER BY h.dataModificacao DESC, h.IDHist DESC";

            using var comando = new MySqlCommand(sql, conexao);
            comando.Parameters.Add("@IDUsu", MySqlDbType.Int32).Value = idUsuario;
            return LerHistoricos(comando);
        }

        public IEnumerable<Historico> ObterHistorico(int idReserva)
        {
            using var conexao = new MySqlConnection(_conexaoMySQL);
            conexao.Open();

            const string sql = @"
                SELECT h.IDHist, h.IDRes, h.IDFun, h.statusAnterior,
                       h.statusNovo, h.dataModificacao, f.nomeFun
                FROM Historico h
                LEFT JOIN Funcionario f ON f.IDFun = h.IDFun
                WHERE h.IDRes = @IDRes
                ORDER BY h.dataModificacao DESC, h.IDHist DESC";

            using var comando = new MySqlCommand(sql, conexao);
            comando.Parameters.Add("@IDRes", MySqlDbType.Int32).Value = idReserva;
            return LerHistoricos(comando);
        }

        public void RegistrarAlteracao(
            int idReserva,
            int? idFuncionario,
            string? statusAnterior,
            string statusNovo)
        {
            using var conexao = new MySqlConnection(_conexaoMySQL);
            conexao.Open();

            const string sql = @"
                INSERT INTO Historico
                    (IDRes, IDFun, statusAnterior, statusNovo, dataModificacao)
                VALUES
                    (@IDRes, @IDFun, @statusAnterior, @statusNovo, NOW())";

            using var comando = new MySqlCommand(sql, conexao);
            AdicionarParametros(comando, idReserva, idFuncionario, statusAnterior, statusNovo);
            comando.ExecuteNonQuery();
        }

        private static List<Historico> LerHistoricos(MySqlCommand comando)
        {
            var historicos = new List<Historico>();
            using var leitor = comando.ExecuteReader();

            while (leitor.Read())
            {
                historicos.Add(new Historico
                {
                    IDHist = Convert.ToInt32(leitor["IDHist"]),
                    IDRes = Convert.ToInt32(leitor["IDRes"]),
                    IDFun = leitor["IDFun"] == DBNull.Value ? null : Convert.ToInt32(leitor["IDFun"]),
                    statusAnterior = leitor["statusAnterior"] == DBNull.Value ? null : Convert.ToString(leitor["statusAnterior"]),
                    statusNovo = leitor["statusNovo"] == DBNull.Value ? null : Convert.ToString(leitor["statusNovo"]),
                    dataModificacao = Convert.ToDateTime(leitor["dataModificacao"]),
                    nomeFun = leitor["nomeFun"] == DBNull.Value ? null : Convert.ToString(leitor["nomeFun"])
                });
            }

            return historicos;
        }

        private static void AdicionarParametros(
            MySqlCommand comando,
            int idReserva,
            int? idFuncionario,
            string? statusAnterior,
            string statusNovo)
        {
            comando.Parameters.Add("@IDRes", MySqlDbType.Int32).Value = idReserva;
            comando.Parameters.Add("@IDFun", MySqlDbType.Int32).Value =
                idFuncionario.HasValue ? idFuncionario.Value : DBNull.Value;
            comando.Parameters.Add("@statusAnterior", MySqlDbType.VarChar).Value =
                statusAnterior ?? (object)DBNull.Value;
            comando.Parameters.Add("@statusNovo", MySqlDbType.VarChar).Value = statusNovo;
        }
    }
}
