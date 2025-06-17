// Programa principal modificado

// --- CAMBIOS EMPIEZAN AQUÍ ---

// Usings necesarios para la nueva configuración
using JaveragesLibrary.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
// Fin de Usings necesarios

using JaveragesLibrary.Services.Features.Mangas;
using JaveragesLibrary.Infrastructure.Repositories;
using JaveragesLibrary.Services.MappingsM; 

var builder = WebApplication.CreateBuilder(args);

// Registra los perfiles de AutoMapper (solo se necesita una vez)
builder.Services.AddAutoMapper(typeof(ResponseMappingProfile).Assembly);

// Configura el DbContext para usar PostgreSQL y la cadena de conexión de appsettings.json
var connectionString = builder.Configuration.GetConnectionString("SupabaseConnection");
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseNpgsql(connectionString));

// Registra el Repositorio y el Servicio con el ciclo de vida Scoped.
// Esto reemplaza las líneas anteriores de AddTransient<MangaRepository> y AddScoped<MangaService>.
builder.Services.AddScoped<MangaRepository>();
builder.Services.AddScoped<MangaService>();

// --- CAMBIOS TERMINAN AQUÍ ---


// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();

builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo
    {
        Version = "v1",
        Title = "Red API",
        Description = "Una API para gestionar una increíble colección de mangas",
        Contact = new Microsoft.OpenApi.Models.OpenApiContact
        {
            Name = "Tu Nombre/Equipo",
            Url = new Uri("https://tuwebsite.com")
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
        options.SwaggerEndpoint("/swagger/v1/swagger.json", "MangaBot API V1");
        options.RoutePrefix = string.Empty;
    });
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

app.Run();