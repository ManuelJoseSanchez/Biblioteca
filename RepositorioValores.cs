using BibliotecaAPI.Entidades;

namespace BibliotecaAPI
{
    public class RepositorioValores : IRepositorioValores
    {

        public IEnumerable<Valor> ObtenerValores()
        {
            return  new List<Valor>
            {
                new Valor{Id=1, nombre="Valor 1"},
                new Valor{Id=1, nombre="valor 2"}
            };
        }
        
    }
}