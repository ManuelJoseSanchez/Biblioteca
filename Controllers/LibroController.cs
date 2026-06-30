using AutoMapper;
using BibliotecaAPI.Datos;
using BibliotecaAPI.DTOs;
using BibliotecaAPI.Entidades;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BibliotecaAPI.Controllers
{
    [ApiController]
    [Route("api/libros")]
    [Authorize(Policy = "esadmin")]
    public class LibroController : ControllerBase
    {
        private readonly ApplicationDbContext Context;
        public readonly IMapper mapper;
        private readonly ITimeLimitedDataProtector protectionProviderTiempo;

        public LibroController(ApplicationDbContext Context, IMapper mapper, IDataProtectionProvider protectionProvider)
        {
            this.Context = Context;
            this.mapper = mapper;
            protectionProviderTiempo = protectionProvider
                                        .CreateProtector("LibroController")
                                        .ToTimeLimitedDataProtector();
        }

        [HttpGet("listado/obtener-token")]
        public ActionResult ObtenerTokenListado()
        {
            var textoplano = Guid.NewGuid().ToString();
            var token = protectionProviderTiempo.Protect(textoplano, lifetime: TimeSpan.FromSeconds(30));
            var url = Url.RouteUrl("ObtenerListadoLibrosUsandoToken", new {token}, "http");
            return Ok(new { url });
        }

        [HttpGet("listado/{token}", Name = "ObtenerListadoLibrosUsandoToken")]
        [AllowAnonymous]
        public async Task<ActionResult> ObtenerListadoLibrosUsandoToken(string token)
        {
            try
            {
                protectionProviderTiempo.Unprotect(token);
            }
            catch
            {
                ModelState.AddModelError(nameof(token),"El token ha expirado");
                return ValidationProblem();
            }
            var libros = await this.Context.Libros.ToListAsync();
            var librosDTO = this.mapper.Map<IEnumerable<LibroDTO>>(libros);
            return Ok(librosDTO);
        }


        [HttpGet]
        public async Task<IEnumerable<LibroDTO>> Get()
        {
            var libros = await this.Context.Libros.ToListAsync();
            var librosDTO = this.mapper.Map<IEnumerable<LibroDTO>>(libros);
            return librosDTO;
        }

        [HttpGet("{id:int}", Name = "ObtenerLibro")]
        public async Task<ActionResult<LibroConAutorDTO>> Get(int id)
        {
            var libro = await this.Context.Libros
                        .Include(x => x.Autores)
                        .ThenInclude(x => x.Autor)
                        .FirstOrDefaultAsync(x => x.Id == id);
            if (libro is null)
            {
                return NotFound();
            }
            var libroDTO = this.mapper.Map<LibroConAutorDTO>(libro);
            return libroDTO;
        }
        [HttpPost]
        public async Task<ActionResult> Post(LibroCreacionDTO libroCreacion)
        {
            if (libroCreacion.AutoresIds is null || libroCreacion.AutoresIds.Count == 0)
            {
                ModelState.AddModelError(nameof(libroCreacion.AutoresIds), "No se puede crear un libro sin autores");
                return ValidationProblem();
            }
            var autorIdsExisten = await this.Context.Autores.Where(x => libroCreacion.AutoresIds.Contains(x.Id)).Select(x => x.Id).ToListAsync();
            if (autorIdsExisten.Count != libroCreacion.AutoresIds.Count)
            {
                var autorIdsNoExisten = libroCreacion.AutoresIds.Except(autorIdsExisten);
                ModelState.AddModelError(nameof(libroCreacion.AutoresIds),
                 $"Algunos de los autores del id {string.Join(", ", autorIdsNoExisten)} no existen");
                return ValidationProblem();
            }
            var libro = this.mapper.Map<Libro>(libroCreacion);
            this.AsignarOrdenAutores(libro);
            this.Context.Add(libro);
            await this.Context.SaveChangesAsync();
            var libroDTO = this.mapper.Map<LibroDTO>(libro);
            return CreatedAtRoute("ObtenerLibro", new { id = libro.Id }, libroDTO);

        }
        private void AsignarOrdenAutores(Libro libro)
        {
            if (libro.Autores is not null)
            {
                for (int i = 0; i < libro.Autores.Count; i++)
                {
                    libro.Autores[i].Order = i;
                }
            }
        }
        [HttpPut("{id:int}")]
        public async Task<ActionResult> Put(int id, LibroCreacionDTO libroCreacionDTO)
        {
            if (libroCreacionDTO.AutoresIds is null || libroCreacionDTO.AutoresIds.Count == 0)
            {
                ModelState.AddModelError(nameof(libroCreacionDTO.AutoresIds), "No se puede crear un libro sin autores");
                return ValidationProblem();
            }
            var autorIdsExisten = await this.Context.Autores
                    .Where(x => libroCreacionDTO.AutoresIds.Contains(x.Id))
                    .Select(x => x.Id).ToListAsync();
            if (autorIdsExisten.Count != libroCreacionDTO.AutoresIds.Count)
            {
                var autorIdsNoExisten = libroCreacionDTO.AutoresIds.Except(autorIdsExisten);
                ModelState.AddModelError(nameof(libroCreacionDTO.AutoresIds),
                 $"Algunos de los autores del id {string.Join(", ", autorIdsNoExisten)} no existen");
                return ValidationProblem();
            }
            var libroDB = await this.Context.Libros.Include(x => x.Autores).FirstOrDefaultAsync(x => x.Id == id);
            if (libroDB is null)
            {
                return NotFound();
            }
            this.mapper.Map(libroCreacionDTO, libroDB);
            this.AsignarOrdenAutores(libroDB);
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