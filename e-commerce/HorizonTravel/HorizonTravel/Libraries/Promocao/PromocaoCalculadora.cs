using HorizonTravel.Models;

namespace HorizonTravel.Libraries.Promocao
{
    public static class PromocaoCalculadora
    {
        public static bool EstaValida(Promocao? promocao)
        {
            return promocao != null && promocao.dataValidade.Date >= DateTime.Today;
        }

        public static decimal CalcularPreco(Pacote pacote, Promocao? promocao)
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
