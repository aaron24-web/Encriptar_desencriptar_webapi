// Middlewares/IpWhitelistMiddleware.cs
using System.Net;
using Microsoft.Extensions.Primitives;

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
            // --- LÓGICA DE DETECCIÓN DE IP DEFINITIVA ---
            // Leemos la IP directamente de los encabezados que nos interesan.
            IPAddress? remoteIp = null;
            
            // Prioridad 1: Encabezado de Cloudflare (el más fiable en tu caso)
            if (context.Request.Headers.TryGetValue("Cf-Connecting-Ip", out StringValues cfIp))
            {
                IPAddress.TryParse(cfIp.ToString(), out remoteIp);
                _logger.LogInformation("IP detectada desde encabezado 'Cf-Connecting-Ip': {RemoteIp}", remoteIp);
            }
            // Prioridad 2: Encabezado estándar X-Forwarded-For (si Cloudflare no estuviera)
            else if (context.Request.Headers.TryGetValue("X-Forwarded-For", out StringValues xffIp))
            {
                // Este encabezado puede ser una lista de IPs (cliente, proxy1, proxy2). La primera es la del cliente.
                var firstIp = xffIp.ToString().Split(',').FirstOrDefault();
                IPAddress.TryParse(firstIp, out remoteIp);
                _logger.LogInformation("IP detectada desde encabezado 'X-Forwarded-For': {RemoteIp}", remoteIp);
            }
            else
            {
                // Último recurso: la IP de la conexión directa.
                remoteIp = context.Connection.RemoteIpAddress;
                 _logger.LogInformation("IP detectada desde la conexión directa: {RemoteIp}", remoteIp);
            }
            // --- FIN DE LÓGICA DE DETECCIÓN ---

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
