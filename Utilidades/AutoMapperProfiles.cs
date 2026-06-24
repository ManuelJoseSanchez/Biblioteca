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
            
            CreateMap<AutorLibro, LibroDTO>()
            .ForMember(dto => dto.Id, config => config.MapFrom(ent => ent.LibroId))
            .ForMember(dto => dto.Titulo, config => config.MapFrom(ent => ent.Libro!.Titulo));

            /*DTO de libbros*/
            CreateMap<Libro, LibroDTO>();
            CreateMap<LibroCreacionDTO, Libro>().ForMember(ent => ent.Autores,
            config=> config.MapFrom(dto => dto.AutoresIds.Select(id => new AutorLibro{ AutorId = id})));

            CreateMap<Libro, LibroConAutorDTO>();

            CreateMap<AutorLibro, AutorDTO>().ForMember(dto => dto.Id, config => config.MapFrom(ent => ent.AutorId))
            .ForMember(dto => dto.NombreCompleto, config => config.MapFrom(e => this.MapearNombreApellidoAutor(e.Autor!)));
            
            CreateMap<LibroCreacionDTO, AutorLibro>()
                .ForMember(ent => ent.Libro, 
                    config => config.MapFrom(dto => new Libro { Titulo = dto.Titulo }));
            /*DTO de comentarios*/
            CreateMap<ComentarioCreacionDTO, Comentario>();
            
            CreateMap<Comentario, ComentarioDTO>()
                    .ForMember(dto =>dto.UsuarioEmail, config => config.MapFrom(ent => ent.Usuario!.Email));
            CreateMap<ComentarioPatchDTO, Comentario>().ReverseMap();

            CreateMap <Usuario, UsuarioDTO>();

        }

        private string MapearNombreApellidoAutor(Autor autor) => $"{autor!.Nombre} {autor.Apellidos}";

    }
}
