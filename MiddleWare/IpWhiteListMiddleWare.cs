// Middlewares/IpWhitelistMiddleware.cs
using System.Net;

// Define un espacio de nombres para organizar el código.
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
            // Guardamos las IPs permitidas al iniciar la aplicación.
            _whitelistedIpStrings = whitelist.Split(';');
        }

        public async Task Invoke(HttpContext context)
        {
            // Gracias a la configuración que haremos en Program.cs, esta IP será la del cliente real.
            var remoteIp = context.Connection.RemoteIpAddress;

            _logger.LogInformation("Petición recibida desde la IP: {RemoteIp}", remoteIp);

            // Si por alguna razón la IP no se puede determinar, bloqueamos el acceso.
            if (remoteIp == null)
            {
                _logger.LogWarning("Acceso denegado: No se pudo determinar la IP de origen.");
                context.Response.StatusCode = StatusCodes.Status403Forbidden;
                await context.Response.WriteAsync("Acceso denegado: IP de origen desconocida.");
                return;
            }

            // Siempre permitimos el acceso desde localhost (127.0.0.1 o ::1) para facilitar el desarrollo.
            if (IPAddress.IsLoopback(remoteIp))
            {
                _logger.LogInformation("Acceso permitido para IP local (loopback): {RemoteIp}", remoteIp);
                await _next.Invoke(context);
                return;
            }

            // Revisamos si la IP del cliente está en nuestra lista blanca.
            // Esta forma de comparar funciona correctamente con formatos IPv4 (como 1.2.3.4)
            // y formatos IPv6-mapeados (como ::ffff:1.2.3.4) que usan los proveedores de nube.
            foreach (var ipString in _whitelistedIpStrings)
            {
                if (IPAddress.TryParse(ipString, out var whitelistedIp) && whitelistedIp.Equals(remoteIp))
                {
                    _logger.LogInformation("Acceso permitido para IP en lista blanca: {RemoteIp}", remoteIp);
                    await _next.Invoke(context);
                    return;
                }
            }
            
            // Si la IP no es local y no está en la lista, el acceso es denegado.
            _logger.LogWarning("Acceso denegado para IP no autorizada: {RemoteIp}", remoteIp);
            context.Response.StatusCode = StatusCodes.Status403Forbidden;
            await context.Response.WriteAsync("Acceso denegado.");
            return;
        }
    }
}
