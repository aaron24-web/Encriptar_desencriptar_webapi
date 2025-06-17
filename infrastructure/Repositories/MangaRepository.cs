// JaveragesLibrary/Infrastructure/Repositories/MangaRepository.cs

using JaveragesLibrary.Infrastructure.Data;
using JaveragesLibrary.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace JaveragesLibrary.Infrastructure.Repositories
{
    public class MangaRepository
    {
        private readonly ApplicationDbContext _context;

        public MangaRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        // --- MÉTODO MODIFICADO ---
        // Ahora acepta los parámetros de paginación
        public async Task<IEnumerable<Manga>> GetAllAsync(int pageNumber, int pageSize)
        {
            return await _context.Mangas
                .OrderBy(m => m.Id) // Es una buena práctica ordenar antes de paginar
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();
        }

        // --- MÉTODOS SIN CAMBIOS (Se incluyen para que el archivo esté completo) ---
        public async Task<Manga?> GetByIdAsync(int id)
        {
            return await _context.Mangas.FindAsync(id);
        }

        public async Task AddAsync(Manga manga)
        {
            await _context.Mangas.AddAsync(manga);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(Manga manga)
        {
            _context.Entry(manga).State = EntityState.Modified;
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var mangaToDelete = await _context.Mangas.FindAsync(id);
            if (mangaToDelete != null)
            {
                _context.Mangas.Remove(mangaToDelete);
                await _context.SaveChangesAsync();
            }
        }
    }
}