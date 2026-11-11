using HorizonTravel.Models;
using HorizonTravel.Models.Constant;
using HorizonTravel.Repository.Contract;
using MySql.Data.MySqlClient;
using System.Data;

namespace HorizonTravel.Repository
{
    public class ParceiroRepository : IParceiroRepository
    {
        private readonly string _conexaoMySQL;

        public ParceiroRepository(IConfiguration conf)
        {
            _conexaoMySQL = conf.GetConnectionString("ConexaoMySQL");
        }

        public void Ativar(int Id)
        {
            string situacao = SituacaoParceiroConstant.Ativo;

            using (var conexao = new MySqlConnection(_conexaoMySQL))
            {
                conexao.Open();
                MySqlCommand cmd = new MySqlCommand(@"UPDATE Parceiro SET situacaoFor=@situacaoFor WHERE IDFor=@IDFor", conexao);
                cmd.Parameters.Add("@situacaoFor", MySqlDbType.VarChar).Value = situacao;
                cmd.Parameters.Add("@IDFor", MySqlDbType.Int32).Value = Id;
                cmd.ExecuteNonQuery();
                conexao.Close();
            }
        }

        public void Atualizar(Parceiro parceiro)
        {
            using (var conexao = new MySqlConnection(_conexaoMySQL))
            {
                conexao.Open();

                MySqlCommand cmd = new MySqlCommand(@"UPDATE Parceiro SET nomeFor=@nomeFor, CNPJFor=@CNPJFor, emailFor=@emailFor, situacaoFor=@situacaoFor
                WHERE IDFor=@IDFor");

                cmd.Parameters.Add("@nomeFor", MySqlDbType.VarChar).Value = parceiro.nomeFor;
                cmd.Parameters.Add("@CNPJFor", MySqlDbType.Int32).Value = parceiro.CNPJFor;
                cmd.Parameters.Add("@emailFor", MySqlDbType.VarChar).Value = parceiro.emailFor;
                cmd.Parameters.Add("@IDFor", MySqlDbType.Int32).Value = parceiro.IDFor;
                cmd.Parameters.Add("@situacaoFor", MySqlDbType.VarChar).Value = parceiro.situacaoFor;


                cmd.ExecuteNonQuery();
                conexao.Close();
            }
        }

        public void Cadastrar(Parceiro parceiro)
        {
            using (var conexao = new MySqlConnection(_conexaoMySQL))
            {
                conexao.Open();

                MySqlCommand cmd = new MySqlCommand(@"INSERT INTO Parceiro(nomeFor, CNPJFor, emailFor, situacaoFor)
                        VALUES (@nomeFor, @CNPJFor, @emailFor, @situacaoFor)", conexao);

                cmd.Parameters.Add("@nomeFor", MySqlDbType.VarChar).Value = parceiro.nomeFor;
                cmd.Parameters.Add("@CNPJFor", MySqlDbType.Int32).Value = parceiro.CNPJFor;
                cmd.Parameters.Add("@emailFor", MySqlDbType.VarChar).Value = parceiro.emailFor;
                cmd.Parameters.Add("@situacaoFor", MySqlDbType.VarChar).Value = parceiro.situacaoFor;

                cmd.ExecuteNonQuery();
                conexao.Close();
            }
        }

        public void Desativar(int Id)
        {
            string situacao = SituacaoParceiroConstant.Inativo;

            using (var conexao = new MySqlConnection(_conexaoMySQL))
            {
                conexao.Open();
                MySqlCommand cmd = new MySqlCommand(@"UPDATE Parceiro SET situacaoFor=@situacaoFor WHERE IDFor=@IDFor", conexao);
                cmd.Parameters.Add("@situacaoFor", MySqlDbType.VarChar).Value = situacao;
                cmd.Parameters.Add("@IDFor", MySqlDbType.Int32).Value = Id;
                cmd.ExecuteNonQuery();
                conexao.Close();
            }
        }

        public void Excluir(int Id)
        {
            using (var conexao = new MySqlConnection(_conexaoMySQL))
            {
                conexao.Open();
                MySqlCommand cmd = new MySqlCommand(@"DELETE FROM Parceiro WHERE IDFor=@IDFor", conexao);
                cmd.Parameters.Add("@IDFor", MySqlDbType.Int32).Value = Id;
                cmd.ExecuteNonQuery();
                conexao.Close();
            }
        }


        public Parceiro ObterParceiro(int Id)
        {
            using (var conexao = new MySqlConnection(_conexaoMySQL))
            {
                conexao.Open();

                MySqlCommand cmd = new MySqlCommand("SELECT * FROM Parceiro WHERE IDFor=@IDFor", conexao);
                cmd.Parameters.AddWithValue("@IDFor", Id);

                Parceiro parceiro = new Parceiro();

                using (MySqlDataReader dr = cmd.ExecuteReader(CommandBehavior.CloseConnection))
                {
                    while (dr.Read())
                    {
                        parceiro.IDFor = (Int32)(dr["IDFor"]);
                        parceiro.nomeFor = (string)(dr["nomeFor"]);
                        parceiro.CNPJFor = (string)(dr["CNPJFor"]);
                        parceiro.emailFor = (string)(dr["emailFor"]);
                        parceiro.situacaoFor = (string)(dr["situacaoFor"]);
                    }
                }

                return parceiro;
            }
        }

        public IEnumerable<Parceiro> ObterTodosParceiros()
        {
            List<Parceiro> parceiroList = new List<Parceiro>();
            using (var conexao = new MySqlConnection(_conexaoMySQL))
            {
                conexao.Open();

                MySqlCommand cmd = new MySqlCommand(@"SELECT * FROM Parceiro", conexao);
                MySqlDataAdapter da = new MySqlDataAdapter(cmd);
                DataTable dt = new DataTable();

                da.Fill(dt);

                conexao.Close();
                foreach (DataRow dr in dt.Rows)
                {
                    parceiroList.Add(
                        new Parceiro
                        {
                            IDFor = Convert.ToInt32(dr["IDFor"]),
                            nomeFor = (string)(dr["nomeFor"]),
                            CNPJFor = (string)(dr["CNPJFor"]),
                            emailFor = (string)(dr["emailFor"]),
                            situacaoFor = (string)(dr["situacaoFor"])
                        }
                    );
                }
                return parceiroList;
            }
        }
    }
}
