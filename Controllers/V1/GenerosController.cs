// JaveragesLibrary/Controllers/V1/GenerosController.cs

using AutoMapper;
using JaveragesLibrary.Domain.Dtos;
using JaveragesLibrary.Services.Features.Generos; // <-- Este 'using' es clave
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;

// NOTA: No necesitamos un 'using' para la entidad aquí, el servicio ya se encarga de eso.

namespace JaveragesLibrary.Controllers.V1
{
    [ApiController]
    [Route("api/v1/[controller]")]
    public class GenerosController : ControllerBase
    {
        private readonly GenerosService _generosService;
        private readonly IMapper _mapper;

        public GenerosController(GenerosService generosService, IMapper mapper)
        {
            _generosService = generosService;
            _mapper = mapper;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var generos = await _generosService.GetAll();
            var generosDtos = _mapper.Map<IEnumerable<GenerosDTO>>(generos);
            return Ok(generosDtos);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var genero = await _generosService.GetById(id);
            if (genero == null) // Este error desaparecerá al corregir el service
            {
                return NotFound();
            }
            var dto = _mapper.Map<GenerosDTO>(genero); // Este también
            return Ok(dto);
        }
    }
}