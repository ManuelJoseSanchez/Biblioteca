using AutoMapper;
using BibliotecaAPI.DTOs;
using BibliotecaAPI.Entidades;

namespace BibliotecaAPI.Utilidades
{
    public class AutoMapperProfiles : Profile
    {
        public AutoMapperProfiles()
        {
            CreateMap<Autor, AutorDTO>().ForMember(dto => dto.NombreCompleto,
            config => config.MapFrom( autor => $"{autor.Nombre} {autor.Apellidos}"));
            CreateMap<Autor, AutorConLibrosDTO>().ForMember(dto => dto.NombreCompleto,
            config => config.MapFrom( autor => $"{autor.Nombre} {autor.Apellidos}"));
            CreateMap<AutorCreacionDTO, Autor>();

            /*DTO de libbros*/
            CreateMap<Libro, LibroDTO>();
            CreateMap<LibroCreacionDTO, Libro>();
            CreateMap<Libro, LibroConAutorDTO>().ForMember(dto => dto.AutorNombre, 
            config => config.MapFrom(ent => $"{ent.autor!.Nombre} {ent.autor.Apellidos}"));

        }
        
    }
}