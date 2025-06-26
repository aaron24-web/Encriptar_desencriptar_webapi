// Program.cs - Solución Definitiva y Segura

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

// Configuración avanzada para que la API confíe en los encabezados del proxy (y obtenga la IP real del cliente).
// Esto es esencial para que funcione en cualquier proveedor de hosting (Azure, AWS, Render, etc.).
builder.Services.Configure<ForwardedHeadersOptions>(options =>
{
    options.ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto;
    // Limpiamos las redes conocidas para asegurarnos de que procese los encabezados
    // de los proxies de la infraestructura de hosting.
    options.KnownNetworks.Clear();
    options.KnownProxies.Clear();
});

// Se mantiene CORS por si la IP escolar tiene una aplicación web que necesite consumir la API.
builder.Services.AddCors(options =>
{
    options.AddPolicy(name: "_myAllowSpecificOrigins", policy =>
    {
        policy.WithOrigins("http://187.155.101.200", "https://187.155.101.200")
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
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

// El primer middleware en ejecutarse. Lee los encabezados del proxy para identificar la IP real.
app.UseForwardedHeaders();

// Nuestro "guardia de seguridad". Se ejecuta justo después de identificar la IP.
// Bloqueará cualquier IP no autorizada antes de que llegue a cualquier otra parte de la app.
// Puedes añadir más IPs separándolas con punto y coma, ej: "187.155.101.200;190.10.20.30"
app.UseMiddleware<IpWhitelistMiddleware>("187.155.101.200");

// Swagger y su UI. Este código solo se ejecutará si la IP fue aprobada por el middleware anterior.
app.UseSwagger();
app.UseSwaggerUI(options =>
{
    options.SwaggerEndpoint("/swagger/v1/swagger.json", "Cifrado API V1");
    // Hacemos que la UI de Swagger sea la página principal
    options.RoutePrefix = string.Empty;
});

// Resto del pipeline estándar.
app.UseHttpsRedirection();
app.UseRouting();

app.UseCors("_myAllowSpecificOrigins");

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
