using Entidades.DataBase;
using Entidades.Exceptions;
using Entidades.Files;
using Entidades.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Entidades.Modelos
{
    public delegate void DelegadoNuevoPedido<T>(T menu);
    public class Mozo<T> where T : IComestible, new()
    {
        private T menu;
        private CancellationTokenSource cancellation;
        private Task tarea;

        public event DelegadoNuevoPedido<T> OnPedido;


        public bool EmpezarATrabajar
        {
            get
            {
                return this.tarea is not null && (this.tarea.Status == TaskStatus.Running ||
                    this.tarea.Status == TaskStatus.WaitingToRun ||
                    this.tarea.Status == TaskStatus.WaitingForActivation);
            }
            set
            {
                if (value && !this.EmpezarATrabajar
)
                {
                    this.cancellation = new CancellationTokenSource();
                    this.TomarPedidos();
                }
                else
                {
                    this.cancellation.Cancel();
                }
            }
        }

        private void TomarPedidos()
        {
            CancellationToken token = this.cancellation.Token;
            this.tarea = Task.Run(() =>
            {
                while (!cancellation.IsCancellationRequested)
                {
                    this.NotificarNuevoPedido();
                    Thread.Sleep(5000);
                }
            }, token);
        }

        private void NotificarNuevoPedido()
        {
            if (this.OnPedido is not null)
            {
                this.menu = new T();
                menu.IniciarPreparacion();
                this.OnPedido.Invoke(menu);
            }
        }


    }
}
