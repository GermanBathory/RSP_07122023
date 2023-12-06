using Entidades.DataBase;
using Entidades.Exceptions;
using Entidades.Files;
using Entidades.Interfaces;
using System.ComponentModel.Design;

namespace Entidades.Modelos
{
    public delegate void DelegadoNuevoIngreso(IComestible menu);
    public delegate void DelegadoDemoraAtencion(double demora);

    public class Cocinero<T> where T : IComestible, new()
    {
        private int cantPedidosFinalizados;
        private string nombre;
        private T menu;
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

        private void IniciarIngreso()
        {
            CancellationToken token = this.cancellation.Token;
            this.tarea = Task.Run(() =>
            {
                while (!cancellation.IsCancellationRequested)
                {
                    this.NotificarNuevoIngreso();
                    this.EsperarProximoIngreso();
                    this.cantPedidosFinalizados++;

                    try
                    {
                        DataBaseManager.GuardarTicket<T>(this.Nombre, this.menu);
                    }
                    catch (DataBaseManagerException ex)
                    {
                        FileManager.Guardar(ex.Message, "logs.txt", true);
                    }
                }
            }, token);
        }

        private void NotificarNuevoIngreso()
        {
            if (this.OnIngreso is not null)
            {
                this.menu = new T();
                menu.IniciarPreparacion();
                this.OnIngreso.Invoke(menu);
            }
        }
        private void EsperarProximoIngreso()
        {
            int tiempoEspera = 0;


            while (this.OnDemora is not null && !cancellation.IsCancellationRequested && !this.menu.Estado)
            {
                tiempoEspera++;
                this.OnDemora.Invoke(tiempoEspera);
                Thread.Sleep(1000);
            }
            this.demoraPreparacionTotal += tiempoEspera;
        }

    }
}
