using HorizonTravel.Models;
using HorizonTravel.Models.Constant;
using HorizonTravel.Repository.Contract;
using Microsoft.AspNetCore.Mvc;
using MySql.Data.MySqlClient;
using Mysqlx.Crud;
using Org.BouncyCastle.Crypto;
using System.Data;

namespace HorizonTravel.Repository
{
    public class UsuarioRepository : IUsuarioRepository
    {
        private readonly string _conexaoMySQL;

        public UsuarioRepository(IConfiguration conf)
        {
            _conexaoMySQL = conf.GetConnectionString("ConexaoMySQL");
        }

        public void Ativar(int Id)
        {
            string situacao = SituacaoUsuConstant.Ativo;

            using(var conexao = new MySqlConnection(_conexaoMySQL))
            {
                conexao.Open();
                MySqlCommand cmd = new MySqlCommand(@"UPDATE Usuario SET situacaoUsu=@situacaoUsu WHERE IDUsu=@IDUsu", conexao);
                cmd.Parameters.Add("@situacaoUsu", MySqlDbType.VarChar).Value = situacao;
                cmd.Parameters.Add("@IDUsu", MySqlDbType.Int32).Value = Id;
                cmd.ExecuteNonQuery();
                conexao.Close();
            }
        }

        public void Atualizar(Usuario usuario)
        {
            using (var conexao = new MySqlConnection(_conexaoMySQL))
            {
                conexao.Open();

                MySqlCommand cmd = new MySqlCommand(@"UPDATE Usuario SET nomeUsu=@nomeUsu, CPFUsu=@CPFUsu, emailUsu=@emailUsu, 
                telefoneUsu=@telefoneUsu, dataNascimentoUsu=@dataNascimentoUsu, senhaUsu=@senhaUsu WHERE IDUsu=@IDUsu");

                cmd.Parameters.Add("@nomeUsu", MySqlDbType.VarChar).Value = usuario.nomeUsu;
                cmd.Parameters.Add("@CPFUsu", MySqlDbType.VarChar).Value = usuario.CPFUsu;
                cmd.Parameters.Add("@emailUsu", MySqlDbType.VarChar).Value = usuario.emailUsu;
                cmd.Parameters.Add("@telefoneUsu", MySqlDbType.VarChar).Value = usuario.telefoneUsu;
                cmd.Parameters.Add("@dataNascimentoUsu", MySqlDbType.DateTime).Value = usuario.dataNascimentoUsu;
                cmd.Parameters.Add("@situacaoUsu", MySqlDbType.VarChar).Value = usuario.situacaoUsu;
                cmd.Parameters.Add("@senhaUsu", MySqlDbType.VarChar).Value = usuario.senhaUsu;

                cmd.ExecuteNonQuery();
                conexao.Close();
            }
        }

        public void Cadastrar(Usuario usuario)
        {
            using (var conexao = new MySqlConnection(_conexaoMySQL))
            {
                conexao.Open();

                MySqlCommand cmd = new MySqlCommand(@"INSERT INTO Usuario(nomeUsu, CPFUsu, emailUsu, telefoneUsu, dataNascimentoUsu, senhaUsu, situacaoUsu)
                VALUES (@nomeUsu, @CPFUsu, @emailUsu, @telefoneUsu, @dataNascimentoUsu, @senhaUsu, @situacaoUsu)", conexao);


                cmd.Parameters.Add("@nomeUsu", MySqlDbType.VarChar).Value = usuario.nomeUsu;
                cmd.Parameters.Add("@CPFUsu", MySqlDbType.VarChar).Value = usuario.CPFUsu;
                cmd.Parameters.Add("@emailUsu", MySqlDbType.VarChar).Value = usuario.emailUsu;
                cmd.Parameters.Add("@telefoneUsu", MySqlDbType.VarChar).Value = usuario.telefoneUsu;
                cmd.Parameters.Add("@dataNascimentoUsu", MySqlDbType.DateTime).Value = usuario.dataNascimentoUsu;
                cmd.Parameters.Add("@senhaUsu", MySqlDbType.VarChar).Value = usuario.senhaUsu;
                cmd.Parameters.Add("@situacaoUsu", MySqlDbType.VarChar).Value = usuario.situacaoUsu;

                cmd.ExecuteNonQuery();
                conexao.Close();
            }
        }

        public void Desativar(int Id)
        {
            string situacao = SituacaoUsuConstant.Inativo;

            using (var conexao = new MySqlConnection(_conexaoMySQL))
            {
                conexao.Open();
                MySqlCommand cmd = new MySqlCommand(@"UPDATE Usuario SET situacaoUsu=@situacaoUsu WHERE IDUsu=@IDUsu", conexao);
                cmd.Parameters.Add("@situacaoUsu", MySqlDbType.VarChar).Value = situacao;
                cmd.Parameters.Add("@IDUsu", MySqlDbType.Int32).Value = Id;
                cmd.ExecuteNonQuery();
                conexao.Close();
            }
        }

        public void Excluir(int Id)
        {
            using (var conexao = new MySqlConnection(_conexaoMySQL))
            {
                conexao.Open();
                MySqlCommand cmd = new MySqlCommand("DELETE * FROM Usuario WHERE IDUsu=@IDUsu", conexao);
                cmd.Parameters.AddWithValue("@IDUsu", Id);
                int i = cmd.ExecuteNonQuery();

                conexao.Close();
            }
        }

        public Usuario Login(string Email, string Senha)
        {
            using (var conexao = new MySqlConnection(_conexaoMySQL))
            {
                conexao.Open();

                MySqlCommand cmd = new MySqlCommand(@" 
                SELECT IDUsu, CPFUsu, nomeUsu, emailUsu, telefoneUsu, dataNascimentoUsu, senhaUsu, situacaoUsu
                FROM Usuario
                WHERE emailUsu = @emailUsu
                AND senhaUsu = @senhaUsu", conexao);

                cmd.Parameters.Add("@emailUsu", MySqlDbType.VarChar).Value = Email;
                cmd.Parameters.Add("@senhaUsu", MySqlDbType.VarChar).Value = Senha;

                MySqlDataAdapter da = new MySqlDataAdapter(cmd);
                MySqlDataReader dr;

                Usuario usuario = new Usuario();
                dr = cmd.ExecuteReader(CommandBehavior.CloseConnection);

                while (dr.Read())
                {
                    usuario.IDUsu = Convert.ToInt32(dr["IDUsu"]);
                    usuario.CPFUsu = Convert.ToString(dr["CPFUsu"]);
                    usuario.nomeUsu = Convert.ToString(dr["nomeUsu"]);
                    usuario.emailUsu = Convert.ToString(dr["emailUsu"]);
                    usuario.telefoneUsu = Convert.ToString(dr["telefoneUsu"]);
                    usuario.dataNascimentoUsu = Convert.ToDateTime(dr["dataNascimentoUsu"]);
                    usuario.senhaUsu = Convert.ToString(dr["senhaUsu"]);
                    usuario.situacaoUsu = Convert.ToString(dr["situacaoUsu"]);
				}
				return usuario;
            }
        }


        public IEnumerable<Usuario> ObterTodosUsuarios()
        {
            List<Usuario> usuList = new List<Usuario>();
            using (var conexao = new MySqlConnection(_conexaoMySQL))
            {
                conexao.Open();

                MySqlCommand cmd = new MySqlCommand(@"
                SELECT IDUsu, CPFUsu, nomeUsu, emailUsu, telefoneUsu, dataNascimentoUsu, senhaUsu, situacaoUsu
                FROM Usuario", conexao);

                MySqlDataAdapter da = new MySqlDataAdapter(cmd);
                DataTable dt = new DataTable();
                da.Fill(dt);

                conexao.Close();

                foreach (DataRow dr in dt.Rows)
                {
                    usuList.Add(
                        new Usuario
                        {
                            IDUsu = (Int32)(dr["IDUsu"]),
                            nomeUsu = (string)(dr["nomeUsu"]),
                            CPFUsu = (string)(dr["CPFUsu"]),
                            emailUsu = (string)(dr["emailUsu"]),
                            telefoneUsu = (string)(dr["telefoneUsu"]),
                            dataNascimentoUsu = (DateTime)(dr["dataNascimentoUsu"]),
                            senhaUsu = (string)(dr["senhaUsu"]),
                            situacaoUsu = Convert.ToString(dr["situacaoUsu"]),
                        }
                    );
                }
                return usuList;
            }
        }

        public Usuario ObterUsuario(int Id)
        {
            using (var conexao = new MySqlConnection(_conexaoMySQL))
            {
                conexao.Open();

                MySqlCommand cmd = new MySqlCommand(@"
                SELECT IDUsu, CPFUsu, nomeUsu, emailUsu, telefoneUsu, dataNascimentoUsu, senhaUsu, situacaoUsu
                FROM Usuario
                WHERE IDUsu = @IDUsu", conexao);
                cmd.Parameters.AddWithValue("@IDUsu", Id);

                Usuario usuario = new Usuario();
                using (MySqlDataReader dr = cmd.ExecuteReader(CommandBehavior.CloseConnection))
                {
                    while (dr.Read())
                    {
                        usuario.IDUsu = (Int32)(dr["IDUsu"]);
                        usuario.nomeUsu = (string)(dr["nomeUsu"]);
                        usuario.CPFUsu = (string)(dr["CPFUsu"]);
                        usuario.emailUsu = (string)(dr["emailUsu"]);
                        usuario.telefoneUsu = (string)(dr["telefoneUsu"]);
                        usuario.dataNascimentoUsu = (DateTime)(dr["dataNascimentoUsu"]);
                        usuario.senhaUsu = (string)(dr["senhaUsu"]);
                        usuario.situacaoUsu = Convert.ToString(dr["situacaoUsu"]);
                    }
                }
                return usuario;
            }
        }
    }
}
