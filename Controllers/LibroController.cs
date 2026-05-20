
using BibliotecaAPI.Datos;
using BibliotecaAPI.Entidades;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BibliotecaAPI.Controllers
{
    [ApiController]
    [Route("api/libros")]
    public class LibroController : ControllerBase
    {
        private readonly ApplicationDbContext Context;
        public LibroController(ApplicationDbContext Context)
        {
            this.Context = Context;
        }

        [HttpGet]
        public async Task<IEnumerable<Libro>> Get()
        {
            return await this.Context.Libros.ToListAsync();
        }

        [HttpGet("{id:int}")]
        public async Task<ActionResult<Libro>> Get(int id)
        {
            var libro = await this.Context.Libros.Include(x => x.autor).FirstOrDefaultAsync(x => x.Id == id);
            if(libro is null)
            {
                return NotFound();
            }
            return libro;
        }
        [HttpPost]
        public async Task<ActionResult> Post(Libro libro)
        {
            var autor = await this.Context.Autores.AnyAsync(x => x.Id == libro.AutorId);
            if(!autor)
            {
                return BadRequest($"El autor de libro no exite");
            }
            this.Context.Add(libro);
            await this.Context.SaveChangesAsync();
            return Ok();

        }
        [HttpPut("{id:int}")]
        public async Task<ActionResult> Put(int id, Libro libro)
        {
            if(id != libro.Id)
            {
                return BadRequest("El libro no coinciden");
            }
            var autor = await this.Context.Autores.AnyAsync(x => x.Id == libro.AutorId);
            if(!autor)
            {
                return BadRequest($"El autor de libro no exite");
            }
            this.Context.Update(libro);
            await this.Context.SaveChangesAsync();
            return Ok();
        }

        [HttpDelete("{id:int}")]
        public async Task<ActionResult> Delete(int id)
        {
            var registrosBorrados = await this.Context.Libros.Where(x => x.Id == id).ExecuteDeleteAsync() ;
            if (registrosBorrados == 0)
            {
                return NotFound();
            }
            return Ok();
        }
    }
}