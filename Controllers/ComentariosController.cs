using System.ComponentModel;
using System.Reflection.Metadata.Ecma335;
using AutoMapper;
using AutoMapper.Configuration.Annotations;
using Biblioteca.DTOs;
using Biblioteca.Entidades;
using BibliotecaAPI.Datos;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualBasic;

namespace Biblioteca.Controllers
{
    [ApiController]
    [Route("api/libros/{librosId:int}/[controller]")]
    public class ComentariosController : ControllerBase
    {
        private readonly ApplicationDbContext context;
        private readonly IMapper mapper;

        public ComentariosController(ApplicationDbContext context, IMapper mapper)
        {
            this.context = context;
            this.mapper = mapper;
        }

        [HttpGet]
        public async Task<ActionResult<List<ComentarioDTO>>> Get(int libroId)
        {
            var isBook = await this.context.Libros.AnyAsync(x => x.Id == libroId);
            if (!isBook)
            {
                return NotFound();
            }

            var comentarios = await this.context.Comentarios.Where(x => x.LibroId == libroId)
            .OrderByDescending(x => x.FechaPublicacion)
            .ToListAsync();
            return this.mapper.Map<List<ComentarioDTO>>(comentarios);
        }

        [HttpGet("{id}", Name = "ObetenerComentario")]
        public async Task<ActionResult<ComentarioDTO>> Get(Guid id)
        {
            var comentario = await this.context.Comentarios.FirstOrDefaultAsync(x => x.Id == id);
            if (comentario is null)
            {
                return NotFound();
            }
            return this.mapper.Map<ComentarioDTO>(comentario);
        }

        [HttpPost]
        public async Task<ActionResult> Post(int libroId, ComentarioCreacionDTO comentarioCreacionDTO)
        {
            var isBook = await this.context.Libros.AnyAsync(x => x.Id == libroId);
            if (!isBook)
            {
                return NotFound();
            }
            var comentario = this.mapper.Map<Comentario>(comentarioCreacionDTO);
            comentario.LibroId = libroId;
            comentario.FechaPublicacion = DateTime.UtcNow;
            this.context.Add(comentario);
            await this.context.SaveChangesAsync();

            var ComentarioDTO = this.mapper.Map<ComentarioDTO>(comentario);

            return CreatedAtRoute("ObetenerComentario",
             new { id = comentario.Id, libroId },
             comentario);
        }

        [HttpPatch("{id}")]
        public async Task<ActionResult> Patch(Guid id, int libroId, JsonPatchDocument<ComentarioPatchDTO> comentarioPatchDTO)
        {
            if (comentarioPatchDTO is null)
            {
                return BadRequest();
            }
            var isBook = await this.ExisteLibro(libroId);
            if (!isBook)
            {
                return NotFound();
            }
            var CometarioDB = await this.context.Comentarios.FirstOrDefaultAsync(x => x.Id == id);
            if (CometarioDB is null)
            {
                return NotFound();
            }
            var comentariosPatch = this.mapper.Map<ComentarioPatchDTO>(CometarioDB);
            comentarioPatchDTO.ApplyTo(comentariosPatch, ModelState);
            var isValidate = TryValidateModel(comentariosPatch);
            if (!isValidate)
            {
                return ValidationProblem();
            }
            this.mapper.Map(comentariosPatch, CometarioDB);
            await this.context.SaveChangesAsync();
            return NoContent();

        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> Delete(Guid id, int libroId)
        {
            var isBook = await this.ExisteLibro(libroId);
            if (!isBook)
            {
                return NotFound();
            }

            var registrosBorrados = await this.context.Comentarios.Where(y => y.Id == id).ExecuteDeleteAsync();
            if (registrosBorrados == 0)
            {
                return NotFound();
            }

            return NoContent();
        }

        private async Task<bool> ExisteLibro(int id)
        {
            var isBook = await this.context.Libros.AnyAsync(x => x.Id == id);
            return isBook;
        }

    }
}