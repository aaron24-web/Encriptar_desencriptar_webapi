// JaveragesLibrary/Infrastructure/Repositories/GenerosRepository.cs

using JaveragesLibrary.Infrastructure.Data;
using JaveragesLibrary.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace JaveragesLibrary.Infrastructure.Repositories
{
    public class GenerosRepository
    {
        private readonly ApplicationDbContext _context;

        public GenerosRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Generos>> GetAllAsync()
        {
            return await _context.Generos.OrderBy(g => g.Nombre).ToListAsync();
        }

        public async Task<Generos?> GetByIdAsync(int id)
        {
            return await _context.Generos.FindAsync(id);
        }
    }
}