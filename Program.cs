// JaveragesLibrary/Program.cs

using JaveragesLibrary.Infrastructure.Data;
using JaveragesLibrary.Infrastructure.Repositories;
using JaveragesLibrary.Services.Features.Generos;
using JaveragesLibrary.Services.Features.Mangas;
using JaveragesLibrary.Services.MappingsM;
using Microsoft.EntityFrameworkCore;

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

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/swagger/v1/swagger.json", "Tu API V1");
        options.RoutePrefix = string.Empty; // <-- Esta línea hace la magia
    });
}

//app.UseHttpsRedirection();

app.MapControllers();

app.Run();