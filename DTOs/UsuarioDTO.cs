using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BibliotecaAPI.DTOs
{
    public class UsuarioDTO
    {
        public required string Email { get; set; }
        public DateTime FechaNacimiento { get; set; }
    }
}