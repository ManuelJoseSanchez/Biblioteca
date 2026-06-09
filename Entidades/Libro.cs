
using System.ComponentModel.DataAnnotations;
using Biblioteca.Entidades;

namespace BibliotecaAPI.Entidades
{
    public class Libro
    {
        public int Id { get; set; }
        [Required]
        [StringLength(250, ErrorMessage = "El campo {0} solo permite {1} carateres o menos")]
        public required string Titulo { get; set; }
       
        public List<AutorLibro> Autores { get; set; } = [];
        public List<Comentario> Comentarios { get; set; } = [];

    }
}