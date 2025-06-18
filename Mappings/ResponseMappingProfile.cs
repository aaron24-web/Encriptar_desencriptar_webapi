// JaveragesLibrary/Services/MappingsM/ResponseMappingProfile.cs

using AutoMapper;
using JaveragesLibrary.Domain.Dtos;
using JaveragesLibrary.Domain.Entities;

namespace JaveragesLibrary.Services.MappingsM
{
    public class ResponseMappingProfile : Profile
    {
        public ResponseMappingProfile()
        {
            CreateMap<Manga, MangaDTO>()
                .ForMember(dest => dest.PublicationYear, opt => opt.MapFrom(src => src.PublicationDate.Year));
            CreateMap<MangaCreateDTO, Manga>();

            // --- AÑADE ESTA LÍNEA ---
            CreateMap<Generos, GenerosDTO>();
        }
    }
}