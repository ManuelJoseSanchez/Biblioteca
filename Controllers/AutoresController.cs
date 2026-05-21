
using BibliotecaAPI.Datos;
using BibliotecaAPI.Entidades;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;


namespace BibliotecaAPI.Controllers
{
    [ApiController]
    [Route("api/autores")]
    public class AutoresController : ControllerBase
    {
        private readonly ApplicationDbContext context;
        public AutoresController(ApplicationDbContext context)
        {
            this.context = context;
        }

        [HttpGet]
        public async Task<IEnumerable<Autor>> Get()
        {
           return await this.context.Autores.ToListAsync();
        }

        [HttpGet("primero")]
        public async Task<ActionResult<Autor>> GetPrimerAutor()
        {
            return await this.context.Autores.FirstAsync();
        }

        [HttpGet("{id:int}")]

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
        public async Task<ActionResult<Autor>> Get([FromRoute] int id, [FromQuery] bool incluirLibros)
        {
            var autor =  await this.context.Autores.Include(x => x.libros).FirstOrDefaultAsync(x => x.Id == id);
            if(autor is null)
            {
                return NotFound();
            }
            return autor;
        }

        [HttpGet("{nombre:alpha}")]
        public async Task<IEnumerable<Autor>>Get([FromRoute] string nombre)
        {
            return await this.context.Autores.Where(x => x.Nombre.Contains(nombre)).ToListAsync();
        }

        /*[HttpGet("{parametro1}/{paramentro2?}")] // parametro opcional
        public ActionResult Get(string parametro1, string? paramentro2)
        {
            return Ok(new {parametro1,paramentro2});
        }*/

        [HttpPost]
        public async Task<ActionResult> Post([FromBody]Autor autor)
        {
            this.context.Add(autor);
            await this.context.SaveChangesAsync();
            return Ok();
            
        }

        [HttpPut("{id:int}")]
        public async Task<ActionResult> Put([FromRoute] int id, [FromBody]Autor autor)
        {
            if(id != autor.Id)
            {
                return BadRequest("Los id deben de coincidir");
            }
            this.context.Update(autor);
            await this.context.SaveChangesAsync();
            return Ok();
        }

        [HttpDelete("{id:int}")]
        public async Task<ActionResult> Delete([FromRoute] int id)
        {
            var registrosBorrados = await this.context.Autores.Where(x => x.Id == id).ExecuteDeleteAsync();
            if( registrosBorrados == 0)
            {
                return NotFound();
            }
            return Ok();
        }
    }
}