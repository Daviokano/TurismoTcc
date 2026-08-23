using HorizonTravel.Models;
using HorizonTravel.Repository.Contract;
using Microsoft.AspNetCore.Mvc;
using MySql.Data.MySqlClient;
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

        public void Atualizar(Usuario usuario)
        {
            throw new NotImplementedException();
        }

        public void Cadastrar(Usuario usuario)
        {
            throw new NotImplementedException();
        }

        public void Excluir(Usuario usuario)
        {
            throw new NotImplementedException();
        }

        //pesquisar o erro depois
        public Usuario Login(string Email, string Senha)
        {
            using (var conexao = new MySqlConnection(_conexaoMySQL))
            {
                conexao.Open();

                MySqlCommand cmd = new MySqlCommand("select * from Usuario where emailUsu = @emailUsu and senhaUsu = @senhaUsu", conexao);

                cmd.Parameters.Add("@emailusu", MySqlDbType.VarChar).Value = Email;
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
                    usuario.enderecoUsu = Convert.ToString(dr["enderecoUsu"]);
                    usuario.dataNascismento = Convert.ToDateTime(dr["dataNascimento"]);
                    usuario.senhaUsu = Convert.ToString(dr["senhaUsu"]);
                }
                return usuario;
            }
        }

        public IEnumerable<Usuario> ObterTodosUsuarios()
        {
            List<Usuario> usuList = new List<Usuario>();
            using(var conexao = new MySqlConnection(_conexaoMySQL))
            {
                conexao.Open();
                MySqlCommand cmd = new MySqlCommand("select * from Usuario", conexao);

                MySqlDataAdapter da = new MySqlDataAdapter(cmd);

                DataTable dt = new DataTable();

                da.Fill(dt);

                conexao.Close();

                foreach(DataRow dr in dt.Rows)
                {
                    usuList.Add(
                        new Usuario
                        {
                            IDUsu = 
                        });
                }
                return usuList;
            }
        }

        public Usuario ObterUsuario(int Id)
        {
            throw new NotImplementedException();
        }
    }
}
