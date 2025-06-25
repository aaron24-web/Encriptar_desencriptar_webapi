// infrastructure/Repositories/EncryptedMessageRepository.cs
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using ENCRYPT.Domain.Entities; // Asegúrate que el namespace coincida con tu proyecto
using ENCRYPT.infrastructure.Data;

namespace ENCRYPT.infrastructure.Repositories
{
    public class EncryptedMessageRepository
    {
        private readonly ApplicationDbContext _context;

        public EncryptedMessageRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task AddAsync(EncryptedMessage message)
        {
            await _context.EncryptedMessages.AddAsync(message);
            await _context.SaveChangesAsync();
        }

        public async Task<EncryptedMessage?> GetByEncryptedTextAsync(string encryptedText)
        {
            return await _context.EncryptedMessages
                .FirstOrDefaultAsync(m => m.EncryptedText == encryptedText);
        }
    }
}