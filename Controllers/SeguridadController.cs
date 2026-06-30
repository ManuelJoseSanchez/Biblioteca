using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Mvc;

namespace BibliotecaAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class SeguridadController : ControllerBase
    {
        private readonly IDataProtector protection;
        private readonly ITimeLimitedDataProtector protectionLimitadoPorTiempo;

        public SeguridadController(IDataProtectionProvider protectionProvider)
        {
            this.protection = protectionProvider.CreateProtector("SeguridadController");
            this.protectionLimitadoPorTiempo =  this.protection.ToTimeLimitedDataProtector();
        }

        [HttpGet("encriptar")]
        public ActionResult Encriptar(string textoPlano)
        {
            string textoCifrado = this.protection.Protect(textoPlano);
            return Ok(new { textoCifrado });
        }

        [HttpGet("desencriptar")]
        public ActionResult Desencriptar(string textoCifrado)
        {
            string textoPlano = this.protection.Unprotect(textoCifrado);
            return Ok( new { textoPlano });
        }

        [HttpGet("encriptar-limitado-por-tiempo")]
        public ActionResult EncriptarLimitadoPorTiempo(string textoPlano)
        {
            string textoCifrado = this.protectionLimitadoPorTiempo.Protect(textoPlano, lifetime: TimeSpan.FromSeconds(30));
            return Ok(new { textoCifrado });
        }

        [HttpGet("desencriptar-limitado-por-tiempo")]
        public ActionResult DesencriptarLimitadoPorTiempo(string textoCifrado)
        {
            string textoPlano = this.protectionLimitadoPorTiempo.Unprotect(textoCifrado);
            return Ok(new { textoPlano });
        }
    }
}