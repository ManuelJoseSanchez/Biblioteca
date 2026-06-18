using System.ComponentModel.DataAnnotations;
using BibliotecaAPI.Entidades;
using Microsoft.AspNetCore.Identity;

namespace Biblioteca.Entidades
{
    public class Comentario
    {
        public Guid Id { get; set; }
        [Required]
        public required string Cuerpo { get; set; }
        public DateTime FechaPublicacion { get; set; }
        public int LibroId { get; set; }
        public Libro? Libro { get; set; }
        public required string UsuarioId { get; set; }
        public IdentityUser? Usuario {get; set;}

    }
}