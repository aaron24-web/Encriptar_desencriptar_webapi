// JaveragesLibrary/Domain/Entities/Manga.cs
namespace JaveragesLibrary.Domain.Entities; // Asegúrate que el namespace coincida con tu estructura

public class Manga
{
    public int Id { get; set; } // Identificador único
    public required string Title { get; set; } // Título del manga (C# 11 'required')
    public required string Author { get; set; } // Autor del manga (C# 11 'required')
    public int? Genre { get; set; } // Género (opcional, por eso el '?')
    public DateTime PublicationDate { get; set; } // Fecha de publicación
    public int Volumes { get; set; } // Número de volúmenes
    public bool IsOngoing { get; set; } // ¿Sigue en publicación?

    // Constructor (opcional, pero útil para inicializar)
    public Manga()
    {
        Title = string.Empty; // Inicialización para 'required' si no se usa constructor con params
        Author = string.Empty;
        PublicationDate = DateTime.MinValue; // O un valor por defecto más sensato
    }
}