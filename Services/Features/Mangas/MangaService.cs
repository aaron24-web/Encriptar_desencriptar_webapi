// JaveragesLibrary/Services/Features/Mangas/MangaService.cs

using JaveragesLibrary.Domain.Entities;
using JaveragesLibrary.Infrastructure.Repositories;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace JaveragesLibrary.Services.Features.Mangas
{
    public class MangaService
    {
        private readonly MangaRepository _mangaRepository;

        public MangaService(MangaRepository mangaRepository)
        {
            _mangaRepository = mangaRepository;
        }

        // --- MÉTODO MODIFICADO ---
        // Ahora acepta y pasa los parámetros de paginación
        public async Task<IEnumerable<Manga>> GetAll(int pageNumber, int pageSize)
        {
            return await _mangaRepository.GetAllAsync(pageNumber, pageSize);
        }

        // --- MÉTODOS SIN CAMBIOS (Se incluyen para que el archivo esté completo) ---
        public async Task<Manga?> GetById(int id)
        {
            return await _mangaRepository.GetByIdAsync(id);
        }

        public async Task Add(Manga manga)
        {
            await _mangaRepository.AddAsync(manga);
        }

        public async Task<bool> Update(Manga mangaToUpdate)
        {
            var existingManga = await _mangaRepository.GetByIdAsync(mangaToUpdate.Id);
            if (existingManga == null)
            {
                return false;
            }
            await _mangaRepository.UpdateAsync(mangaToUpdate);
            return true;
        }

        public async Task<bool> Delete(int id)
        {
            var existingManga = await _mangaRepository.GetByIdAsync(id);
            if (existingManga == null)
            {
                return false;
            }
            await _mangaRepository.DeleteAsync(id);
            return true;
        }
    }
}