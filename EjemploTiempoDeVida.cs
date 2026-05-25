namespace BibliotecaAPI
{
    public class ServicioTransient
    {
        private readonly Guid _id;

        public ServicioTransient()
        {
            _id=Guid.NewGuid();
        }

        public Guid ObternerGuid => _id;
    }

    public class ServicioScoped
    {
        private readonly Guid _id;

        public ServicioScoped()
        {
            _id=Guid.NewGuid();
        }

        public Guid ObtenerGuid => _id;

    }

    public class ServicioSinglento
    {
        private readonly Guid _id;

        public ServicioSinglento()
        {
            _id =  Guid.NewGuid();
        }

        public Guid ObternerGuid => _id;
    }
    
}