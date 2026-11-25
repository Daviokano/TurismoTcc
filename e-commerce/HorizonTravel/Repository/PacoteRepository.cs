using HorizonTravel.Models;
using HorizonTravel.Repository.Contract;
using MySql.Data.MySqlClient;
using System.Data;

namespace HorizonTravel.Repository
{
    public class PacoteRepository : IPacoteRepository
    {
        private readonly string _conexaoMySQL;

        public PacoteRepository(IConfiguration configuration)
        {
            _conexaoMySQL = configuration.GetConnectionString("ConexaoMySQL");
        }

        public void Cadastrar(Pacote pacote)
        {
            using var conexao = new MySqlConnection(_conexaoMySQL);
            conexao.Open();

            const string sql = @"
                INSERT INTO Pacote
                    (IDFun, CNPJFor, IDPromo, precoPac, quantidadePac, quantidadePassageirosPac,
                    descricaoPac, avaliacaoPac, fotoPac, destinoPac, origemPac, dataPac, situacaoPac)
                VALUES
                    (@IDFun, @CNPJFor, @IDPromo, @precoPac, @quantidadePac, @quantidadePassageirosPac,
                    @descricaoPac, @avaliacaoPac, @fotoPac, @destinoPac, @origemPac, @dataPac, @situacaoPac)";

            using var comando = new MySqlCommand(sql, conexao);
            AdicionarParametros(comando, pacote, incluirId: false);
            comando.ExecuteNonQuery();
        }

        public void Atualizar(Pacote pacote)
        {
            using var conexao = new MySqlConnection(_conexaoMySQL);
            conexao.Open();

            const string sql = @"
                UPDATE Pacote
                SET IDFun = @IDFun,
                    CNPJFor = @CNPJFor,
                    IDPromo = @IDPromo,
                    precoPac = @precoPac,
                    quantidadePac = @quantidadePac,
                    quantidadePassageirosPac = @quantidadePassageirosPac,
                    descricaoPac = @descricaoPac,
                    avaliacaoPac = @avaliacaoPac,
                    fotoPac = @fotoPac,
                    destinoPac = @destinoPac,
                    origemPac = @origemPac,
                    dataPac = @dataPac,
                    situacaoPac = @situacaoPac
                WHERE IDPac = @IDPac";

            using var comando = new MySqlCommand(sql, conexao);
            AdicionarParametros(comando, pacote, incluirId: true);
            comando.ExecuteNonQuery();
        }

        public Pacote ObterPacote(int id)
        {
            using var conexao = new MySqlConnection(_conexaoMySQL);
            conexao.Open();

            const string sql = "SELECT * FROM Pacote WHERE IDPac = @IDPac";
            using var comando = new MySqlCommand(sql, conexao);
            comando.Parameters.Add("@IDPac", MySqlDbType.Int32).Value = id;

            using var leitor = comando.ExecuteReader(CommandBehavior.CloseConnection);
            return leitor.Read() ? Mapear(leitor) : null;
        }

        public IEnumerable<Pacote> ObterTodosPacotes()
        {
            var pacotes = new List<Pacote>();

            using var conexao = new MySqlConnection(_conexaoMySQL);
            conexao.Open();

            const string sql = "SELECT * FROM Pacote ORDER BY IDPac DESC";
            using var comando = new MySqlCommand(sql, conexao);
            using var leitor = comando.ExecuteReader();

            while (leitor.Read())
            {
                pacotes.Add(Mapear(leitor));
            }

            return pacotes;
        }

        private static void AdicionarParametros(
            MySqlCommand comando,
            Pacote pacote,
            bool incluirId)
        {
            string cnpj = System.Text.RegularExpressions.Regex.Replace(
                pacote.CNPJFor ?? string.Empty,
                @"\D",
                string.Empty);

            if (incluirId)
            {
                comando.Parameters.Add("@IDPac", MySqlDbType.Int32).Value = pacote.IDPac;
            }

            comando.Parameters.Add("@IDFun", MySqlDbType.Int32).Value = pacote.IDFun;
            comando.Parameters.Add("@CNPJFor", MySqlDbType.VarChar).Value = cnpj;
            comando.Parameters.Add("@IDPromo", MySqlDbType.Int32).Value =
                pacote.IDPromo.HasValue ? pacote.IDPromo.Value : DBNull.Value;
            comando.Parameters.Add("@precoPac", MySqlDbType.Decimal).Value = pacote.precoPac;
            comando.Parameters.Add("@quantidadePac", MySqlDbType.Int32).Value = pacote.quantidadePac;
            comando.Parameters.Add("@quantidadePassageirosPac", MySqlDbType.Int32).Value = pacote.quantidadePassageirosPac;
            comando.Parameters.Add("@descricaoPac", MySqlDbType.Text).Value = pacote.descricaoPac;
            comando.Parameters.Add("@avaliacaoPac", MySqlDbType.Decimal).Value =
                pacote.avaliacaoPac.HasValue ? pacote.avaliacaoPac.Value : DBNull.Value;
            comando.Parameters.Add("@fotoPac", MySqlDbType.VarChar).Value = pacote.fotoPac;
            comando.Parameters.Add("@destinoPac", MySqlDbType.VarChar).Value = pacote.destinoPac;
            comando.Parameters.Add("@origemPac", MySqlDbType.VarChar).Value = pacote.origemPac;
            comando.Parameters.Add("@dataPac", MySqlDbType.Date).Value = pacote.dataPac.Date;
            comando.Parameters.Add("@situacaoPac", MySqlDbType.VarChar).Value = pacote.situacaoPac;
        }

        private static Pacote Mapear(IDataRecord registro)
        {
            return new Pacote
            {
                IDPac = Convert.ToInt32(registro["IDPac"]),
                IDFun = Convert.ToInt32(registro["IDFun"]),
                CNPJFor = Convert.ToString(registro["CNPJFor"]),
                IDPromo = registro["IDPromo"] == DBNull.Value ? null : Convert.ToInt32(registro["IDPromo"]),
                precoPac = Convert.ToDecimal(registro["precoPac"]),
                quantidadePac = Convert.ToInt32(registro["quantidadePac"]),
                quantidadePassageirosPac = Convert.ToInt32(registro["quantidadePassageirosPac"]),
                descricaoPac = Convert.ToString(registro["descricaoPac"]),
                avaliacaoPac = registro["avaliacaoPac"] == DBNull.Value ? null : Convert.ToDecimal(registro["avaliacaoPac"]),
                fotoPac = registro["fotoPac"] == DBNull.Value ? null : Convert.ToString(registro["fotoPac"]),
                destinoPac = Convert.ToString(registro["destinoPac"]),
                origemPac = registro["origemPac"] == DBNull.Value
                    ? null
                    : Convert.ToString(registro["origemPac"]),
                dataPac = Convert.ToDateTime(registro["dataPac"]),
                situacaoPac = Convert.ToString(registro["situacaoPac"])
            };
        }
    }
}
