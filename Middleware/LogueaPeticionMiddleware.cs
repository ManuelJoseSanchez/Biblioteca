
namespace Biblioteca.Middleware
{
    public class LogueaPeticionMiddleware
    {
        private readonly RequestDelegate next;

        public LogueaPeticionMiddleware(RequestDelegate next)
        {
            this.next = next;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            var logger = context.RequestServices.GetRequiredService<ILogger<Program>>();
            logger.LogInformation($"Peticion: {context.Request.Method} {context.Request.Path}");

            await next.Invoke(context);

            logger.LogInformation($"Respuesta: {context.Response.StatusCode}");
        }
    }

    public static class LogueaPeticionMiddlewareExtensions
    {
        public static IApplicationBuilder UseLogueaPeticion(this IApplicationBuilder builder1)
        {
            return builder1.UseMiddleware<LogueaPeticionMiddleware>();
        }
    }
}