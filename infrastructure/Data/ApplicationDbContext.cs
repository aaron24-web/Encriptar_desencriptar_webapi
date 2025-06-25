// infrastructure/Data/ApplicationDbContext.cs
using Microsoft.EntityFrameworkCore;
using ENCRYPT.Domain.Entities;

namespace ENCRYPT.infrastructure.Data 
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }

        public DbSet<EncryptedMessage> EncryptedMessages { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<EncryptedMessage>(entity =>
            {
                entity.ToTable("encdes");

                entity.HasKey(e => e.Id);
                
                entity.Property(e => e.Id)
                      .HasColumnName("id_mensaje") // Corregido según tu tabla
                      .UseIdentityByDefaultColumn();

                entity.Property(e => e.PlainText).HasColumnName("frase_original");
                entity.Property(e => e.EncryptedText).HasColumnName("frase_encriptada");
                
                // --- CONFIGURACIÓN CRÍTICA AQUÍ ---
                entity.Property(e => e.CreatedAt)
                      .HasColumnName("fecha_creacion")
                      .ValueGeneratedOnAdd(); // <-- ESTA LÍNEA ES LA CLAVE
            });
        }
    }
}