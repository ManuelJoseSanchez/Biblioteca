using System.ComponentModel.DataAnnotations;
using BibliotecaAPI.Validaciones;

namespace BibliotecaAPI.DTOs
{
    public class AutorCreacionDTO
    {
        [Required(ErrorMessage = "El campo {0} es requerido")]
        [StringLength(150,ErrorMessage ="El campo {0} solo permite {1} carateres o menos")]
        [PrimeraLetraMayucula]
        public required string Nombre { get; set; }
        [Required(ErrorMessage = "El campo {0} es requerido")]
        [StringLength(150,ErrorMessage ="El campo {0} solo permite {1} carateres o menos")]
        [PrimeraLetraMayucula]
        public required string Apellidos { get; set; }
        [StringLength(20,ErrorMessage ="El campo {0} solo permite {1} carateres o menos")]
        public string? Identificasion { get; set; }
        public List<LibroCreacionDTO> Libros { get; set; } = [];
    }
}
