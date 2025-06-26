// Program.cs - Versión Final y Completa para Entorno Cloud/Proxy

using System.Net;
using System.Text;
using ENCRYPT.infrastructure.Data;
using ENCRYPT.infrastructure.Repositories;
using ENCRYPT.Services.Features.Cryptography;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Microsoft.AspNetCore.HttpOverrides;
using ENCRYPT.Middlewares; // ¡Importante! Añadimos el using para nuestro middleware.

var builder = WebApplication.CreateBuilder(args);

// --- 1. CONFIGURACIÓN DE SERVICIOS ---

// --- CONFIGURACIÓN AVANZADA Y DEFINITIVA PARA FORWARDED HEADERS ---
// Esto es crucial para que funcione en Render.com o cualquier otro proveedor de nube.
builder.Services.Configure<ForwardedHeadersOptions>(options =>
{
    options.ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto;
    
    // El paso clave: Por defecto, ASP.NET Core no confía en proxies de redes privadas.
    // Al limpiar estas listas, le decimos que confíe en la cadena de proxies de Render
    // para que pueda leer correctamente el encabezado X-Forwarded-For con tu IP real.
    options.KnownNetworks.Clear();
    options.KnownProxies.Clear();
});

var connectionString = builder.Configuration.GetConnectionString("SupabaseConnection");
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseNpgsql(connectionString, npgsqlOptions =>
    {
        npgsqlOptions.CommandTimeout(90);
    }));

builder.Services.AddScoped<EncryptionService>();
builder.Services.AddScoped<EncryptedMessageRepository>();

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = builder.Configuration["Jwt:Issuer"],
            ValidAudience = builder.Configuration["Jwt:Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]))
        };
    });

builder.Services.AddAuthorization();
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo { Version = "v1", Title = "API de Cifrado" });
    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme {
        In = ParameterLocation.Header, Description = "Por favor, introduce 'Bearer' seguido de un espacio y el token",
        Name = "Authorization", Type = SecuritySchemeType.ApiKey, Scheme = "Bearer"
    });
    options.AddSecurityRequirement(new OpenApiSecurityRequirement {
        {
            new OpenApiSecurityScheme { Reference = new OpenApiReference { Type = ReferenceType.SecurityScheme, Id = "Bearer" } },
            new string[] {}
        }
    });
});

// --- 2. CONFIGURACIÓN DEL PIPELINE DE LA APLICACIÓN (EL ORDEN ES CRÍTICO) ---
var app = builder.Build();

// El primer middleware en ejecutarse. Ahora está configurado para confiar en los proxies de Render.
app.UseForwardedHeaders();

// Nuestro "guardia de seguridad". Bloqueará cualquier IP no autorizada.
app.UseMiddleware<IpWhitelistMiddleware>("187.155.101.200");

// Swagger y su UI. Este código solo se ejecutará si la IP fue aprobada por el middleware anterior.
app.UseSwagger();
app.UseSwaggerUI(options =>
{
    options.SwaggerEndpoint("/swagger/v1/swagger.json", "Cifrado API V1");
    options.RoutePrefix = string.Empty;
});

app.UseHttpsRedirection();
app.UseRouting();

// CORS ya no es tan crítico para la seguridad de IP, pero se mantiene como buena práctica.
app.UseCors("_myAllowSpecificOrigins");

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
