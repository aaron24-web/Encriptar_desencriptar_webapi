// Middlewares/IpWhitelistMiddleware.cs
using System.Net;

namespace ENCRYPT.Middlewares
{
    public class IpWhitelistMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<IpWhitelistMiddleware> _logger;
        private readonly string[] _whitelistedIpStrings;

        public IpWhitelistMiddleware(RequestDelegate next, ILogger<IpWhitelistMiddleware> logger, string whitelist)
        {
            _next = next;
            _logger = logger;
            _whitelistedIpStrings = whitelist.Split(';');
        }

        public async Task Invoke(HttpContext context)
        {
            // =================================================================
            // INICIO DEL CÓDIGO DE DIAGNÓSTICO TEMPORAL
            // Este bloque imprimirá todos los encabezados de la petición.
            // =================================================================
            _logger.LogWarning("--- INICIANDO DIAGNÓSTICO DE ENCABEZADOS ---");
            foreach (var header in context.Request.Headers)
            {
                _logger.LogWarning("Encabezado: {Key}: {Value}", header.Key, header.Value);
            }
            _logger.LogWarning("--- FIN DEL DIAGNÓSTICO DE ENCABEZADOS ---");
            // =================================================================
            
            var remoteIp = context.Connection.RemoteIpAddress;
            _logger.LogInformation("Petición recibida desde la IP (detectada por ASP.NET): {RemoteIp}", remoteIp);

            if (remoteIp == null)
            {
                _logger.LogWarning("Acceso denegado: No se pudo determinar la IP de origen.");
                context.Response.StatusCode = StatusCodes.Status403Forbidden;
                await context.Response.WriteAsync("Acceso denegado: IP de origen desconocida.");
                return;
            }

            if (IPAddress.IsLoopback(remoteIp))
            {
                _logger.LogInformation("Acceso permitido para IP local (loopback): {RemoteIp}", remoteIp);
                await _next.Invoke(context);
                return;
            }

            foreach (var ipString in _whitelistedIpStrings)
            {
                if (IPAddress.TryParse(ipString, out var whitelistedIp) && whitelistedIp.Equals(remoteIp))
                {
                    _logger.LogInformation("Acceso permitido para IP en lista blanca: {RemoteIp}", remoteIp);
                    await _next.Invoke(context);
                    return;
                }
            }
            
            _logger.LogWarning("Acceso denegado para IP no autorizada: {RemoteIp}", remoteIp);
            context.Response.StatusCode = StatusCodes.Status403Forbidden;
            await context.Response.WriteAsync("Acceso denegado.");
            return;
        }
    }
}
