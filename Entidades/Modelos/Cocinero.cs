using Entidades.DataBase;
using Entidades.Exceptions;
using Entidades.Files;
using Entidades.Interfaces;


namespace Entidades.Modelos
{
    public delegate void DelegadoNuevoIngreso(IComestible menu);
    public delegate void DelegadoDemoraAtencion(double demora);

    public class Cocinero<T> where T : IComestible, new()
    {
        private int cantPedidosFinalizados;
        private string nombre;
        private double demoraPreparacionTotal;
        private CancellationTokenSource cancellation;

        private Task tarea;

        public event DelegadoNuevoIngreso OnIngreso;
        public event DelegadoDemoraAtencion OnDemora;



        public Cocinero(string nombre)
        {
            this.nombre = nombre;
        }

        //No hacer nada
        public bool HabilitarCocina
        {
            get
            {
                return this.tarea is not null && (this.tarea.Status == TaskStatus.Running ||
                    this.tarea.Status == TaskStatus.WaitingToRun ||
                    this.tarea.Status == TaskStatus.WaitingForActivation);
            }
            set
            {
                if (value && !this.HabilitarCocina)
                {
                    this.cancellation = new CancellationTokenSource();
                    this.IniciarIngreso();
                }
                else
                {
                    this.cancellation.Cancel();
                }
            }
        }

        //no hacer nada
        public double TiempoMedioDePreparacion { get => this.cantPedidosFinalizados == 0 ? 0 : this.demoraPreparacionTotal / this.cantPedidosFinalizados; }
        public string Nombre { get => nombre; }
        public int CantPedidosFinalizados { get => cantPedidosFinalizados; }

        private void IniciarIngreso()
        {
            this.tarea = Task.Run(() =>
            {
                while (!cancellation.IsCancellationRequested)
                {
                    this.NotificarNuevoIngreso();
                    this.EsperarProximoIngreso();
                    this.cantPedidosFinalizados++;
                    T comida = new T();
                    DataBaseManager.GuardarTicket<T>(this.Nombre, comida);
                }
            });
        }

        private void NotificarNuevoIngreso()
        {
            if (this.OnIngreso is not null)
            {
                T menu = new T();
                menu.IniciarPreparacion();
                menu.ToString();
                this.OnIngreso.Invoke(menu);
            }
        }
        private void EsperarProximoIngreso()
        {
            int tiempoEspera = 0;
            T comida = new T();

            if (this.OnDemora is not null)
            {
                this.tarea = Task.Run(() =>
                {
                    while (!cancellation.IsCancellationRequested && !comida.Estado)
                    {
                        Thread.Sleep(1000);
                        tiempoEspera++;
                    }
                });
            }
            this.OnDemora.Invoke(this.demoraPreparacionTotal += tiempoEspera);
        }

    }
}
