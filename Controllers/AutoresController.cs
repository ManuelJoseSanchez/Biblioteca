
using AutoMapper;
using Biblioteca.DTOs;
using BibliotecaAPI.Datos;
using BibliotecaAPI.DTOs;
using BibliotecaAPI.Entidades;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;


namespace BibliotecaAPI.Controllers
{
    [ApiController]
    [Route("api/autores")]
    public class AutoresController : ControllerBase
    {
        private readonly ApplicationDbContext context;
        public readonly ILogger<AutoresController> logger;

        public readonly IMapper mapper;

        public AutoresController(ApplicationDbContext context, ILogger<AutoresController> logger, IMapper mapper)
        {
            this.context = context;
            this.logger = logger;
            this.mapper = mapper;
        }

        [HttpGet]
        public async Task<IEnumerable<AutorDTO>> Get()
        {
            this.logger.LogInformation("Obtenido los datos de autores");
            var autores = await this.context.Autores.ToListAsync();
            var autoresDTO = this.mapper.Map<IEnumerable<AutorDTO>>(autores);
            return autoresDTO;
        }

        [HttpGet("primero")]
        public async Task<ActionResult<Autor>> GetPrimerAutor()
        {
            return await this.context.Autores.FirstAsync();
        }

        [HttpGet("{id:int}", Name = "ObtenerAutor")]

        /**
        * obtener datos de la cabecera de la petición
         * [FromHeader] string valorCabecera
         * 
         * obtener datos de la cadena de consulta
         * [FromQuery] string valorQuery
         * 
         * obtener datos del url de la petición
         * [FromRoute] string valorCuerpo
        */
        public async Task<ActionResult<AutorConLibrosDTO>> Get([FromRoute] int id)
        {
            var autor = await this.context.Autores.Include(x => x.libros).FirstOrDefaultAsync(x => x.Id == id);
            if (autor is null)
            {
                return NotFound();
            }
            var autorDTO = this.mapper.Map<AutorConLibrosDTO>(autor);
            return autorDTO;
        }


        [HttpPost]
        public async Task<ActionResult> Post([FromBody] AutorCreacionDTO autorCreacionDTO)
        {
            var autor = this.mapper.Map<Autor>(autorCreacionDTO);
            this.context.Add(autor);
            await this.context.SaveChangesAsync();
            var autorDTO = this.mapper.Map<AutorDTO>(autor);
            return CreatedAtRoute("ObtenerAutor", new { id = autor.Id }, autorDTO);

        }

        [HttpPut("{id:int}")]
        public async Task<ActionResult> Put([FromRoute] int id, [FromBody] AutorCreacionDTO autorCreacionDTO)
        {
            var autor = this.mapper.Map<Autor>(autorCreacionDTO);
            autor.Id = id;
            this.context.Update(autor);
            await this.context.SaveChangesAsync();
            return NoContent();
        }

        [HttpPatch("{id:int}")]
        public async Task<ActionResult> Patch(int id, [FromBody] JsonPatchDocument<AutorPatchDTO> patchDocument)
        {
            if (patchDocument is null)
            {
                return BadRequest();
            }
            var autorDB = await this.context.Autores.FirstOrDefaultAsync(x => x.Id == id);
            if (autorDB is null)
            {
                return NotFound();
            }
            var autorPatch = this.mapper.Map<AutorPatchDTO>(autorDB);

            patchDocument.ApplyTo(autorPatch, ModelState);

            if (!ModelState.IsValid)
            {
                return ValidationProblem(ModelState);
            }

            var esValido = TryValidateModel(autorPatch);

            if (!esValido)
            {
                return ValidationProblem(ModelState);
            }

            this.mapper.Map(autorPatch, autorDB);
            await this.context.SaveChangesAsync();

            return NoContent();
        }

        [HttpDelete("{id:int}")]
        public async Task<ActionResult> Delete([FromRoute] int id)
        {
            var registrosBorrados = await this.context.Autores.Where(x => x.Id == id).ExecuteDeleteAsync();
            if (registrosBorrados == 0)
            {
                return NotFound();
            }
            return NoContent();
        }
    }
}
