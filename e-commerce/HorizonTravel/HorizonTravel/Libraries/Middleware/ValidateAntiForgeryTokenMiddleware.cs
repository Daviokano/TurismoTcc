using Microsoft.AspNetCore.Antiforgery;

namespace HorizonTravel.Libraries.Middleware
{
    public class ValidateAntiForgeryTokenMiddleware
    {
        private RequestDelegate _next;
        private IAntiforgery _antiforgery;

        public ValidateAntiForgeryTokenMiddleware(RequestDelegate next, IAntiforgery antiforgery)
        {
            _next = next;
            _antiforgery = antiforgery;
        }

        public async Task Invoke(HttpContext context)
        {
            if (string.Equals(context.Request.Method, "POST", StringComparison.OrdinalIgnoreCase))
            {
                await _antiforgery.ValidateRequestAsync(context);
            }
            await _next(context);
        }
    }
}
