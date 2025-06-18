// JaveragesLibrary/Services/Features/Generos/GenerosService.cs

using JaveragesLibrary.Infrastructure.Repositories;
using System.Collections.Generic;
using System.Threading.Tasks;
// --- CORRECCIÓN AQUÍ ---
// Le damos un apodo a la clase 'Generos' para evitar confusión con el namespace
using GenerosEntity = JaveragesLibrary.Domain.Entities.Generos;

namespace JaveragesLibrary.Services.Features.Generos
{
    public class GenerosService
    {
        private readonly GenerosRepository _generosRepository;

        public GenerosService(GenerosRepository generosRepository)
        {
            _generosRepository = generosRepository;
        }

        // Usamos el apodo 'GenerosEntity' en los tipos de retorno
        public async Task<IEnumerable<GenerosEntity>> GetAll()
        {
            return await _generosRepository.GetAllAsync();
        }

        public async Task<GenerosEntity?> GetById(int id)
        {
            return await _generosRepository.GetByIdAsync(id);
        }
    }
}