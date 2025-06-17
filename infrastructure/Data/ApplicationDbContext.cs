// Asegúrate que tu archivo ApplicationDbContext.cs se vea así:

namespace JaveragesLibrary.Infrastructure.Data;

using Microsoft.EntityFrameworkCore;
using JaveragesLibrary.Domain.Entities;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
    {
    }

    // --- ¡ESTA ES LA LÍNEA CLAVE QUE SOLUCIONA EL ERROR! ---
    // Esta propiedad le dice a Entity Framework que tienes una "tabla" de Mangas.
    public DbSet<Manga> Mangas { get; set; }
    // ---------------------------------------------------------

    protected override void OnModelCreating(ModelBuilder modelBuilder)
{
    base.OnModelCreating(modelBuilder);

    // Le decimos a EF Core a qué tabla y columnas mapear la clase Manga
    modelBuilder.Entity<Manga>(entity =>
    {
        // 1. El nombre de la tabla en la base de datos
        entity.ToTable("mangas");

        // 2. Mapeo de cada propiedad de la clase C# a su columna en la base de datos
        entity.Property(e => e.Id).HasColumnName("id");
        entity.Property(e => e.Title).HasColumnName("titulo");
        entity.Property(e => e.Author).HasColumnName("autor");
        entity.Property(e => e.Genre).HasColumnName("genero");
        entity.Property(e => e.PublicationDate).HasColumnName("fecha_publicacion");
        entity.Property(e => e.Volumes).HasColumnName("volumenes");
        entity.Property(e => e.IsOngoing).HasColumnName("en_publicacion");
    });
}
}