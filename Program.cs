// JaveragesLibrary/Program.cs

using System.Text;
using JaveragesLibrary.Infrastructure.Data;
using JaveragesLibrary.Infrastructure.Repositories;
using JaveragesLibrary.Services.Features.Generos;
using JaveragesLibrary.Services.Features.Mangas;
using JaveragesLibrary.Services.MappingsM;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

// Registra los perfiles de AutoMapper
builder.Services.AddAutoMapper(typeof(ResponseMappingProfile).Assembly);

// Configura el DbContext
var connectionString = builder.Configuration.GetConnectionString("SupabaseConnection");
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseNpgsql(connectionString));

// El registro de todos los servicios
builder.Services.AddScoped<MangaRepository>();
builder.Services.AddScoped<MangaService>();
builder.Services.AddScoped<GenerosRepository>();
builder.Services.AddScoped<GenerosService>();

// --- CONFIGURACIÓN DE JWT ---
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
// --- FIN DE CONFIGURACIÓN DE JWT ---

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();

// --- MODIFICACIÓN DE SWAGGERGEN PARA AÑADIR EL BOTÓN 'AUTHORIZE' ---
builder.Services.AddSwaggerGen(options =>
{
    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        In = ParameterLocation.Header,
        Description = "Por favor, introduce 'Bearer' seguido de un espacio y el token",
        Name = "Authorization",
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer"
    });
    options.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            new string[] {}
        }
    });
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/swagger/v1/swagger.json", "Tu API V1");
        options.RoutePrefix = string.Empty;
    });
}

// app.UseHttpsRedirection();

// --- AÑADE ESTAS DOS LÍNEAS ANTES DE `app.MapControllers()` ---
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();