using HorizonTravel.Models;
using ModeloPromocao = HorizonTravel.Models.Promocao;

namespace HorizonTravel.Libraries.Promocao
{
    public static class PromocaoCalculadora
    {
        public static bool EstaValida(ModeloPromocao? promocao)
        {
            return promocao != null && promocao.dataValidade.Date >= DateTime.Today;
        }

        public static decimal CalcularPreco(Pacote pacote, ModeloPromocao? promocao)
        {
            if (!EstaValida(promocao))
            {
                return pacote.precoPac;
            }

            decimal desconto = pacote.precoPac * promocao!.porcentagemDesconto / 100;
            return Math.Round(pacote.precoPac - desconto, 2);
        }
    }
}
