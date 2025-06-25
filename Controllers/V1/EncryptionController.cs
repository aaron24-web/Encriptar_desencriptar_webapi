// Controllers/V1/EncryptionController.cs
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using ENCRYPT.Domain.Dtos;
using ENCRYPT.Domain.Entities;
using ENCRYPT.Services.Features.Cryptography;
using ENCRYPT.infrastructure.Repositories;
using Microsoft.AspNetCore.Authorization;
// Asegúrate que el namespace coincida con tu proyecto
namespace ENCRYPT.Controllers.V1
{
    [ApiController]
    [Route("api/v1/[controller]")]
    [Authorize]
    public class EncryptionController : ControllerBase
    {
        private readonly EncryptionService _encryptionService;
        private readonly EncryptedMessageRepository _repository;

        public EncryptionController(EncryptionService encryptionService, EncryptedMessageRepository repository)
        {
            _encryptionService = encryptionService;
            _repository = repository;
        }

        [HttpPost("encrypt")]
        public async Task<IActionResult> EncryptText([FromBody] EncryptRequestDto request)
        {
            if (string.IsNullOrEmpty(request.Text))
            {
                return BadRequest("El texto a encriptar no puede estar vacío.");
            }

            // 1. Llama al servicio para encriptar la frase
            string encryptedText = _encryptionService.Encrypt(request.Text);

            // 2. Prepara el objeto para guardarlo en la base de datos
            var newMessage = new EncryptedMessage
            {
                PlainText = request.Text,
                EncryptedText = encryptedText
            };

            // 3. Usa el repositorio para guardar el registro
            await _repository.AddAsync(newMessage);

            // 4. Devuelve el resultado encriptado al usuario
            return Ok(new { EncryptedResult = encryptedText });
        }

        [HttpGet("decrypt/{encryptedText}")]
        public async Task<IActionResult> DecryptText(string encryptedText)
        {
            // 1. Usa el repositorio para buscar el mensaje por su texto cifrado
            var savedMessage = await _repository.GetByEncryptedTextAsync(encryptedText);

            if (savedMessage == null)
            {
                return NotFound(new { message = "El mensaje cifrado no fue encontrado en el historial." });
            }

            // 2. Devuelve la frase original que encontramos en la base de datos
            return Ok(new { OriginalText = savedMessage.PlainText });
        }
    }
}