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

        public void Ativar(string cnpj)
        {
            string situacao = SituacaoParceiroConstant.Ativo;

            using (var conexao = new MySqlConnection(_conexaoMySQL))
            {
                conexao.Open();
                MySqlCommand cmd = new MySqlCommand(@"UPDATE Parceiro SET situacaoFor=@situacaoFor WHERE CNPJFor=@CNPJFor", conexao);
                cmd.Parameters.Add("@situacaoFor", MySqlDbType.VarChar).Value = situacao;
                cmd.Parameters.Add("@CNPJFor", MySqlDbType.VarChar).Value = cnpj;
                cmd.ExecuteNonQuery();
                conexao.Close();
            }
        }

        public void Atualizar(Parceiro parceiro, string cnpjOriginal)
        {
            using (var conexao = new MySqlConnection(_conexaoMySQL))
            {
                conexao.Open();

                MySqlCommand cmd = new MySqlCommand(@"UPDATE Parceiro SET nomeFor=@nomeFor, CNPJFor=@CNPJFor, emailFor=@emailFor, situacaoFor=@situacaoFor
                WHERE CNPJFor=@cnpjOriginal", conexao);

                cmd.Parameters.Add("@nomeFor", MySqlDbType.VarChar).Value = parceiro.nomeFor;
                cmd.Parameters.Add("@CNPJFor", MySqlDbType.VarChar).Value = parceiro.CNPJFor;
                cmd.Parameters.Add("@cnpjOriginal", MySqlDbType.VarChar).Value = cnpjOriginal;
                cmd.Parameters.Add("@emailFor", MySqlDbType.VarChar).Value = parceiro.emailFor;
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

                string cnpj = System.Text.RegularExpressions.Regex.Replace(parceiro.CNPJFor, @"\D", "");

                MySqlCommand cmd = new MySqlCommand(@"INSERT INTO Parceiro(nomeFor, CNPJFor, emailFor, situacaoFor)
                        VALUES (@nomeFor, @CNPJFor, @emailFor, @situacaoFor)", conexao);

                cmd.Parameters.Add("@nomeFor", MySqlDbType.VarChar).Value = parceiro.nomeFor;
                cmd.Parameters.Add("@CNPJFor", MySqlDbType.VarChar).Value = cnpj;
                cmd.Parameters.Add("@emailFor", MySqlDbType.VarChar).Value = parceiro.emailFor;
                cmd.Parameters.Add("@situacaoFor", MySqlDbType.VarChar).Value = parceiro.situacaoFor;

                cmd.ExecuteNonQuery();
                conexao.Close();
            }
        }

        public void Desativar(string cnpj)
        {
            string situacao = SituacaoParceiroConstant.Inativo;

            using (var conexao = new MySqlConnection(_conexaoMySQL))
            {
                conexao.Open();
                MySqlCommand cmd = new MySqlCommand(@"UPDATE Parceiro SET situacaoFor=@situacaoFor WHERE CNPJFor=@CNPJFor", conexao);
                cmd.Parameters.Add("@situacaoFor", MySqlDbType.VarChar).Value = situacao;
                cmd.Parameters.Add("@CNPJFor", MySqlDbType.VarChar).Value = cnpj;
                cmd.ExecuteNonQuery();
                conexao.Close();
            }
        }

        public void Excluir(string cnpj)
        {
            using (var conexao = new MySqlConnection(_conexaoMySQL))
            {
                conexao.Open();
                MySqlCommand cmd = new MySqlCommand(@"DELETE FROM Parceiro WHERE CNPJFor=@CNPJFor", conexao);
                cmd.Parameters.Add("@CNPJFor", MySqlDbType.VarChar).Value = cnpj;
                cmd.ExecuteNonQuery();
                conexao.Close();
            }
        }


        public Parceiro ObterParceiro(string cnpj)
        {
            using (var conexao = new MySqlConnection(_conexaoMySQL))
            {
                conexao.Open();

                MySqlCommand cmd = new MySqlCommand("SELECT * FROM Parceiro WHERE CNPJFor=@CNPJFor", conexao);
                cmd.Parameters.AddWithValue("@CNPJFor", cnpj);

                Parceiro parceiro = new Parceiro();

                using (MySqlDataReader dr = cmd.ExecuteReader(CommandBehavior.CloseConnection))
                {
                    while (dr.Read())
                    {
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
