using Entidades.Enumerados;
using System.Runtime.CompilerServices;

namespace Entidades.MetodosDeExtension
{
    public static class IngredientesExtension
    {
        /// <summary>
        /// Extiende la clase ListEIngredientes. Calcula costo total sumando el costo incial más el porcentaje
        /// de los ingredientes.
        /// </summary>
        /// <param name="ingredientes">Clase que se extiende</param>
        /// <param name="costoInicial">Costo base</param>
        /// <returns>El costo incrementado.</returns>
        public static double CalcularCostoIngredientes(this List<EIngrediente> ingredientes, int costoInicial)
        {
            double valorIncrementado = costoInicial;
            foreach (EIngrediente ingrediente in ingredientes)
            {
                valorIncrementado += costoInicial * (int)ingrediente / 100;                
            }
            return valorIncrementado;
        }

        /// <summary>
        /// Extiende la clase Random. Genera una lista de ingredientes, genera un numero random dentro de la cantidad de la lista 
        /// y retorna una lista con una cantidad aleatoria de ingredientes.
        /// </summary>
        /// <param name="rand">Clase que se extiende</param>
        /// <returns>Lista con cantidad aleatoria de ingredientes.</returns>
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
