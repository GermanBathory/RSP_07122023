using Entidades.DataBase;
using Entidades.Exceptions;
using Entidades.Files;
using Entidades.Interfaces;
using System.ComponentModel.Design;

namespace Entidades.Modelos
{
    public delegate void DelegadoPedidoEnCurso(IComestible menu);
    public delegate void DelegadoDemoraAtencion(double demora);

    public class Cocinero<T> where T : IComestible, new()
    {
        private int cantPedidosFinalizados;
        private string nombre;
        private T pedidoEnPreparacion;
        private Mozo <T> mozo;
        private Queue<T> pedidos;
        private double demoraPreparacionTotal;
        private CancellationTokenSource cancellation;

        private Task tarea;

        public event DelegadoPedidoEnCurso OnPedido;
        public event DelegadoDemoraAtencion OnDemora;



        public Cocinero(string nombre)
        {
            this.nombre = nombre;
            this.mozo = new Mozo<T>();
            this.pedidos = new Queue<T>();
            this.mozo.OnPedido += this.TomarNuevoPedido;
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
                    this.mozo.EmpezarATrabajar = true;
                    this.EmpezarACocinar();

                }
                else
                {
                    this.cancellation.Cancel();
                    this.mozo.EmpezarATrabajar = false;
                }
            }
        }

        //no hacer nada
        public double TiempoMedioDePreparacion { get => this.cantPedidosFinalizados == 0 ? 0 : this.demoraPreparacionTotal / this.cantPedidosFinalizados; }
        public string Nombre { get => nombre; }
        public int CantPedidosFinalizados { get => cantPedidosFinalizados; }
        public Queue<T> Pedidos { get => pedidos; }

        private void EmpezarACocinar()
        {
            CancellationToken token = this.cancellation.Token;
            this.tarea = Task.Run(() =>
            {
                while (!cancellation.IsCancellationRequested)
                {
                    if (pedidos.Count > 0)
                    {
                        this.pedidoEnPreparacion = pedidos.Dequeue();
                        this.OnPedido.Invoke(this.pedidoEnPreparacion);

                        this.EsperarProximoIngreso();
                        this.cantPedidosFinalizados++;

                        try
                        {
                            DataBaseManager.GuardarTicket<T>(this.Nombre, this.pedidoEnPreparacion);
                        }
                        catch (DataBaseManagerException ex)
                        {
                            FileManager.Guardar(ex.Message, "logs.txt", true);
                        }
                    }

                }
            }, token);
        }

        private void TomarNuevoPedido (T menu)
        {
            if (this.OnPedido is not null)
            {
                this.pedidos.Enqueue(menu);
            }
        }

        private void EsperarProximoIngreso()
        {
            int tiempoEspera = 0;


            while (this.OnDemora is not null && !cancellation.IsCancellationRequested && !this.pedidoEnPreparacion.Estado)
            {
                tiempoEspera++;
                this.OnDemora.Invoke(tiempoEspera);
                Thread.Sleep(1000);
            }
            this.demoraPreparacionTotal += tiempoEspera;
        }

    }
}
