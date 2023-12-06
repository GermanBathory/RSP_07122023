using Entidades.Exceptions;
using Entidades.Files;
using Entidades.Modelos;

namespace MisTest
{
    [TestClass]
    public class TestCocina
    {
        [TestMethod]
        [ExpectedException(typeof(FileManagerException))]
        public void AlGuardarUnArchivo_ConNombreInvalido_TengoUnaExcepcion()
        {
            //arrange
            string nombreArchivo = "*";
            string data = "Prueba";

            //act
            FileManager.Guardar(data, nombreArchivo, true);

            //assert
        }

        [TestMethod]

        public void AlInstanciarUnCocinero_SeEspera_PedidosCero()
        {
            //arrange
            Cocinero<Hamburguesa> cocinero = new Cocinero<Hamburguesa>("GatoDumas");
            bool resultado = false;

            //act
            if (cocinero.TiempoMedioDePreparacion == 0)
            {
                resultado = true;              
            }

            //assert
            Assert.IsTrue(resultado);
        }
    }
}