using BibliotecaAPI.Entidades;

namespace BibliotecaAPI
{
    public class RepositorioValores : IRepositorioValores
    {
        private List<Valor> _valores;


        public RepositorioValores()
        {
            this._valores = new List<Valor>
            {
                new Valor{Id=1, nombre="Valor 1"},
                new Valor{Id=1, nombre="valor 2"}
            };
        }
        public IEnumerable<Valor> ObtenerValores()
        {
            return this._valores;
        }

        public void InsertarValor(Valor valor)
        {
            this._valores.Add(valor);
        }

    }
}