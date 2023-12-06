namespace Entidades.Interfaces
{
    public interface IComestible
    {
        bool Estado { get; }
        string Imagen { get; }
        string Ticket { get; }

        void IniciarPreparacion();
        void FinalizarPreparacion(string cocinero);

    }
}
