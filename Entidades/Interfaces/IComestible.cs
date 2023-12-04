namespace Entidades.Interfaces
{
    public interface IComestible
    {
        public bool Estado { get; }
        public string Imagen { get; }
        public string Ticket { get; }

        public void IniciarPreparacion();
        public void FinalizarPreparacion(string cocinero);

    }
}
