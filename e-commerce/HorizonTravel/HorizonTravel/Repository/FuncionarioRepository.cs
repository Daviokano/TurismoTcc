using HorizonTravel.Models;
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

        public void Atualizar(Funcionario funcionario)
        {
            using (var conexao = new MySqlConnection(_conexaoMySQL))
            {
                conexao.Open();

                MySqlCommand cmd = new MySqlCommand(@"UPDATE Funcionario SET nomeFun=@nomeFun, CPFFun=@CPFFun, emailFun=@emailFun, 
                telefoneFun=@telefoneFun, dataNascimentoFun=@dataNascimentoFun, senhaUsu=@senhaFun WHERE IDFun=@IDFun");

                cmd.Parameters.Add("@nomeFun", MySqlDbType.VarChar).Value = funcionario.nomeFun;
                cmd.Parameters.Add("@CPFFun", MySqlDbType.VarChar).Value = funcionario.CPFFun;
                cmd.Parameters.Add("@emailFun", MySqlDbType.VarChar).Value = funcionario.emailFun;
                cmd.Parameters.Add("@telefoneFun", MySqlDbType.VarChar).Value = funcionario.telefoneFun;
                cmd.Parameters.Add("@dataNascimentoFun", MySqlDbType.VarChar).Value = funcionario.dataNascimentoFun;
                cmd.Parameters.Add("@senhaFun", MySqlDbType.VarChar).Value = funcionario.senhaFun;
                

                cmd.ExecuteNonQuery();
                conexao.Close();
            }
        }

        public void Cadastrar(Funcionario funcionario)
        {
            using (var conexao = new MySqlConnection(_conexaoMySQL))
            {
                conexao.Open();

                MySqlCommand cmd = new MySqlCommand(@"INSERT INTO Funcionario(nomeFun, CPFFun, emailFun, emailFun, dataNascimentoFun, senhaFun)
                VALUES (@nomeFun, @CPFFun, @emailFun, @telefoneFun, @dataNascimentoFun, @senhaFun)", conexao);

                cmd.Parameters.Add("@nomeFun", MySqlDbType.VarChar).Value = funcionario.nomeFun;
                cmd.Parameters.Add("@CPFFun", MySqlDbType.VarChar).Value = funcionario.CPFFun;
                cmd.Parameters.Add("@emailFun", MySqlDbType.VarChar).Value = funcionario.emailFun;
                cmd.Parameters.Add("@telefoneFun", MySqlDbType.VarChar).Value = funcionario.telefoneFun;
                cmd.Parameters.Add("@dataNascimentoFun", MySqlDbType.VarChar).Value = funcionario.dataNascimentoFun;
                cmd.Parameters.Add("@senhaFun", MySqlDbType.VarChar).Value = funcionario.senhaFun;


                cmd.ExecuteNonQuery();
                conexao.Close();
            }
        }

        public void Excluir(int Id)
        {
            using (var conexao = new MySqlConnection(_conexaoMySQL))
            {
                conexao.Open();

                MySqlCommand cmd = new MySqlCommand("DELETE * FROM Funcionario WHERE IDFun=@IDFun", conexao);
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
                    funcionario.senhaFun = Convert.ToInt32(dr["senhaFun"]);
                }
                return funcionario;
            }
        }

        //Terminar depois
        public Funcionario ObterFuncionario(int Id)
        {
            using (var conexao = new MySqlConnection(_conexaoMySQL))
            {
                conexao.Open();
                MySqlCommand cmd = new MySqlCommand(@"SELECT * FROM Funcionario WHERE emailFun=@emailFun AND senhaFun=@senhaFun", conexao);
                cmd.Parameters.AddWithValue("IDFun", Id);
               
                MySqlDataAdapter da = new MySqlDataAdapter(cmd);
                MySqlDataReader dr;

                Funcionario funcionario = new Funcionario();
                dr = cmd.ExecuteReader(CommandBehavior.CloseConnection);
                while (dr.Read())
                {
                    //@nomeFun, @CPFFun, @emailFun, @telefoneFun, @dataNascimentoFun, @senhaFun
                    funcionario.IDFun = (Int32)(dr)
                }


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
                        }
                    );
                }
            }
        }
    }
}
