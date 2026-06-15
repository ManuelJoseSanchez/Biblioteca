using AutoMapper;
using BibliotecaAPI.Datos;
using BibliotecaAPI.DTOs;
using BibliotecaAPI.Entidades;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
namespace BibliotecaAPI.Controllers
{
    [ApiController]
    [Route("api/autores_coleccion")]
    [Authorize]
    public class AutoresColeccionController : ControllerBase
    {
        private readonly ApplicationDbContext context;
        private readonly IMapper mapper;

        public AutoresColeccionController(ApplicationDbContext context, IMapper mapper)
        {
            this.context = context;
            this.mapper = mapper;
        }

        [HttpGet("{ids}", Name = "ObternerAutoresPorIds")]
        public async Task<ActionResult<List<AutorConLibrosDTO>>> Get(string ids)
        {
            var idsColeccion = new List<int>();
            foreach (var id in ids.Split(","))
            {
                if (int.TryParse(id, out int idInt))
                {
                    idsColeccion.Add(idInt);
                }
            }

            if (!idsColeccion.Any())
            {
                ModelState.AddModelError(nameof(ids), "Ningun Id fue encontrado");
                return ValidationProblem();
            }

            var autores = await this.context.Autores.Include(x => x.Libros)
                            .ThenInclude(x => x.Libro)
                            .Where(x => idsColeccion.Contains(x.Id))
                            .ToListAsync();
            if (autores.Count != idsColeccion.Count)
            {
                return NotFound();
            }

            var autoresDTO = this.mapper.Map<List<AutorConLibrosDTO>>(autores);
            return autoresDTO;
        }

        [HttpPost]
        public async Task<ActionResult> Post(IEnumerable<AutorCreacionDTO> autorCreacionDTOs)
        {
            var autores = this.mapper.Map<IEnumerable<Autor>>(autorCreacionDTOs);
            this.context.AddRange(autores);
            await this.context.SaveChangesAsync();

            var autoresDTO = this.mapper.Map<IEnumerable<AutorDTO>>(autores);
            var ids = autores.Select(x => x.Id);
            var idsString = string.Join(",", ids);
            return CreatedAtRoute("ObternerAutoresPorIds", new { ids = idsString }, autoresDTO);
        }
    }
}