using System.Data;
using HorizonTravel.Models;
using HorizonTravel.Repository.Contract;
using MySql.Data.MySqlClient;

namespace HorizonTravel.Repository
{
    public class ReservaRepository : IReservaRepository
    {
        private readonly string _conexaoMySQL;

        public ReservaRepository(IConfiguration configuration)
        {
            _conexaoMySQL = configuration.GetConnectionString("ConexaoMySQL");
        }

        public int CadastrarReserva(Reserva reserva)
        {
            using var conexao = new MySqlConnection(_conexaoMySQL);
            conexao.Open();

            const string sql = @"
                INSERT INTO Reserva
                    (IDUsu, IDPac, dataRes, statusRes)
                VALUES
                    (@IDUsu, @IDPac, @dataRes, @statusRes);
                SELECT LAST_INSERT_ID();";

            using var comando = new MySqlCommand(sql, conexao);
            comando.Parameters.AddWithValue("@IDUsu", reserva.IDUsu);
            comando.Parameters.AddWithValue("@IDPac", reserva.IDPac);
            comando.Parameters.AddWithValue("@dataRes", reserva.dataRes.Date);
            comando.Parameters.AddWithValue("@statusRes", reserva.statusRes);

            return Convert.ToInt32(comando.ExecuteScalar());
        }

        public void CadastrarPassageiros(
            int idReserva,
            IEnumerable<Passageiro> passageiros)
        {
            using var conexao = new MySqlConnection(_conexaoMySQL);
            conexao.Open();

            const string sql = @"
                INSERT INTO Passageiro
                    (IDRes, nomeCompleto, CPF, dataNascimento, passaporte)
                VALUES
                    (@IDRes, @nomeCompleto, @CPF, @dataNascimento, @passaporte)";

            foreach (var passageiro in passageiros)
            {
                using var comando = new MySqlCommand(sql, conexao);
                comando.Parameters.AddWithValue("@IDRes", idReserva);
                comando.Parameters.AddWithValue("@nomeCompleto", passageiro.nomeCompleto);
                comando.Parameters.AddWithValue("@CPF", passageiro.CPF);
                comando.Parameters.AddWithValue("@dataNascimento", passageiro.dataNascimento.Date);
                comando.Parameters.AddWithValue(
                    "@passaporte",
                    string.IsNullOrWhiteSpace(passageiro.passaporte)
                        ? DBNull.Value
                        : passageiro.passaporte);
                comando.ExecuteNonQuery();
            }
        }

        public void CadastrarPagamento(Pagamento pagamento)
        {
            using var conexao = new MySqlConnection(_conexaoMySQL);
            conexao.Open();

            const string sql = @"
                INSERT INTO Pagamento
                    (IDRes, valorPag, dataPag, metodoPag, statusPag)
                VALUES
                    (@IDRes, @valorPag, @dataPag, @metodoPag, @statusPag)";

            using var comando = new MySqlCommand(sql, conexao);
            comando.Parameters.AddWithValue("@IDRes", pagamento.IDRes);
            comando.Parameters.AddWithValue("@valorPag", pagamento.valorPag);
            comando.Parameters.AddWithValue("@dataPag", pagamento.dataPag.Date);
            comando.Parameters.AddWithValue("@metodoPag", pagamento.metodoPag);
            comando.Parameters.AddWithValue("@statusPag", pagamento.statusPag);
            comando.ExecuteNonQuery();
        }

        public int FinalizarReserva(
            Reserva reserva,
            IEnumerable<Passageiro> passageiros,
            Pagamento pagamento)
        {
            var listaPassageiros = passageiros.ToList();

            using var conexao = new MySqlConnection(_conexaoMySQL);
            conexao.Open();
            using var transacao = conexao.BeginTransaction();

            try
            {
                const string sqlEstoque = @"
                    UPDATE Pacote
                    SET quantidadePac = quantidadePac - @quantidade
                    WHERE IDPac = @IDPac
                      AND quantidadePac >= @quantidade
                      AND situacaoPac = 'A'";

                using var comandoEstoque = new MySqlCommand(sqlEstoque, conexao, transacao);
                comandoEstoque.Parameters.Add("@quantidade", MySqlDbType.Int32).Value = listaPassageiros.Count;
                comandoEstoque.Parameters.Add("@IDPac", MySqlDbType.Int32).Value = reserva.IDPac;

                if (comandoEstoque.ExecuteNonQuery() != 1)
                {
                    throw new InvalidOperationException("O pacote não possui vagas disponíveis.");
                }

                const string sqlReserva = @"
                    INSERT INTO Reserva (IDUsu, IDPac, dataRes, statusRes)
                    VALUES (@IDUsu, @IDPac, @dataRes, @statusRes)";

                using var comandoReserva = new MySqlCommand(sqlReserva, conexao, transacao);
                comandoReserva.Parameters.Add("@IDUsu", MySqlDbType.Int32).Value = reserva.IDUsu;
                comandoReserva.Parameters.Add("@IDPac", MySqlDbType.Int32).Value = reserva.IDPac;
                comandoReserva.Parameters.Add("@dataRes", MySqlDbType.Date).Value = reserva.dataRes.Date;
                comandoReserva.Parameters.Add("@statusRes", MySqlDbType.VarChar).Value = reserva.statusRes;
                comandoReserva.ExecuteNonQuery();

                int idReserva = Convert.ToInt32(comandoReserva.LastInsertedId);

                const string sqlPassageiro = @"
                    INSERT INTO Passageiro
                        (IDRes, nomeCompleto, CPF, dataNascimento, passaporte)
                    VALUES
                        (@IDRes, @nomeCompleto, @CPF, @dataNascimento, @passaporte)";

                foreach (var passageiro in listaPassageiros)
                {
                    using var comandoPassageiro = new MySqlCommand(sqlPassageiro, conexao, transacao);
                    comandoPassageiro.Parameters.Add("@IDRes", MySqlDbType.Int32).Value = idReserva;
                    comandoPassageiro.Parameters.Add("@nomeCompleto", MySqlDbType.VarChar).Value = passageiro.nomeCompleto;
                    comandoPassageiro.Parameters.Add("@CPF", MySqlDbType.VarChar).Value = passageiro.CPF;
                    comandoPassageiro.Parameters.Add("@dataNascimento", MySqlDbType.Date).Value = passageiro.dataNascimento.Date;
                    comandoPassageiro.Parameters.Add("@passaporte", MySqlDbType.VarChar).Value =
                        string.IsNullOrWhiteSpace(passageiro.passaporte)
                            ? DBNull.Value
                            : passageiro.passaporte;
                    comandoPassageiro.ExecuteNonQuery();
                }

                const string sqlPagamento = @"
                    INSERT INTO Pagamento
                        (IDRes, valorPag, dataPag, metodoPag, statusPag)
                    VALUES
                        (@IDRes, @valorPag, @dataPag, @metodoPag, @statusPag)";

                using var comandoPagamento = new MySqlCommand(sqlPagamento, conexao, transacao);
                comandoPagamento.Parameters.Add("@IDRes", MySqlDbType.Int32).Value = idReserva;
                comandoPagamento.Parameters.Add("@valorPag", MySqlDbType.Decimal).Value = pagamento.valorPag;
                comandoPagamento.Parameters.Add("@dataPag", MySqlDbType.Date).Value = pagamento.dataPag.Date;
                comandoPagamento.Parameters.Add("@metodoPag", MySqlDbType.VarChar).Value = pagamento.metodoPag;
                comandoPagamento.Parameters.Add("@statusPag", MySqlDbType.VarChar).Value = pagamento.statusPag;
                comandoPagamento.ExecuteNonQuery();

                transacao.Commit();
                return idReserva;
            }
            catch
            {
                transacao.Rollback();
                throw;
            }
        }

        public IEnumerable<ReservaResumo> ObterReservasPorUsuario(int idUsuario)
        {
            const string sql = @"
                SELECT r.IDRes, r.IDUsu, r.IDPac, u.nomeUsu, u.emailUsu, u.telefoneUsu,
                       p.destinoPac, p.dataPac, r.dataRes, r.statusRes,
                       COALESCE(pg.valorPag, 0) AS valorPag,
                       COALESCE(pg.statusPag, 'Sem pagamento') AS statusPag,
                       COALESCE(pg.metodoPag, '-') AS metodoPag,
                       COUNT(pa.IDPassageiro) AS quantidadePassageiros
                FROM Reserva r
                INNER JOIN Usuario u ON u.IDUsu = r.IDUsu
                INNER JOIN Pacote p ON p.IDPac = r.IDPac
                LEFT JOIN Pagamento pg ON pg.IDRes = r.IDRes
                LEFT JOIN Passageiro pa ON pa.IDRes = r.IDRes
                WHERE r.IDUsu = @IDUsu
                GROUP BY r.IDRes, r.IDUsu, r.IDPac, u.nomeUsu, u.emailUsu, u.telefoneUsu,
                         p.destinoPac, p.dataPac, r.dataRes, r.statusRes,
                         pg.valorPag, pg.statusPag, pg.metodoPag
                ORDER BY r.dataRes DESC, r.IDRes DESC";

            using var conexao = new MySqlConnection(_conexaoMySQL);
            conexao.Open();
            using var comando = new MySqlCommand(sql, conexao);
            comando.Parameters.Add("@IDUsu", MySqlDbType.Int32).Value = idUsuario;

            return LerReservas(comando);
        }

        public IEnumerable<Historico> BuscarHistoricoPorCliente(int idUsuario)
        {
            using var conexao = new MySqlConnection(_conexaoMySQL);
            conexao.Open();

            const string sql = @"
                SELECT h.IDHist,
                       h.IDRes,
                       h.IDFun,
                       h.statusAnterior,
                       h.statusNovo,
                       h.dataModificacao,
                       f.nomeFun
                FROM Historico h
                INNER JOIN Reserva r
                    ON r.IDRes = h.IDRes
                LEFT JOIN Funcionario f
                    ON f.IDFun = h.IDFun
                WHERE r.IDUsu = @IDUsu
                ORDER BY h.dataModificacao DESC,
                         h.IDHist DESC";

            using var comando = new MySqlCommand(sql, conexao);
            comando.Parameters.Add("@IDUsu", MySqlDbType.Int32).Value = idUsuario;

            using var leitor = comando.ExecuteReader();
            var historicos = new List<Historico>();

            while (leitor.Read())
            {
                historicos.Add(MapearHistorico(leitor));
            }

            return historicos;
        }

        public void RegistrarAlteracao(
            int idReserva,
            int? idFuncionario,
            string? statusAnterior,
            string statusNovo)
        {
            using var conexao = new MySqlConnection(_conexaoMySQL);
            conexao.Open();

            const string sql = @"
                INSERT INTO Historico
                    (IDRes, IDFun, statusAnterior, statusNovo, dataModificacao)
                VALUES
                    (@IDRes, @IDFun, @statusAnterior, @statusNovo, NOW())";

            using var comando = new MySqlCommand(sql, conexao);
            AdicionarParametrosHistorico(
                comando,
                idReserva,
                idFuncionario,
                statusAnterior,
                statusNovo);
            comando.ExecuteNonQuery();
        }

        public ReservaDetalhesViewModel? ObterDetalhesReserva(int idReserva)
        {
            const string sqlReserva = @"
                SELECT r.IDRes, r.IDUsu, r.IDPac, u.nomeUsu, u.emailUsu, u.telefoneUsu,
                       p.destinoPac, p.dataPac, r.dataRes, r.statusRes,
                       COALESCE(pg.valorPag, 0) AS valorPag,
                       COALESCE(pg.statusPag, 'Sem pagamento') AS statusPag,
                       COALESCE(pg.metodoPag, '-') AS metodoPag,
                       (SELECT COUNT(*) FROM Passageiro WHERE IDRes = r.IDRes) AS quantidadePassageiros
                FROM Reserva r
                INNER JOIN Usuario u ON u.IDUsu = r.IDUsu
                INNER JOIN Pacote p ON p.IDPac = r.IDPac
                LEFT JOIN Pagamento pg ON pg.IDRes = r.IDRes
                WHERE r.IDRes = @IDRes";

            using var conexao = new MySqlConnection(_conexaoMySQL);
            conexao.Open();
            using var comandoReserva = new MySqlCommand(sqlReserva, conexao);
            comandoReserva.Parameters.Add("@IDRes", MySqlDbType.Int32).Value = idReserva;
            using var leitorReserva = comandoReserva.ExecuteReader();

            if (!leitorReserva.Read())
            {
                return null;
            }

            var detalhes = new ReservaDetalhesViewModel
            {
                Reserva = MapearReserva(leitorReserva)
            };

            leitorReserva.Close();
            detalhes.Passageiros = ObterPassageiros(conexao, idReserva);
            detalhes.Historico = ObterHistorico(conexao, idReserva);
            return detalhes;
        }

        public void AtualizarStatusPagamento(int idReserva, string statusPagamento, int? idFuncionario)
        {
            using var conexao = new MySqlConnection(_conexaoMySQL);
            conexao.Open();
            using var transacao = conexao.BeginTransaction();

            try
            {
                const string sqlStatusAtual = @"
                    SELECT statusPag
                    FROM Pagamento
                    WHERE IDRes = @IDRes
                    FOR UPDATE";

                string? statusAnterior;
                using (var comandoStatusAtual = new MySqlCommand(sqlStatusAtual, conexao, transacao))
                {
                    comandoStatusAtual.Parameters.Add("@IDRes", MySqlDbType.Int32).Value = idReserva;
                    statusAnterior = Convert.ToString(comandoStatusAtual.ExecuteScalar());
                }

                if (string.IsNullOrWhiteSpace(statusAnterior))
                {
                    throw new InvalidOperationException("Pagamento não encontrado.");
                }

                const string sqlPagamento = @"
                    UPDATE Pagamento
                    SET statusPag = @statusPag
                    WHERE IDRes = @IDRes";

                using var comandoPagamento = new MySqlCommand(sqlPagamento, conexao, transacao);
                comandoPagamento.Parameters.Add("@statusPag", MySqlDbType.VarChar).Value = statusPagamento;
                comandoPagamento.Parameters.Add("@IDRes", MySqlDbType.Int32).Value = idReserva;

                comandoPagamento.ExecuteNonQuery();

                if (statusPagamento == "Cancelado" && statusAnterior != "Cancelado")
                {
                    const string sqlDevolverVagas = @"
                        UPDATE Pacote p
                        INNER JOIN Reserva r ON r.IDPac = p.IDPac
                        SET p.quantidadePac = p.quantidadePac + (
                            SELECT COUNT(*)
                            FROM Passageiro pa
                            WHERE pa.IDRes = r.IDRes
                        )
                        WHERE r.IDRes = @IDRes";

                    using var comandoDevolverVagas = new MySqlCommand(sqlDevolverVagas, conexao, transacao);
                    comandoDevolverVagas.Parameters.Add("@IDRes", MySqlDbType.Int32).Value = idReserva;
                    comandoDevolverVagas.ExecuteNonQuery();
                }

                string? statusReserva = statusPagamento switch
                {
                    "Pago" => "Confirmada",
                    "Cancelado" => "Cancelada",
                    _ => null
                };

                if (statusReserva != null)
                {
                    AtualizarStatusReservaInterno(conexao, transacao, idReserva, statusReserva, idFuncionario);
                }

                transacao.Commit();
            }
            catch
            {
                transacao.Rollback();
                throw;
            }
        }

        public void AtualizarStatusReserva(int idReserva, string statusReserva, int? idFuncionario)
        {
            using var conexao = new MySqlConnection(_conexaoMySQL);
            conexao.Open();
            using var transacao = conexao.BeginTransaction();

            try
            {
                AtualizarStatusReservaInterno(conexao, transacao, idReserva, statusReserva, idFuncionario);
                transacao.Commit();
            }
            catch
            {
                transacao.Rollback();
                throw;
            }
        }

        private static IEnumerable<ReservaResumo> LerReservas(MySqlCommand comando)
        {
            var reservas = new List<ReservaResumo>();
            using var leitor = comando.ExecuteReader();

            while (leitor.Read())
            {
                reservas.Add(MapearReserva(leitor));
            }

            return reservas;
        }

        private static ReservaResumo MapearReserva(IDataRecord registro)
        {
            return new ReservaResumo
            {
                IDRes = Convert.ToInt32(registro["IDRes"]),
                IDUsu = Convert.ToInt32(registro["IDUsu"]),
                IDPac = Convert.ToInt32(registro["IDPac"]),
                nomeUsu = Convert.ToString(registro["nomeUsu"]) ?? string.Empty,
                emailUsu = Convert.ToString(registro["emailUsu"]) ?? string.Empty,
                telefoneUsu = Convert.ToString(registro["telefoneUsu"]) ?? string.Empty,
                destinoPac = Convert.ToString(registro["destinoPac"]) ?? string.Empty,
                dataPac = Convert.ToDateTime(registro["dataPac"]),
                dataRes = Convert.ToDateTime(registro["dataRes"]),
                valorPag = Convert.ToDecimal(registro["valorPag"]),
                statusRes = Convert.ToString(registro["statusRes"]) ?? string.Empty,
                statusPag = Convert.ToString(registro["statusPag"]) ?? string.Empty,
                metodoPag = Convert.ToString(registro["metodoPag"]) ?? string.Empty,
                quantidadePassageiros = Convert.ToInt32(registro["quantidadePassageiros"])
            };
        }

        private static IEnumerable<Passageiro> ObterPassageiros(MySqlConnection conexao, int idReserva)
        {
            const string sql = @"
                SELECT IDPassageiro, IDRes, nomeCompleto, CPF, dataNascimento, passaporte
                FROM Passageiro
                WHERE IDRes = @IDRes
                ORDER BY IDPassageiro";

            var passageiros = new List<Passageiro>();
            using var comando = new MySqlCommand(sql, conexao);
            comando.Parameters.Add("@IDRes", MySqlDbType.Int32).Value = idReserva;
            using var leitor = comando.ExecuteReader();

            while (leitor.Read())
            {
                passageiros.Add(new Passageiro
                {
                    IDPassageiro = Convert.ToInt32(leitor["IDPassageiro"]),
                    IDRes = Convert.ToInt32(leitor["IDRes"]),
                    nomeCompleto = Convert.ToString(leitor["nomeCompleto"]) ?? string.Empty,
                    CPF = Convert.ToString(leitor["CPF"]) ?? string.Empty,
                    dataNascimento = Convert.ToDateTime(leitor["dataNascimento"]),
                    passaporte = leitor["passaporte"] == DBNull.Value
                        ? null
                        : Convert.ToString(leitor["passaporte"])
                });
            }

            return passageiros;
        }

        private static IEnumerable<Historico> ObterHistorico(MySqlConnection conexao, int idReserva)
        {
            const string sql = @"
                SELECT h.IDHist, h.IDRes, h.IDFun, h.statusAnterior, h.statusNovo,
                       h.dataModificacao, f.nomeFun
                FROM Historico h
                LEFT JOIN Funcionario f ON f.IDFun = h.IDFun
                WHERE h.IDRes = @IDRes
                ORDER BY h.dataModificacao DESC, h.IDHist DESC";

            var historico = new List<Historico>();
            using var comando = new MySqlCommand(sql, conexao);
            comando.Parameters.Add("@IDRes", MySqlDbType.Int32).Value = idReserva;
            using var leitor = comando.ExecuteReader();

            while (leitor.Read())
            {
                historico.Add(MapearHistorico(leitor));
            }

            return historico;
        }

        private static Historico MapearHistorico(IDataRecord registro)
        {
            return new Historico
            {
                IDHist = Convert.ToInt32(registro["IDHist"]),
                IDRes = Convert.ToInt32(registro["IDRes"]),
                IDFun = registro["IDFun"] == DBNull.Value
                    ? null
                    : Convert.ToInt32(registro["IDFun"]),
                statusAnterior = registro["statusAnterior"] == DBNull.Value
                    ? null
                    : Convert.ToString(registro["statusAnterior"]),
                statusNovo = registro["statusNovo"] == DBNull.Value
                    ? null
                    : Convert.ToString(registro["statusNovo"]),
                dataModificacao = Convert.ToDateTime(registro["dataModificacao"]),
                nomeFun = registro["nomeFun"] == DBNull.Value
                    ? null
                    : Convert.ToString(registro["nomeFun"])
            };
        }

        private static void AdicionarParametrosHistorico(
            MySqlCommand comando,
            int idReserva,
            int? idFuncionario,
            string? statusAnterior,
            string statusNovo)
        {
            comando.Parameters.Add("@IDRes", MySqlDbType.Int32).Value = idReserva;
            comando.Parameters.Add("@IDFun", MySqlDbType.Int32).Value =
                idFuncionario.HasValue ? idFuncionario.Value : DBNull.Value;
            comando.Parameters.Add("@statusAnterior", MySqlDbType.VarChar).Value =
                statusAnterior ?? (object)DBNull.Value;
            comando.Parameters.Add("@statusNovo", MySqlDbType.VarChar).Value = statusNovo;
        }

        private static void AtualizarStatusReservaInterno(
            MySqlConnection conexao,
            MySqlTransaction transacao,
            int idReserva,
            string statusNovo,
            int? idFuncionario)
        {
            const string sqlConsulta = @"
                SELECT statusRes
                FROM Reserva
                WHERE IDRes = @IDRes
                FOR UPDATE";

            string? statusAnterior;
            using (var comandoConsulta = new MySqlCommand(sqlConsulta, conexao, transacao))
            {
                comandoConsulta.Parameters.Add("@IDRes", MySqlDbType.Int32).Value = idReserva;
                statusAnterior = Convert.ToString(comandoConsulta.ExecuteScalar());
            }

            if (string.IsNullOrWhiteSpace(statusAnterior))
            {
                throw new InvalidOperationException("Reserva não encontrada.");
            }

            if (statusAnterior == statusNovo)
            {
                return;
            }

            const string sqlAtualizar = @"
                UPDATE Reserva
                SET statusRes = @statusRes
                WHERE IDRes = @IDRes";

            using (var comandoAtualizar = new MySqlCommand(sqlAtualizar, conexao, transacao))
            {
                comandoAtualizar.Parameters.Add("@statusRes", MySqlDbType.VarChar).Value = statusNovo;
                comandoAtualizar.Parameters.Add("@IDRes", MySqlDbType.Int32).Value = idReserva;
                comandoAtualizar.ExecuteNonQuery();
            }

            const string sqlHistorico = @"
                INSERT INTO Historico
                    (IDRes, IDFun, statusAnterior, statusNovo, dataModificacao)
                VALUES
                    (@IDRes, @IDFun, @statusAnterior, @statusNovo, NOW())";

            using var comandoHistorico = new MySqlCommand(sqlHistorico, conexao, transacao);
            AdicionarParametrosHistorico(
                comandoHistorico,
                idReserva,
                idFuncionario,
                statusAnterior,
                statusNovo);
            comandoHistorico.ExecuteNonQuery();
        }
    }
}
