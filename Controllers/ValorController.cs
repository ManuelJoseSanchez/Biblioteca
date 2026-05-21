using BibliotecaAPI.Entidades;
using Microsoft.AspNetCore.Mvc;

namespace BibliotecaAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ValorController : ControllerBase
    {
        private readonly IRepositorioValores repositorioValores;

        public ValorController(IRepositorioValores repositorioValores)
        {
            this.repositorioValores = repositorioValores;
        }


        [HttpGet]
        public IEnumerable<Valor> Get()
        {
            var respositorioValores = new RepositorioValores();
            return respositorioValores.ObtenerValores();
        }
        
    }
}