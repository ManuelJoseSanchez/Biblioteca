
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
        public async Task<ActionResult<Autor>> Get(int id)
        {
            var autor =  await this.context.Autores.Include(x => x.libros).FirstOrDefaultAsync(x => x.Id == id);
            if(autor is null)
            {
                return NotFound();
            }
            return autor;
        }

        [HttpGet("{parametro1}/{paramentro2?}")] // parametro opcional
        public ActionResult Get(string parametro1, string? paramentro2)
        {
            return Ok(new {parametro1,paramentro2});
        }

        [HttpPost]
        public async Task<ActionResult> Post(Autor autor)
        {
            this.context.Add(autor);
            await this.context.SaveChangesAsync();
            return Ok();
            
        }

        [HttpPut("{id:int}")]
        public async Task<ActionResult> Put(int id, Autor autor)
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
        public async Task<ActionResult> Delete(int id)
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