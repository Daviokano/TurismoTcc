namespace HorizonTravel.Libraries.Cookie
{
    public class Cookie
    {
        private IHttpContextAccessor _context;

        public Cookie(IHttpContextAccessor context)
        {
            _context = context;
        }

        public void Cadastrar(string chave, string valor)
        {
            CookieOptions opcoes = new CookieOptions
            {
                Expires = DateTime.Now.AddDays(7),
                IsEssential = true,
                HttpOnly = true
            };

            _context.HttpContext.Response.Cookies.Append(chave, valor, opcoes);
        }

        public void Remover(string chave)
        {
            _context.HttpContext.Response.Cookies.Delete(chave);
        }

        public string Consultar(string chave)
        {
            return _context.HttpContext.Request.Cookies[chave];
        }

        public bool Existe(string chave)
        {
            return _context.HttpContext.Request.Cookies[chave] != null;
        }
    }
}
