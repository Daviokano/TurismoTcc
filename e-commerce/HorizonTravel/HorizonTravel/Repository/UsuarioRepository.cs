using HorizonTravel.Models;
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

        public void Atualizar(Usuario usuario)
        {
            using (var conexao = new MySqlConnection(_conexaoMySQL))
            {
                conexao.Open();

                MySqlCommand cmdEndereco = new MySqlCommand(@"UPDATE Endereco (logEnd=@logEnd, numEnd=@numEnd, bairroEnd=@bairroEnd, cidEnd=@cidEnd,
                estEnd=@estEnd, CEPEnd=@CEPEnd WHERE IDEnd=@IDEnd)", conexao);

                cmdEndereco.Parameters.Add("@logEnd", MySqlDbType.VarChar).Value = usuario.Endereco.logEnd;
                cmdEndereco.Parameters.Add("@numEnd", MySqlDbType.VarChar).Value = usuario.Endereco.numEnd;
                cmdEndereco.Parameters.Add("@bairroEnd", MySqlDbType.VarChar).Value = usuario.Endereco.bairroEnd;
                cmdEndereco.Parameters.Add("@cidEnd", MySqlDbType.VarChar).Value = usuario.Endereco.cidEnd;
                cmdEndereco.Parameters.Add("@estEnd", MySqlDbType.VarChar).Value = usuario.Endereco.estEnd;
                cmdEndereco.Parameters.Add("@CEPEnd", MySqlDbType.VarChar).Value = usuario.Endereco.CEPEnd;
                cmdEndereco.Parameters.Add("@IDEnd", MySqlDbType.VarChar).Value = usuario.Endereco.IDEnd;

                cmdEndereco.ExecuteNonQuery();

                MySqlCommand cmd = new MySqlCommand(@"UPDATE Usuario SET nomeUsu=@nomeUsu, CPFUsu=@CPFUsu, emailUsu=@emailUsu, 
                    telefoneUsu=@telefoneUsu, dataNascimentoUsu=@dataNascimentoUsu, senhaUsu=@senhaUsu WHERE IDUsu=@IDUsu");

                cmd.Parameters.Add("@nomeUsu", MySqlDbType.VarChar).Value = usuario.nomeUsu;
                cmd.Parameters.Add("@CPFUsu", MySqlDbType.VarChar).Value = usuario.CPFUsu;
                cmd.Parameters.Add("@emailUsu", MySqlDbType.VarChar).Value = usuario.emailUsu;
                cmd.Parameters.Add("@telefoneUsu", MySqlDbType.VarChar).Value = usuario.telefoneUsu;
                cmd.Parameters.Add("@dataNascismentoUsu", MySqlDbType.DateTime).Value = usuario.dataNascismentoUsu;
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

                MySqlCommand cmdEndereco = new MySqlCommand(@"INSERT INTO Endereco (logEnd, numEnd, bairroEnd, cidEnd, estEnd, CEPEnd)
                VALUES (@logEnd, @numEnd, @bairroEnd, @cidEnd, @estEnd, @CEPEnd)", conexao);

                cmdEndereco.Parameters.Add("@logEnd", MySqlDbType.VarChar).Value = usuario.Endereco.logEnd;
                cmdEndereco.Parameters.Add("@numEnd", MySqlDbType.VarChar).Value = usuario.Endereco.numEnd;
                cmdEndereco.Parameters.Add("@bairroEnd", MySqlDbType.VarChar).Value = usuario.Endereco.bairroEnd;
                cmdEndereco.Parameters.Add("@cidEnd", MySqlDbType.VarChar).Value = usuario.Endereco.cidEnd;
                cmdEndereco.Parameters.Add("@estEnd", MySqlDbType.VarChar).Value = usuario.Endereco.estEnd;
                cmdEndereco.Parameters.Add("@CEPEnd", MySqlDbType.VarChar).Value = usuario.Endereco.CEPEnd;

                cmdEndereco.ExecuteNonQuery();

                // Pega o id do endereço que acabou de ser cadastrado, tipo, endereço tal tem id=15, ele pega o id 15
                int IDEnd = Convert.ToInt32(cmdEndereco.LastInsertedId);

                MySqlCommand cmd = new MySqlCommand(@"INSERT INTO Usuario(nomeUsu, CPFUsu, emailUsu, telefoneUsu, dataNascimentoUsu, senhaUsu)
                VALUES (@nomeUsu, @CPFUsu, @emailUsu, @telefoneUsu, @dataNascimentoUsu, @senhaUsu)", conexao);


                cmd.Parameters.Add("@nomeUsu", MySqlDbType.VarChar).Value = usuario.nomeUsu;
                cmd.Parameters.Add("@CPFUsu", MySqlDbType.VarChar).Value = usuario.CPFUsu;
                cmd.Parameters.Add("@emailUsu", MySqlDbType.VarChar).Value = usuario.emailUsu;
                cmd.Parameters.Add("@telefoneUsu", MySqlDbType.VarChar).Value = usuario.telefoneUsu;
                cmd.Parameters.Add("@dataNascismentoUsu", MySqlDbType.DateTime).Value = usuario.dataNascismentoUsu;
                cmd.Parameters.Add("@senhaUsu", MySqlDbType.VarChar).Value = usuario.senhaUsu;
                cmd.Parameters.Add("@IDEnd", MySqlDbType.Int32).Value = IDEnd;

                cmd.ExecuteNonQuery();
                conexao.Close();
            }
        }

        public void Excluir(int Id)
        {
            using (var conexao = new MySqlConnection(_conexaoMySQL))
            {
                conexao.Open();

                //pegando endereço do usuário
                MySqlCommand cmdEndereco = new MySqlCommand("SELECT IDEnd FROM Usuario WHERE IDUsu = @IDUsu", conexao);
                cmdEndereco.Parameters.AddWithValue("@IDUsu", Id);
                //guardando o id do endereço
                int IDEnd = Convert.ToInt32(cmdEndereco.ExecuteScalar());

                MySqlCommand cmd = new MySqlCommand("DELETE * FROM Usuario WHERE IDUsu=@IDUsu", conexao);
                cmd.Parameters.AddWithValue("@IDUsu", Id);
                int i = cmd.ExecuteNonQuery();

                //deletando o endereço desse usuário
                MySqlCommand cmdExcluirEndereco = new MySqlCommand("DELETE FROM Endereco WHERE IDEnd = @IDEnd",conexao);

                conexao.Close();
            }
        }

        //pesquisar o erro depois
        public Usuario Login(string Email, string Senha)
        {
            using (var conexao = new MySqlConnection(_conexaoMySQL))
            {
                conexao.Open();

<<<<<<< HEAD
                MySqlCommand cmd = new MySqlCommand(@" 
                SELECT IDUsu, CPFUsu, nomeUsu, emailUsu, telefoneUsu, IDEnd, dataNascimentoUsu, senhaUsu, IDEnd
                FROM Usuario
                WHERE emailUsu = @emailUsu
                AND senhaUsu = @senhaUsu", conexao);

                cmd.Parameters.Add("@emailUsu", MySqlDbType.VarChar).Value = Email;
=======
                //mudar as váriaveis quando a palloma terminar de atualizar o banco
				MySqlCommand cmd = new MySqlCommand("SELECT u.*, e.IDEndereco, e.Rua, e.Numero, e.Cidade, e.Estado, e.CEP FROM Usuario u LEFT JOIN Endereco e ON u.idEndereco = e.IDEndereco WHERE u.emailUsu = @emailUsu AND u.senhaUsu = @senhaUsu", conexao);

				cmd.Parameters.Add("@emailusu", MySqlDbType.VarChar).Value = Email;
>>>>>>> a5f08bc4d6687722b9189c5ad78a9a3155867564
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
<<<<<<< HEAD
                    usuario.dataNascismentoUsu = Convert.ToDateTime(dr["dataNascimento"]);
                    usuario.senhaUsu = Convert.ToString(dr["senhaUsu"]);
                    usuario.IDEnd = Convert.ToInt32(dr["IDEnd"]);
                }
                return usuario;
=======
                    usuario.dataNascismento = Convert.ToDateTime(dr["dataNascimento"]);
                    usuario.senhaUsu = Convert.ToString(dr["senhaUsu"]);

					// parte do endereço
					usuario.enderecoUsu = new Endereco();

					usuario.enderecoUsu.enderecoId = Convert.ToInt32(dr["IDEndereco"]);
					usuario.enderecoUsu.Rua = Convert.ToString(dr["Rua"]);
					usuario.enderecoUsu.Numero = Convert.ToString(dr["Numero"]);
					usuario.enderecoUsu.Cidade = Convert.ToString(dr["Cidade"]);
					usuario.enderecoUsu.Estado = Convert.ToString(dr["Estado"]);
					usuario.enderecoUsu.CEP = Convert.ToString(dr["CEP"]);
				}
				return usuario;
>>>>>>> a5f08bc4d6687722b9189c5ad78a9a3155867564
            }
        }

        //terminar depois
        public IEnumerable<Usuario> ObterTodosUsuarios()
        {
            List<Usuario> usuList = new List<Usuario>();
            using(var conexao = new MySqlConnection(_conexaoMySQL))
            {
                conexao.Open();

                MySqlCommand cmd = new MySqlCommand(@" 
                SELECT u.IDUsu, u.CPFUsu, u.nomeUsu, u.emailUsu, u.telefoneUsu, u.IDEnd, u.dataNascimentoUsu, u.senhaUsu,e.IDEnd
                AS IDEnd, e.logEnd, e.numEnd, e.bairroEnd, e.cidEnd, e.estEnd, e.CEPEnd
                FROM Usuario u
                INNER JOIN Endereco e 
                ON u.IDEnd = e.IDEnd", conexao);

                MySqlDataAdapter da = new MySqlDataAdapter(cmd);

                DataTable dt = new DataTable();

                da.Fill(dt);

                conexao.Close();

                foreach(DataRow dr in dt.Rows)
                {
                    usuList.Add(
                        new Usuario
                        {
                            IDUsu = Convert.ToInt32(dr["IDUsu"]),
                            CPFUsu = Convert.ToString(dr["CPFUsu"]),
                            nomeUsu = Convert.ToString(dr["nomeUsu"]),
                            emailUsu = Convert.ToString(dr["emailUsu"]),
                            telefoneUsu = Convert.ToString(dr["telefoneUsu"]),
                            IDEnd = Convert.ToInt32(dr["IDEnd"]),
                            dataNascismentoUsu = Convert.ToDateTime(dr["dataNascimentoUsu"]),
                            senhaUsu = Convert.ToString(dr["senhaUsu"]),

                            Endereco = new Endereco
                            {
                                IDEnd = Convert.ToInt32(dr["IDEnd"]),
                                logEnd = Convert.ToString(dr["logEnd"]),
                                numEnd = Convert.ToString(dr["numEnd"]),
                                bairroEnd = Convert.ToString(dr["bairroEnd"]),
                                cidEnd = Convert.ToString(dr["cidEnd"]),
                                estEnd = Convert.ToString(dr["estEnd"]),
                                CEPEnd = Convert.ToString(dr["CEPEnd"])
                            }
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
                SELECT u.IDUsu, u.CPFUsu, u.nomeUsu, u.emailUsu, u.telefoneUsu, u.IDEnd, u.dataNascimentoUsu, u.senhaUsu,e.IDEnd
                AS IDEnd, e.logEnd, e.numEnd, e.bairroEnd, e.cidEnd, e.estEnd, e.CEPEnd
                FROM Usuario u
                INNER JOIN Endereco e 
                ON u.IDEnd = e.IDEnd", conexao);
                cmd.Parameters.AddWithValue("@IDUsu", Id);

                MySqlDataAdapter da = new MySqlDataAdapter(cmd);
                MySqlDataReader dr;

                Usuario usuario = new Usuario();
                dr = cmd.ExecuteReader(CommandBehavior.CloseConnection);

                while (dr.Read())
                {
                    usuario.IDUsu = (Int32)(dr["IDUsu"]);
                    usuario.nomeUsu = (string)(dr["nomeUsu"]);
                    usuario.CPFUsu = (string)(dr["CPFUsu"]);
                    usuario.emailUsu = (string)(dr["emailUsu"]);
                    usuario.telefoneUsu = (string)(dr["telefoneUsu"]);
                    usuario.dataNascismentoUsu = (DateTime)(dr["dataNascismentoUsu"]);
                    usuario.senhaUsu = (string)(dr["senhaUsu"]);
                    usuario.IDEnd = (Int32)(dr["IDEnd"]);

                    usuario.Endereco = new Endereco
                    {
                        IDEnd = Convert.ToInt32(dr["IDEnd"]),
                        logEnd = Convert.ToString(dr["logEnd"]),
                        numEnd = Convert.ToString(dr["numEnd"]),
                        bairroEnd = Convert.ToString(dr["bairroEnd"]),
                        cidEnd = Convert.ToString(dr["cidEnd"]),
                        estEnd = Convert.ToString(dr["estEnd"]),
                        CEPEnd = Convert.ToString(dr["CEPEnd"])
                    };
                }
                return usuario;
            }
        }
    }
}
