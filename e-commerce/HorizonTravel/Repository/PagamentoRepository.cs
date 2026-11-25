using HorizonTravel.Models;
using HorizonTravel.Repository.Contract;
using MySql.Data.MySqlClient;

namespace HorizonTravel.Repository
{
    public class PagamentoRepository : IPagamentoRepository
    {
        private readonly string _conexaoMySQL;

        public PagamentoRepository(IConfiguration configuration)
        {
            _conexaoMySQL = configuration.GetConnectionString("ConexaoMySQL");
        }

        public Pagamento? ObterPagamentoPorReserva(int idReserva)
        {
            using var conexao = new MySqlConnection(_conexaoMySQL);
            conexao.Open();

            const string sql = @"
                SELECT IDPag, IDRes, valorPag, dataPag, metodoPag, statusPag
                FROM Pagamento
                WHERE IDRes = @IDRes";

            using var comando = new MySqlCommand(sql, conexao);
            comando.Parameters.Add("@IDRes", MySqlDbType.Int32).Value = idReserva;

            using var leitor = comando.ExecuteReader();

            if (!leitor.Read())
            {
                return null;
            }

            return new Pagamento
            {
                IDPag = Convert.ToInt32(leitor["IDPag"]),
                IDRes = Convert.ToInt32(leitor["IDRes"]),
                valorPag = Convert.ToDecimal(leitor["valorPag"]),
                dataPag = Convert.ToDateTime(leitor["dataPag"]),
                metodoPag = Convert.ToString(leitor["metodoPag"]),
                statusPag = Convert.ToString(leitor["statusPag"])
            };
        }
    }
}
