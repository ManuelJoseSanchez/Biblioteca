using Microsoft.AspNetCore.Identity;

namespace BibliotecaAPI.Servicios
{
    public class ServiciosUsuarios : IServiciosUsuarios
    {
        private readonly UserManager<IdentityUser> userManager;
        private readonly IHttpContextAccessor httpContextAccessor;

        public ServiciosUsuarios(UserManager<IdentityUser> userManager, IHttpContextAccessor httpContextAccessor)
        {
            this.userManager = userManager;
            this.httpContextAccessor = httpContextAccessor;
        }

        public async Task<IdentityUser?> ObtenerUsuario()
        {
            var emailClaim = this.httpContextAccessor.HttpContext!
                            .User.Claims.Where(x => x.Type == "email").FirstOrDefault();
            if (emailClaim is null)
            {
                return null;
            }

            var email = emailClaim.Value;
            return await this.userManager.FindByEmailAsync(email);
        }
    }
}