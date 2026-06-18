using AutoMapper;
using Biblioteca.DTOs;
using Biblioteca.Entidades;
using BibliotecaAPI.Datos;
using BibliotecaAPI.Servicios;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;


namespace Biblioteca.Controllers
{
    [ApiController]
    [Route("api/libros/{librosId:int}/[controller]")]
    [Authorize]
    public class ComentariosController : ControllerBase
    {
        private readonly ApplicationDbContext context;
        private readonly IMapper mapper;
        private readonly IServiciosUsuarios serviciosUsuarios;

        public ComentariosController(ApplicationDbContext context, IMapper mapper, IServiciosUsuarios serviciosUsuarios)
        {
            this.context = context;
            this.mapper = mapper;
            this.serviciosUsuarios = serviciosUsuarios;
        }

        [HttpGet]
        public async Task<ActionResult<List<ComentarioDTO>>> Get(int librosId)
        {
            var isBook = await this.context.Libros.AnyAsync(x => x.Id == librosId);
            if (!isBook)
            {
                return NotFound();
            }

            var comentarios = await this.context.Comentarios
            .Include(x => x.Usuario)
            .Where(x => x.LibroId == librosId)
            .OrderByDescending(x => x.FechaPublicacion)
            .ToListAsync();
            return this.mapper.Map<List<ComentarioDTO>>(comentarios);
        }

        [HttpGet("{id}", Name = "ObetenerComentario")]
        public async Task<ActionResult<ComentarioDTO>> Get(Guid id)
        {
            var comentario = await this.context.Comentarios
            .Include(x => x.Usuario)
            .FirstOrDefaultAsync(x => x.Id == id);
            if (comentario is null)
            {
                return NotFound();
            }
            return this.mapper.Map<ComentarioDTO>(comentario);
        }

        [HttpPost]
        public async Task<ActionResult> Post(int librosId, ComentarioCreacionDTO comentarioCreacionDTO)
        {
            Console.WriteLine(librosId);
            var isBook = await this.context.Libros.AnyAsync(x => x.Id == librosId);
            if (!isBook)
            {
                return NotFound();
            }
            var usuario = await this.serviciosUsuarios.ObtenerUsuario();
            if(usuario is null)
            {
                return NotFound();
            }
            var comentario = this.mapper.Map<Comentario>(comentarioCreacionDTO);
            comentario.LibroId = librosId;
            comentario.FechaPublicacion = DateTime.UtcNow;
            comentario.UsuarioId=usuario.Id;
            this.context.Add(comentario);
            await this.context.SaveChangesAsync();

            var ComentarioDTO = this.mapper.Map<ComentarioDTO>(comentario);

            return CreatedAtRoute("ObetenerComentario",
             new { id = comentario.Id, librosId },
             ComentarioDTO);
        }

        [HttpPatch("{id}")]
        public async Task<ActionResult> Patch(Guid id, int librosId, JsonPatchDocument<ComentarioPatchDTO> comentarioPatchDTO)
        {
            if (comentarioPatchDTO is null)
            {
                return BadRequest();
            }
            var isBook = await this.ExisteLibro(librosId);
            if (!isBook)
            {
                return NotFound();
            }
            var usuario = await this.serviciosUsuarios.ObtenerUsuario();
            if(usuario is null)
            {
                return NotFound();
            }

            var CometarioDB = await this.context.Comentarios.FirstOrDefaultAsync(x => x.Id == id);
            if (CometarioDB is null)
            {
                return NotFound();
            }
            if(CometarioDB.UsuarioId != usuario.Id)
            {
                return Forbid();
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
        public async Task<ActionResult> Delete(Guid id, int librosId)
        {
            var isBook = await this.ExisteLibro(librosId);
            if (!isBook)
            {
                return NotFound();
            }
            var usuario = await serviciosUsuarios.ObtenerUsuario();
            if(usuario is null)
            {
                return NotFound();
            }

            var comentarioDB = await this.context.Comentarios.FirstOrDefaultAsync(x => x.Id == id);
            if(comentarioDB is null)
            {
                return NotFound();
            }

            if(comentarioDB.UsuarioId != usuario.Id)
            {
                return Forbid();
            }
            this.context.Remove(comentarioDB);
            await this.context.SaveChangesAsync();
            return NoContent();
        }

        private async Task<bool> ExisteLibro(int id)
        {
            var isBook = await this.context.Libros.AnyAsync(x => x.Id == id);
            return isBook;
        }

    }
}