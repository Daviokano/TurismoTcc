using HorizonTravel.Models;
using HorizonTravel.Repository.Contract;
using MySql.Data.MySqlClient;
using System.Data;

namespace HorizonTravel.Repository
{
    public class AvaliacaoRepository : IAvaliacaoRepository
    {
        private readonly string _conexaoMySQL;

        public AvaliacaoRepository(IConfiguration conf)
        {
            _conexaoMySQL = conf.GetConnectionString("ConexaoMySQL");
        }

        public void Atualizar(Avaliacao avaliacao)
        {
            using (var conexao = new MySqlConnection(_conexaoMySQL))
            {
                conexao.Open();

                MySqlCommand cmd = new MySqlCommand(@"UPDATE Avaliacao SET notaAva=@notaAva, comentarioAva=@comentarioAva, dataAva=@dataAva
                WHERE IDAvaliacao=@IDAvaliacao", conexao);

                cmd.Parameters.Add("@IDAvaliacao", MySqlDbType.Int32).Value = avaliacao.IDAvaliacao;
                cmd.Parameters.Add("@notaAva", MySqlDbType.Decimal).Value = avaliacao.notaAva;
                cmd.Parameters.Add("@comentarioAva", MySqlDbType.VarChar).Value = (object?)avaliacao.comentarioAva ?? DBNull.Value;
                cmd.Parameters.Add("@dataAva", MySqlDbType.DateTime).Value = avaliacao.dataAva;

                cmd.ExecuteNonQuery();
                conexao.Close();
            }
        }

        public void Cadastrar(Avaliacao avaliacao)
        {
            using (var conexao = new MySqlConnection(_conexaoMySQL))
            {
                conexao.Open();

                MySqlCommand cmd = new MySqlCommand(@"INSERT INTO Avaliacao(notaAva, comentarioAva, dataAva, IDUsu, IDPac)
                        VALUES (@notaAva, @comentarioAva, @dataAva, @IDUsu, @IDPac)", conexao);

                cmd.Parameters.Add("@notaAva", MySqlDbType.Decimal).Value = avaliacao.notaAva;
                cmd.Parameters.Add("@comentarioAva", MySqlDbType.VarChar).Value = (object?)avaliacao.comentarioAva ?? DBNull.Value;
                cmd.Parameters.Add("@dataAva", MySqlDbType.DateTime).Value = avaliacao.dataAva;
                cmd.Parameters.Add("@IDUsu", MySqlDbType.Int32).Value = avaliacao.IDUsu;
                cmd.Parameters.Add("@IDPac", MySqlDbType.Int32).Value = avaliacao.IDPac;

                cmd.ExecuteNonQuery();
                conexao.Close();
            }
        }

        public void Excluir(int idAvaliacao)
        {
            using (var conexao = new MySqlConnection(_conexaoMySQL))
            {
                conexao.Open();
                MySqlCommand cmd = new MySqlCommand(@"DELETE FROM Avaliacao WHERE IDAvaliacao=@IDAvaliacao", conexao);
                cmd.Parameters.Add("@IDAvaliacao", MySqlDbType.Int32).Value = idAvaliacao;
                cmd.ExecuteNonQuery();
                conexao.Close();
            }
        }

        public Avaliacao? ObterAvaliacao(int idAvaliacao)
        {
            using (var conexao = new MySqlConnection(_conexaoMySQL))
            {
                conexao.Open();

                MySqlCommand cmd = new MySqlCommand("SELECT * FROM Avaliacao WHERE IDAvaliacao=@IDAvaliacao", conexao);
                cmd.Parameters.AddWithValue("@IDAvaliacao", idAvaliacao);

                Avaliacao? avaliacao = null;

                using (MySqlDataReader dr = cmd.ExecuteReader(CommandBehavior.CloseConnection))
                {
                    while (dr.Read())
                    {
                        avaliacao = new Avaliacao();
                        avaliacao.IDAvaliacao = Convert.ToInt32(dr["IDAvaliacao"]);
                        avaliacao.notaAva = Convert.ToDecimal(dr["notaAva"]);
                        avaliacao.comentarioAva = dr["comentarioAva"] == DBNull.Value ? null : (string)dr["comentarioAva"];
                        avaliacao.dataAva = Convert.ToDateTime(dr["dataAva"]);
                        avaliacao.IDUsu = Convert.ToInt32(dr["IDUsu"]);
                        avaliacao.IDPac = Convert.ToInt32(dr["IDPac"]);
                    }
                }

                return avaliacao;
            }
        }

        public IEnumerable<Avaliacao> ObterTodasAvaliacoes()
        {
            List<Avaliacao> avaliacaoList = new List<Avaliacao>();
            using (var conexao = new MySqlConnection(_conexaoMySQL))
            {
                conexao.Open();

                MySqlCommand cmd = new MySqlCommand(@"SELECT * FROM Avaliacao ORDER BY dataAva DESC", conexao);
                MySqlDataAdapter da = new MySqlDataAdapter(cmd);
                DataTable dt = new DataTable();

                da.Fill(dt);

                conexao.Close();
                foreach (DataRow dr in dt.Rows)
                {
                    avaliacaoList.Add(MontarAvaliacao(dr));
                }
                return avaliacaoList;
            }
        }

        public IEnumerable<Avaliacao> ListarPorPacote(int idPac)
        {
            List<Avaliacao> avaliacaoList = new List<Avaliacao>();
            using (var conexao = new MySqlConnection(_conexaoMySQL))
            {
                conexao.Open();

                MySqlCommand cmd = new MySqlCommand(@"SELECT * FROM Avaliacao WHERE IDPac=@IDPac ORDER BY dataAva DESC", conexao);
                cmd.Parameters.Add("@IDPac", MySqlDbType.Int32).Value = idPac;

                MySqlDataAdapter da = new MySqlDataAdapter(cmd);
                DataTable dt = new DataTable();

                da.Fill(dt);

                conexao.Close();
                foreach (DataRow dr in dt.Rows)
                {
                    avaliacaoList.Add(MontarAvaliacao(dr));
                }
                return avaliacaoList;
            }
        }

        public IEnumerable<Avaliacao> ListarPorUsuario(int idUsu)
        {
            List<Avaliacao> avaliacaoList = new List<Avaliacao>();
            using (var conexao = new MySqlConnection(_conexaoMySQL))
            {
                conexao.Open();

                MySqlCommand cmd = new MySqlCommand(@"SELECT * FROM Avaliacao WHERE IDUsu=@IDUsu ORDER BY dataAva DESC", conexao);
                cmd.Parameters.Add("@IDUsu", MySqlDbType.Int32).Value = idUsu;

                MySqlDataAdapter da = new MySqlDataAdapter(cmd);
                DataTable dt = new DataTable();

                da.Fill(dt);

                conexao.Close();
                foreach (DataRow dr in dt.Rows)
                {
                    avaliacaoList.Add(MontarAvaliacao(dr));
                }
                return avaliacaoList;
            }
        }

        // Converte uma linha do DataTable em um objeto Avaliacao
        private Avaliacao MontarAvaliacao(DataRow dr)
        {
            return new Avaliacao
            {
                IDAvaliacao = Convert.ToInt32(dr["IDAvaliacao"]),
                notaAva = Convert.ToDecimal(dr["notaAva"]),
                comentarioAva = dr["comentarioAva"] == DBNull.Value ? null : (string)dr["comentarioAva"],
                dataAva = Convert.ToDateTime(dr["dataAva"]),
                IDUsu = Convert.ToInt32(dr["IDUsu"]),
                IDPac = Convert.ToInt32(dr["IDPac"])
            };
        }
    }
}