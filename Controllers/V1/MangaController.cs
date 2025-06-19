// JaveragesLibrary/Controllers/V1/MangaController.cs

using AutoMapper;
using JaveragesLibrary.Domain.Dtos;
using JaveragesLibrary.Domain.Entities;
using JaveragesLibrary.Services.Features.Mangas;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;

namespace JaveragesLibrary.Controllers.V1
{
    [ApiController]
    [Route("api/v1/[controller]")]
    [Authorize]
    public class MangaController : ControllerBase
    {
        private readonly MangaService _mangaService;
        private readonly IMapper _mapper;

        public MangaController(MangaService mangaService, IMapper mapper)
        {
            _mangaService = mangaService;
            _mapper = mapper;
        }

        // --- MÉTODO MODIFICADO ---
        // GET api/v1/manga?pageNumber=1&pageSize=10
        [HttpGet]
        public async Task<IActionResult> GetAll([FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10)
        {
            // Es buena práctica limitar el tamaño máximo de página
            const int maxPageSize = 50;
            if (pageSize > maxPageSize)
            {
                pageSize = maxPageSize;
            }

            var mangas = await _mangaService.GetAll(pageNumber, pageSize);
            var mangaDtos = _mapper.Map<IEnumerable<MangaDTO>>(mangas);
            return Ok(mangaDtos);
        }

        // --- MÉTODOS SIN CAMBIOS (Se incluyen para que el archivo esté completo) ---
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var manga = await _mangaService.GetById(id);
            if (manga == null)
            {
                return NotFound();
            }
            var dto = _mapper.Map<MangaDTO>(manga);
            return Ok(dto);
        }

        [HttpPost]
        public async Task<IActionResult> Add(MangaCreateDTO mangaDto)
        {
            var entity = _mapper.Map<Manga>(mangaDto);
            await _mangaService.Add(entity);
            var dto = _mapper.Map<MangaDTO>(entity);
            return CreatedAtAction(nameof(GetById), new { id = entity.Id }, dto);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] Manga mangaToUpdate)
        {
            if (id != mangaToUpdate.Id)
            {
                return BadRequest("El ID de la ruta no coincide con el ID del cuerpo.");
            }
            var success = await _mangaService.Update(mangaToUpdate);
            if (!success)
            {
                return NotFound($"Manga con ID {id} no encontrado.");
            }
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var success = await _mangaService.Delete(id);
            if (!success)
            {
                return NotFound($"Manga con ID {id} no encontrado.");
            }
            return NoContent();
        }
    }
}