using Entidades.Enumerados;
using System.Runtime.CompilerServices;

namespace Entidades.MetodosDeExtension
{
    public static class IngredientesExtension
    {
        /// <summary>
        /// Calcula costo sumando el costo de los ingredientes al costo base.
        /// </summary>
        /// <param name="ingredientes">Lista de ingredientes</param>
        /// <param name="costoInicial">Costo base</param>
        /// <returns>El valor incrementado</returns>
        public static double CalcularCostoIngredientes(this List<EIngrediente> ingredientes, int costoInicial)
        {
            double valorIncrementado = 0;
            foreach (EIngrediente ingrediente in ingredientes)
            {
                valorIncrementado = costoInicial + (costoInicial * (int)ingrediente / 100);
            }
            return valorIncrementado;
        }

        public static List<EIngrediente> IngredientesAleatorios(this Random rand)
        {
            List<EIngrediente> ingredientes = new List<EIngrediente>()
            {
                EIngrediente.QUESO,
                EIngrediente.PANCETA,
                EIngrediente.ADHERESO,
                EIngrediente.HUEVO,
                EIngrediente.JAMON,
            };

            int aleatorio = rand.Next(1, ingredientes.Count + 1);

            return ingredientes.Take(aleatorio).ToList();
        }

    }
}
