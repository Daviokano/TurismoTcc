using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace HorizonTravel.Libraries.Filtro
{
    public class ValidateHttpReferAttribute : Attribute, IActionFilter
    {
       
        public void OnActionExecuted(ActionExecutedContext context)
        {
            //executa antes de passar pelo controller
            //esta validação verifica se a requisição veio do host

            string referer = context.HttpContext.Request.Headers["Referer"].ToString();
            if(string.IsNullOrEmpty(referer))
            {
                context.Result = new ContentResult()
                {
                    Content = "Acesso negado!"
                };
            }
            else
            {
                Uri uri = new Uri(referer);
                string hostReferer = uri.Host;
                string hostServidor = context.HttpContext.Request.Host.Host;
                if (hostReferer != hostServidor)
                {
                    context.Result = new ContentResult()
                    {
                        Content = "Acesso negado!"
                    };
                }
            }
        }

        public void OnActionExecuting(ActionExecutingContext context)
        {
            //executado depois de passar pelo controller
        }
    }
}
