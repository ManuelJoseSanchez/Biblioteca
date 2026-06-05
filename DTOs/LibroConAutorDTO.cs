using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BibliotecaAPI.DTOs
{
    public class LibroConAutorDTO : LibroDTO
    {
        public int AutorId { get; set; }
        public required string AutorNombre { get; set; }
    }
}