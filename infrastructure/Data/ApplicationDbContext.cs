// JaveragesLibrary/Infrastructure/Data/ApplicationDbContext.cs

using Microsoft.EntityFrameworkCore;
using JaveragesLibrary.Domain.Entities;

namespace JaveragesLibrary.Infrastructure.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
        }

        // --- CAMBIO 1: AÑADE LA NUEVA TABLA ---
        public DbSet<Generos> Generos { get; set; }
        public DbSet<Manga> Mangas { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Mapeo para la tabla Generos
            modelBuilder.Entity<Generos>(entity =>
            {
                entity.ToTable("generos");
                entity.Property(e => e.Id).HasColumnName("id");
                entity.Property(e => e.Nombre).HasColumnName("nombre");
            });

            // Mapeo para la tabla Mangas
            modelBuilder.Entity<Manga>(entity =>
            {
                entity.ToTable("mangas");

                entity.Property(e => e.Id).HasColumnName("id");
                entity.Property(e => e.Title).HasColumnName("titulo");
                entity.Property(e => e.Author).HasColumnName("autor");
                
                // --- CAMBIO 2: CORRIGE EL NOMBRE DE LA COLUMNA ---
                entity.Property(e => e.Genre).HasColumnName("generos"); // Antes era "genero"

                entity.Property(e => e.PublicationDate).HasColumnName("fecha_publicacion");
                entity.Property(e => e.Volumes).HasColumnName("volumenes");
                entity.Property(e => e.IsOngoing).HasColumnName("en_publicacion");
            });
        }
    }
}