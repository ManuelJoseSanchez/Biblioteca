using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Biblioteca.DTOs;
using BibliotecaAPI.Servicios;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;

namespace Biblioteca.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UsuariosController : ControllerBase
    {
        private readonly UserManager<IdentityUser> userManager;
        private readonly IConfiguration configuration;
        private readonly SignInManager<IdentityUser> signInManager;
        private readonly IServiciosUsuarios serviciosUsuarios;

        public UsuariosController(UserManager<IdentityUser> userManager, IConfiguration configuration,
        SignInManager<IdentityUser> signInManager, IServiciosUsuarios serviciosUsuarios)
        {
            this.userManager = userManager;
            this.configuration = configuration;
            this.signInManager = signInManager;
            this.serviciosUsuarios = serviciosUsuarios;
        }

        [HttpPost("registro")]
        public async Task<ActionResult<RespuestaAutenticacionDTO>> Registro(CredencialesUsuarioDTO credencialesUsuarioDTO)
        {
            var usuario = new IdentityUser
            {
                UserName = credencialesUsuarioDTO.Email,
                Email = credencialesUsuarioDTO.Email,
            };

            var resultado = await this.userManager.CreateAsync(usuario, credencialesUsuarioDTO.Password!);

            if (resultado.Succeeded)
            {
                var respuestaAutenticacion = await this.ConstruirToken(credencialesUsuarioDTO);
                return respuestaAutenticacion;
            }
            else
            {
                foreach (var error in resultado.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
                return ValidationProblem();
            }
        }

        [HttpPost("login")]
        public async Task<ActionResult<RespuestaAutenticacionDTO>> Login(CredencialesUsuarioDTO credencialesUsuarioDTO)
        {
            var usuario = await this.userManager.FindByEmailAsync(credencialesUsuarioDTO.Email);

            if (usuario is null)
            {
                return this.RetornarLoginIncorrecto();
            }

            var resultado = await this.signInManager.CheckPasswordSignInAsync(usuario, credencialesUsuarioDTO.Password!, lockoutOnFailure: false);

            if (resultado.Succeeded)
            {
                return await this.ConstruirToken(credencialesUsuarioDTO);
            }
            else
            {
                return this.RetornarLoginIncorrecto();
            }

        }

        [HttpGet("renovar-token")]
        [Authorize]
        public async Task<ActionResult<RespuestaAutenticacionDTO>> RenovarToken()
        {
            var usuario = await this.serviciosUsuarios.ObtenerUsuario();
            if (usuario is null)
            {
                return NotFound();
            }

            var credencialesUsuarioDto = new CredencialesUsuarioDTO { Email = usuario.Email! };
            var respuestaAutenticacion = await this.ConstruirToken(credencialesUsuarioDto);
            return respuestaAutenticacion;
        }

        [HttpPost("hacer-admin")]
        [Authorize(Policy = "esadmin")]
        public async Task<ActionResult> HecerAdmin(EditarClaimDTO editarClaimDTO)
        {
            var usuario = await this.userManager.FindByEmailAsync(editarClaimDTO.Email);
            if (usuario is null)
            {
                return NotFound();
            }
            await this.userManager.AddClaimAsync(usuario, new Claim("esadmin", "true"));
            return NoContent();
        }

        [HttpPost("remover-admin")]
        [Authorize(Policy = "esadmin")]
        public async Task<ActionResult> RemoverAdmin(EditarClaimDTO editarClaimDTO)
        {
            var usuario = await this.userManager.FindByEmailAsync(editarClaimDTO.Email);
            if (usuario is null)
            {
                return NotFound();
            }
            await this.userManager.RemoveClaimAsync(usuario, new Claim("esadmin", "true"));
            return NoContent();
        }

        private ActionResult RetornarLoginIncorrecto()
        {
            ModelState.AddModelError(string.Empty, "Login incorrecto");
            return ValidationProblem();
        }

        private async Task<RespuestaAutenticacionDTO> ConstruirToken(CredencialesUsuarioDTO credencialesUsuarioDTO)
        {
            var claims = new List<Claim>
            {
                new Claim("email", credencialesUsuarioDTO.Email),
                new Claim("lo que yo quiera","cualquier valor")
            };

            var usuario = await this.userManager.FindByEmailAsync(credencialesUsuarioDTO.Email);
            var claimsDB = await this.userManager.GetClaimsAsync(usuario!);

            claims.AddRange(claimsDB);

            var llave = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(this.configuration["llavejwt"]!));
            var credenciales = new SigningCredentials(llave, SecurityAlgorithms.HmacSha256);

            var expiracion = DateTime.UtcNow.AddDays(1);

            var tokenDeSeguridad = new JwtSecurityToken(issuer: null, audience: null, claims: claims, expires: expiracion, signingCredentials: credenciales);

            var token = new JwtSecurityTokenHandler().WriteToken(tokenDeSeguridad);

            return new RespuestaAutenticacionDTO
            {
                Token = token,
                Expiracion = expiracion
            };
        }


    }
}