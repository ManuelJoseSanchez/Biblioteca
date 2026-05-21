using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BibliotecaAPI.Entidades
{
    public class Valor
    {
        public int Id { get; set; }

        public required string nombre { get; set; }
    }
}