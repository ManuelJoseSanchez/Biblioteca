using BibliotecaAPI.Entidades;
using Microsoft.AspNetCore.Mvc;

namespace BibliotecaAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ValorController : ControllerBase
    {
        private readonly IRepositorioValores repositorioValores;
        public readonly ServicioTransient Transient;
        public readonly ServicioTransient Transient1;
        public readonly ServicioScoped Scoped;
        public readonly ServicioScoped Scoped1;
        public readonly ServicioSinglento Singlento;

        public ValorController(IRepositorioValores repositorioValores,
        ServicioTransient transient,
        ServicioTransient transient1,
        ServicioScoped scoped,
        ServicioScoped scoped1,
        ServicioSinglento singlento)
        {
            this.repositorioValores = repositorioValores;
            Transient = transient;
            Transient1 = transient1;
            Scoped = scoped;
            Scoped1 = scoped1;
            Singlento = singlento;
        }


        [HttpGet]
        public IEnumerable<Valor> Get()
        {
            var respositorioValores = new RepositorioValores();
            return respositorioValores.ObtenerValores();
        }
        
    }
}