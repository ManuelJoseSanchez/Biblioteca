using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Biblioteca.Middleware
{
    public class BloqueoPeticionMiddleware
    {
        private readonly RequestDelegate next;

        public BloqueoPeticionMiddleware(RequestDelegate next)
        {
            this.next = next;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            if (context.Request.Path == "/bloquedo")
            {
                context.Response.StatusCode = 403;
                await context.Response.WriteAsync("Acceso denegado");
            }
            else
            {
                await next.Invoke(context);
            }
        }

    }

    public static class BloqueoPeticionMiddlewareExtensions
    {
        public static IApplicationBuilder UseBloqueoPeticion(this IApplicationBuilder builder1)
        {
            return builder1.UseMiddleware<BloqueoPeticionMiddleware>();
        }

    }

}