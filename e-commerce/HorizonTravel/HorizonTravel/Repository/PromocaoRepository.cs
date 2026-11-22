using HorizonTravel.Models;
using HorizonTravel.Repository.Contract;
using MySql.Data.MySqlClient;
using System.Data;

namespace HorizonTravel.Repository
{
    public class PromocaoRepository : IPromocaoRepository
    {
        private readonly string _conexaoMySQL;

        public PromocaoRepository(IConfiguration configuration)
        {
            _conexaoMySQL = configuration.GetConnectionString("ConexaoMySQL");
        }

        public void Cadastrar(Promocao promocao)
        {
            using var conexao = new MySqlConnection(_conexaoMySQL);
            conexao.Open();

            const string sql = @"
                INSERT INTO Promocao (porcentagemDesconto, dataValidade)
                VALUES (@porcentagemDesconto, @dataValidade)";

            using var comando = new MySqlCommand(sql, conexao);
            AdicionarParametros(comando, promocao);
            comando.ExecuteNonQuery();
        }

        public void Atualizar(Promocao promocao)
        {
            using var conexao = new MySqlConnection(_conexaoMySQL);
            conexao.Open();

            const string sql = @"
                UPDATE Promocao
                SET porcentagemDesconto = @porcentagemDesconto,
                    dataValidade = @dataValidade
                WHERE IDPromo = @IDPromo";

            using var comando = new MySqlCommand(sql, conexao);
            comando.Parameters.Add("@IDPromo", MySqlDbType.Int32).Value = promocao.IDPromo;
            AdicionarParametros(comando, promocao);
            comando.ExecuteNonQuery();
        }

        public void Remover(int id)
        {
            using var conexao = new MySqlConnection(_conexaoMySQL);
            conexao.Open();

            const string sql = "DELETE FROM Promocao WHERE IDPromo = @IDPromo";
            using var comando = new MySqlCommand(sql, conexao);
            comando.Parameters.Add("@IDPromo", MySqlDbType.Int32).Value = id;
            comando.ExecuteNonQuery();
        }

        public Promocao? ObterPromocao(int id)
        {
            using var conexao = new MySqlConnection(_conexaoMySQL);
            conexao.Open();

            const string sql = "SELECT * FROM Promocao WHERE IDPromo = @IDPromo";
            using var comando = new MySqlCommand(sql, conexao);
            comando.Parameters.Add("@IDPromo", MySqlDbType.Int32).Value = id;

            using var leitor = comando.ExecuteReader(CommandBehavior.CloseConnection);
            return leitor.Read() ? Mapear(leitor) : null;
        }

        public IEnumerable<Promocao> ObterTodasPromocoes()
        {
            return Consultar("SELECT * FROM Promocao ORDER BY dataValidade DESC");
        }

        public IEnumerable<Promocao> ObterPromocoesAtivas()
        {
            using var conexao = new MySqlConnection(_conexaoMySQL);
            conexao.Open();

            const string sql = @"
                SELECT * FROM Promocao
                WHERE dataValidade >= @hoje
                ORDER BY dataValidade";

            using var comando = new MySqlCommand(sql, conexao);
            comando.Parameters.Add("@hoje", MySqlDbType.Date).Value = DateTime.Today;

            using var leitor = comando.ExecuteReader();
            var promocoes = new List<Promocao>();

            while (leitor.Read())
            {
                promocoes.Add(Mapear(leitor));
            }

            return promocoes;
        }

        private IEnumerable<Promocao> Consultar(string sql)
        {
            using var conexao = new MySqlConnection(_conexaoMySQL);
            conexao.Open();

            using var comando = new MySqlCommand(sql, conexao);
            using var leitor = comando.ExecuteReader();
            var promocoes = new List<Promocao>();

            while (leitor.Read())
            {
                promocoes.Add(Mapear(leitor));
            }

            return promocoes;
        }

        private static void AdicionarParametros(MySqlCommand comando, Promocao promocao)
        {
            comando.Parameters.Add("@porcentagemDesconto", MySqlDbType.Decimal).Value = promocao.porcentagemDesconto;
            comando.Parameters.Add("@dataValidade", MySqlDbType.Date).Value = promocao.dataValidade.Date;
        }

        private static Promocao Mapear(IDataRecord registro)
        {
            return new Promocao
            {
                IDPromo = Convert.ToInt32(registro["IDPromo"]),
                porcentagemDesconto = Convert.ToDecimal(registro["porcentagemDesconto"]),
                dataValidade = Convert.ToDateTime(registro["dataValidade"])
            };
        }
    }
}
