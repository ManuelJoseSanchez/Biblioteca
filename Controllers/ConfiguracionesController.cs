using Microsoft.AspNetCore.Mvc;

namespace BibliotecaAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ConfiguracionesController : ControllerBase
    {
        private readonly IConfiguration configuration;

        public ConfiguracionesController(IConfiguration configuration)
        {
            this.configuration = configuration;
        }

        [HttpGet]
        public ActionResult<string> Get()
        {
            var opcion1 = this.configuration["apellido"];
            var opcion2 = this.configuration.GetValue<string>("apellido");
            return opcion2!;
        }

        [HttpGet("secciones")]
        public ActionResult<string> GetSeccion()
        {
            var opcion1 = this.configuration["ConnectionStrings:DefaultConnection"];
            var opcion2 = this.configuration.GetValue<string>("ConnectionStrings:DefaultConnection");
            var seccion =  this.configuration.GetSection("ConnectionStrings");
            var opcion3 = seccion.GetValue<string>("DefaultConnection");
            return opcion3!;
        }
    }
}