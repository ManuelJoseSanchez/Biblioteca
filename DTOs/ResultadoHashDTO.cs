using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BibliotecaAPI.DTOs
{
    public class ResultadoHashDTO
    {
        public required string  Hash { get; set; }
        public required byte[] Sal { get; set; }
    }
}