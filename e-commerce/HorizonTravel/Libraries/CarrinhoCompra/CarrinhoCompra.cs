using HorizonTravel.Models;
using Newtonsoft.Json;
using CookieHelper = HorizonTravel.Libraries.Cookie.Cookie;

namespace HorizonTravel.Libraries.CarrinhoCompra
{
    public class CarrinhoCompra
    {
        private const string Chave = "HorizonTravel.Carrinho";
        private CookieHelper _cookie;

        public CarrinhoCompra(CookieHelper cookie)
        {
            _cookie = cookie;
        }

        public void Salvar(List<ItemCarrinho> lista)
        {
            _cookie.Cadastrar(Chave, JsonConvert.SerializeObject(lista));
        }

        public List<ItemCarrinho> Consultar()
        {
            if (_cookie.Existe(Chave))
            {
                return JsonConvert.DeserializeObject<List<ItemCarrinho>>(_cookie.Consultar(Chave))
                    ?? new List<ItemCarrinho>();
            }

            return new List<ItemCarrinho>();
        }

        public void Cadastrar(ItemCarrinho item)
        {
            List<ItemCarrinho> lista = Consultar();
            ItemCarrinho itemLocalizado = lista.SingleOrDefault(x => x.IdPacote == item.IdPacote);

            if (itemLocalizado == null)
            {
                item.Quantidade = 1;
                lista.Add(item);
            }
            else
            {
                itemLocalizado.Quantidade++;
            }

            Salvar(lista);
        }

        public void Atualizar(ItemCarrinho item)
        {
            List<ItemCarrinho> lista = Consultar();
            ItemCarrinho itemLocalizado = lista.SingleOrDefault(x => x.IdPacote == item.IdPacote);

            if (itemLocalizado != null)
            {
                itemLocalizado.Quantidade = Math.Max(1, item.Quantidade);
                Salvar(lista);
            }
        }

        public void Remover(ItemCarrinho item)
        {
            List<ItemCarrinho> lista = Consultar();
            lista.RemoveAll(x => x.IdPacote == item.IdPacote);
            Salvar(lista);
        }

        public void RemoverTodos()
        {
            _cookie.Remover(Chave);
        }
    }
}
