using BibliotecaAPI.DTOs;

namespace BibliotecaAPI.Servicios
{
    public interface IServicioHash
    {
        
        public ResultadoHashDTO Hash(string input);
        public ResultadoHashDTO Hash(string input, byte[] sal);
    }
}