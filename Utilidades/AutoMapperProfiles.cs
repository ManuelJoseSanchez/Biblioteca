using AutoMapper;
using Biblioteca.DTOs;
using Biblioteca.Entidades;
using BibliotecaAPI.DTOs;
using BibliotecaAPI.Entidades;


namespace BibliotecaAPI.Utilidades
{
    public class AutoMapperProfiles : Profile
    {
        public AutoMapperProfiles()
        {
            CreateMap<Autor, AutorDTO>().ForMember(dto => dto.NombreCompleto,
            config => config.MapFrom(autor => this.MapearNombreApellidoAutor(autor)));

            CreateMap<Autor, AutorConLibrosDTO>().ForMember(dto => dto.NombreCompleto,
            config => config.MapFrom(autor => this.MapearNombreApellidoAutor(autor)));
            CreateMap<AutorCreacionDTO, Autor>();
            CreateMap<Autor, AutorPatchDTO>().ReverseMap();

            /*DTO de libbros*/
            CreateMap<Libro, LibroDTO>();
            CreateMap<LibroCreacionDTO, Libro>();

            CreateMap<Libro, LibroConAutorDTO>().ForMember(dto => dto.AutorNombre,
            config => config.MapFrom(ent => this.MapearNombreApellidoAutor(ent.autor!)));

            /*DTO de comentarios*/
            CreateMap<ComentarioCreacionDTO, Comentario>();
            CreateMap<Comentario, ComentarioDTO>();
            CreateMap<ComentarioPatchDTO, Comentario>().ReverseMap();

        }

        private string MapearNombreApellidoAutor(Autor autor) => $"{autor!.Nombre} {autor.Apellidos}";

    }
}