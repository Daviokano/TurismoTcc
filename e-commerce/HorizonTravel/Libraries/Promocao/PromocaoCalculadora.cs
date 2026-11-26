using HorizonTravel.Models;
using HorizonTravel.Models.Constant;
using ModeloPromocao = HorizonTravel.Models.Promocao;

namespace HorizonTravel.Libraries.Promocao
{
    public static class PromocaoCalculadora
    {
        public static bool EstaValida(ModeloPromocao? promocao)
        {
            return promocao != null
                && promocao.situacaoPromocao == SituacaoPromocaoConstant.Ativo
                && promocao.dataValidade.Date >= DateTime.Today;
        }

        public static bool EstaAplicavel(Pacote pacote, ModeloPromocao? promocao)
        {
            if (!EstaValida(promocao))
            {
                return false;
            }

            string abrangencia = promocao!.abrangenciaPromocao.Trim();

            if (abrangencia.Equals("Todos", StringComparison.OrdinalIgnoreCase))
            {
                return true;
            }

            if (abrangencia.Equals("Nacionais", StringComparison.OrdinalIgnoreCase))
            {
                return pacote.destinoPac.Equals("Brasil", StringComparison.OrdinalIgnoreCase);
            }

            if (abrangencia.Equals("Internacionais", StringComparison.OrdinalIgnoreCase))
            {
                return !pacote.destinoPac.Equals("Brasil", StringComparison.OrdinalIgnoreCase);
            }

            return pacote.destinoPac.Equals(abrangencia, StringComparison.OrdinalIgnoreCase);
        }

        public static ModeloPromocao? ObterMelhorPromocao(
            Pacote pacote,
            IEnumerable<ModeloPromocao> promocoes)
        {
            return promocoes
                .Where(promocao => EstaAplicavel(pacote, promocao))
                .OrderByDescending(promocao => promocao.porcentagemDesconto)
                .ThenByDescending(promocao => promocao.dataValidade)
                .FirstOrDefault();
        }

        public static decimal CalcularPreco(Pacote pacote, ModeloPromocao? promocao)
        {
            if (!EstaAplicavel(pacote, promocao))
            {
                return pacote.precoPac;
            }

            decimal desconto = pacote.precoPac * promocao!.porcentagemDesconto / 100;
            return Math.Round(pacote.precoPac - desconto, 2);
        }
    }
}
