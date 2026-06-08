
using AutoMapper;
using BibliotecaAPI.Datos;
using BibliotecaAPI.DTOs;
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
        public readonly IMapper mapper;
        public LibroController(ApplicationDbContext Context, IMapper mapper)
        {
            this.Context = Context;
            this.mapper = mapper;
        }

        [HttpGet]
        public async Task<IEnumerable<LibroDTO>> Get()
        {
            var libros = await this.Context.Libros.ToListAsync();
            var librosDTO = this.mapper.Map<IEnumerable<LibroDTO>>(libros);
            return librosDTO;
        }

        [HttpGet("{id:int}", Name = "ObtenerLibro")]
        public async Task<ActionResult<LibroDTO>> Get(int id)
        {
            var libro = await this.Context.Libros.Include(x => x.autor).FirstOrDefaultAsync(x => x.Id == id);
            if (libro is null)
            {
                return NotFound();
            }
            var libroDTO = this.mapper.Map<LibroDTO>(libro);
            return libroDTO;
        }
        [HttpPost]
        public async Task<ActionResult> Post(LibroCreacionDTO libroCreacion)
        {
            var autor = await this.Context.Autores.AnyAsync(x => x.Id == libroCreacion.AutorId);
            if (!autor)
            {
                ModelState.AddModelError(nameof(libroCreacion.AutorId), $"El autor del id {libroCreacion.AutorId} no existe");
                return ValidationProblem();
            }
            var libro = this.mapper.Map<Libro>(libroCreacion);
            this.Context.Add(libro);
            await this.Context.SaveChangesAsync();
            var libroDTO = this.mapper.Map<LibroDTO>(libro);
            return CreatedAtRoute("ObtenerLibro", new { id = libro.Id }, libroDTO);

        }
        [HttpPut("{id:int}")]
        public async Task<ActionResult> Put(int id, LibroCreacionDTO libroCreacionDTO)
        {
            var libro = this.mapper.Map<Libro>(libroCreacionDTO);
            libro.Id = id;
            var autor = await this.Context.Autores.AnyAsync(x => x.Id == libro.AutorId);
            if (!autor)
            {
                return BadRequest($"El autor de libro no exite");
            }
            this.Context.Update(libro);
            await this.Context.SaveChangesAsync();
            return NoContent();
        }

        [HttpDelete("{id:int}")]
        public async Task<ActionResult> Delete(int id)
        {
            var registrosBorrados = await this.Context.Libros.Where(x => x.Id == id).ExecuteDeleteAsync();
            if (registrosBorrados == 0)
            {
                return NotFound();
            }
            return NoContent();
        }
    }
}