using HorizonTravel.Models;
using HorizonTravel.Models.Constant;
using HorizonTravel.Repository.Contract;
using MySql.Data.MySqlClient;
using System.Data;

namespace HorizonTravel.Repository
{
    public class FuncionarioRepository : IFuncionarioRepositoty
    {
        private readonly string _conexaoMySQL;

        public FuncionarioRepository(IConfiguration conf)
        {
            _conexaoMySQL = conf.GetConnectionString("ConexaoMySQL");
        }

        public void AlterarSituacao(int id, string situacao)
        {
            using (var conexao = new MySqlConnection(_conexaoMySQL))
            {
                conexao.Open();

                MySqlCommand cmd = new MySqlCommand("UPDATE Funcionario SET situacaoFun = @situacaoFun WHERE IDFun = @IDFun", conexao);
                cmd.Parameters.AddWithValue("@situacaoFun", situacao);
                cmd.Parameters.AddWithValue("@IDFun", id);
                cmd.ExecuteNonQuery();
            }
        }

        public void Ativar(int Id)
        {
            string situacao = SituacaoFunConstant.Ativo;

            using (var conexao = new MySqlConnection(_conexaoMySQL))
            {
                conexao.Open();
                MySqlCommand cmd = new MySqlCommand(@"UPDATE Funcionario SET situacaoFun=@situacaoFun WHERE IDFun=@IDFun", conexao);
                cmd.Parameters.Add("@situacaoFun", MySqlDbType.VarChar).Value = situacao;
                cmd.Parameters.Add("@IDFun", MySqlDbType.Int32).Value = Id;
                cmd.ExecuteNonQuery();
                conexao.Close();
            }
        }

        public void Atualizar(Funcionario funcionario)
        {
            using (var conexao = new MySqlConnection(_conexaoMySQL))
            {
                conexao.Open();

                MySqlCommand cmd = new MySqlCommand(@"UPDATE Funcionario SET nomeFun=@nomeFun, CPFFun=@CPFFun, emailFun=@emailFun, 
                telefoneFun=@telefoneFun, dataNascimentoFun=@dataNascimentoFun, senhaFun=@senhaFun, permissaoFun=@permissaoFun WHERE IDFun=@IDFun", conexao);

                string cpfLimpo = Regex.Replace(funcionario.CPFFun ?? "", @"\D", "");

                cmd.Parameters.Add("@nomeFun", MySqlDbType.VarChar).Value = funcionario.nomeFun;
                cmd.Parameters.Add("@CPFFun", MySqlDbType.VarChar).Value = cpfLimpo;
                cmd.Parameters.Add("@emailFun", MySqlDbType.VarChar).Value = funcionario.emailFun;
                cmd.Parameters.Add("@telefoneFun", MySqlDbType.VarChar).Value = funcionario.telefoneFun;
                cmd.Parameters.Add("@dataNascimentoFun", MySqlDbType.VarChar).Value = funcionario.dataNascimentoFun.ToString("yyyy-MM-dd");
                cmd.Parameters.Add("@senhaFun", MySqlDbType.VarChar).Value = funcionario.senhaFun;
                cmd.Parameters.Add("@IDFun", MySqlDbType.Int32).Value = funcionario.IDFun;
                cmd.Parameters.Add("@permissaoFun", MySqlDbType.VarChar).Value = funcionario.permissaoFun;

                cmd.ExecuteNonQuery();
                conexao.Close();
            }
        }

        public void Cadastrar(Funcionario funcionario)
        {
            using (var conexao = new MySqlConnection(_conexaoMySQL))
            {
                conexao.Open();

                string cpf = System.Text.RegularExpressions.Regex.Replace(funcionario.CPFFun, @"\D", "");

                MySqlCommand cmd = new MySqlCommand(@"INSERT INTO Funcionario(nomeFun, CPFFun, emailFun, telefoneFun, dataNascimentoFun, senhaFun, permissaoFun, situacaoFun)
                    VALUES (@nomeFun, @CPFFun, @emailFun, @telefoneFun, @dataNascimentoFun, @senhaFun, @permissaoFun, @situacaoFun)", conexao);

                cmd.Parameters.Add("@nomeFun", MySqlDbType.VarChar).Value = funcionario.nomeFun;
                cmd.Parameters.Add("@CPFFun", MySqlDbType.VarChar).Value = cpf;
                cmd.Parameters.Add("@emailFun", MySqlDbType.VarChar).Value = funcionario.emailFun;
                cmd.Parameters.Add("@telefoneFun", MySqlDbType.VarChar).Value = funcionario.telefoneFun;
                cmd.Parameters.Add("@dataNascimentoFun", MySqlDbType.VarChar).Value = funcionario.dataNascimentoFun.ToString("yyyy-MM-dd");
                cmd.Parameters.Add("@senhaFun", MySqlDbType.VarChar).Value = funcionario.senhaFun;
                cmd.Parameters.Add("@permissaoFun", MySqlDbType.VarChar).Value = PermissaoFunConstant.Funcionario;
                cmd.Parameters.Add("@situacaoFun", MySqlDbType.VarChar).Value = funcionario.situacaoFun;


                cmd.ExecuteNonQuery();
                conexao.Close();
            }
        }

        public int ContarAdministradoresAtivos()
        {
            using (var conexao = new MySqlConnection(_conexaoMySQL))
            {
                conexao.Open();

                MySqlCommand cmd = new MySqlCommand("SELECT COUNT(*) FROM Funcionario WHERE permissaoFun = @permissaoFun AND situacaoFun = 'A'", conexao);
                cmd.Parameters.AddWithValue("@permissaoFun", PermissaoFunConstant.Administrador);
                return Convert.ToInt32(cmd.ExecuteScalar());
            }
        }

        public void Desativar(int Id)
        {
            string situacao = SituacaoFunConstant.Inativo;

            using (var conexao = new MySqlConnection(_conexaoMySQL))
            {
                conexao.Open();
                MySqlCommand cmd = new MySqlCommand(@"UPDATE Funcionario SET situacaoFun=@situacaoFun WHERE IDFun=@IDFun", conexao);
                cmd.Parameters.Add("@situacaoFun", MySqlDbType.VarChar).Value = situacao;
                cmd.Parameters.Add("@IDFun", MySqlDbType.Int32).Value = Id;
                cmd.ExecuteNonQuery();
                conexao.Close();
            }
        }

        public void Excluir(int Id)
        {
            using (var conexao = new MySqlConnection(_conexaoMySQL))
            {
                conexao.Open();

                MySqlCommand cmd = new MySqlCommand("DELETE FROM Funcionario WHERE IDFun=@IDFun", conexao);
                cmd.Parameters.AddWithValue("@IDFun", Id);
                int i = cmd.ExecuteNonQuery();
                conexao.Close();
            }
        }

        public Funcionario Login(string Email, string Senha)
        {
            using (var conexao = new MySqlConnection(_conexaoMySQL))
            {
                conexao.Open();

                MySqlCommand cmd = new MySqlCommand(@"SELECT * FROM Funcionario WHERE emailFun=@emailFun AND senhaFun=@senhaFun", conexao);

                cmd.Parameters.Add("@emailFun", MySqlDbType.VarChar).Value = Email;
                cmd.Parameters.Add("@senhaFun", MySqlDbType.VarChar).Value = Senha;

                MySqlDataAdapter da = new MySqlDataAdapter(cmd);
                MySqlDataReader dr;

                Funcionario funcionario = new Funcionario();

                dr = cmd.ExecuteReader(CommandBehavior.CloseConnection);

                while (dr.Read())
                {
                    funcionario.IDFun = Convert.ToInt32(dr["IDFun"]);
                    funcionario.nomeFun = Convert.ToString(dr["nomeFun"]);
                    funcionario.CPFFun = Convert.ToString(dr["CPFFun"]);
                    funcionario.emailFun = Convert.ToString(dr["emailFun"]);
                    funcionario.telefoneFun = Convert.ToString(dr["telefoneFun"]);
                    funcionario.dataNascimentoFun = Convert.ToDateTime(dr["dataNascimentoFun"]);
                    funcionario.senhaFun = Convert.ToString(dr["senhaFun"]);
                    funcionario.permissaoFun = Convert.ToString(dr["permissaoFun"]);
                    funcionario.situacaoFun = Convert.ToString(dr["situacaoFun"]);
                }
                return funcionario;
            }
        }

        public Funcionario ObterFuncionario(int Id)
        {
            using (var conexao = new MySqlConnection(_conexaoMySQL))
            {
                conexao.Open();

                MySqlCommand cmd = new MySqlCommand("SELECT * FROM Funcionario WHERE IDFun=@IDFun", conexao);
                cmd.Parameters.AddWithValue("@IDFun", Id);

                Funcionario funcionario = new Funcionario();

                using (MySqlDataReader dr = cmd.ExecuteReader(CommandBehavior.CloseConnection))
                {
                    while (dr.Read())
                    {
                        funcionario.IDFun = (Int32)(dr["IDFun"]);
                        funcionario.nomeFun = (string)(dr["nomeFun"]);
                        funcionario.CPFFun = (string)(dr["CPFFun"]);
                        funcionario.emailFun = (string)(dr["emailFun"]);
                        funcionario.telefoneFun = (string)(dr["telefoneFun"]);
                        funcionario.dataNascimentoFun = (DateTime)(dr["dataNascimentoFun"]);
                        funcionario.senhaFun = (string)(dr["senhaFun"]);
                        funcionario.permissaoFun = (string)(dr["permissaoFun"]);
                        funcionario.situacaoFun = (string)(dr["situacaoFun"]);
                    }
                }

                return funcionario;
            }
        }

        public IEnumerable<Funcionario> ObterTodosFuncionarios()
        {
            List<Funcionario> funList = new List<Funcionario>();
            using (var conexao = new MySqlConnection(_conexaoMySQL))
            {
                conexao.Open();

                MySqlCommand cmd = new MySqlCommand(@"SELECT * FROM Funcionario", conexao);
                MySqlDataAdapter da = new MySqlDataAdapter(cmd);
                DataTable dt = new DataTable();

                da.Fill(dt);

                conexao.Close();
                foreach (DataRow dr in dt.Rows)
                {
                    funList.Add(
                        new Funcionario
                        {
                            IDFun = Convert.ToInt32(dr["IDFun"]),
                            nomeFun = (string)(dr["nomeFun"]),
                            CPFFun = (string)(dr["CPFFun"]),
                            emailFun = (string)(dr["emailFun"]),
                            telefoneFun = (string)(dr["telefoneFun"]),
                            dataNascimentoFun = (DateTime)(dr["dataNascimentoFun"]),
                            senhaFun = (string)(dr["senhaFun"]),
                            permissaoFun = (string)(dr["permissaoFun"]),
                            situacaoFun = (string)(dr["situacaoFun"]),
                        }
                    );
                }
                return funList;
            }
        }
    }
}
