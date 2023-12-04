using System.Data.SqlClient;
using Entidades.Excepciones;
using Entidades.Exceptions;
using Entidades.Files;
using Entidades.Interfaces;

namespace Entidades.DataBase
{
    public static class DataBaseManager
    {
        private static SqlConnection connection;
        private static string stringConnection;

        static DataBaseManager()
        {
            DataBaseManager.stringConnection = "Server=.;Database=20230622SP;Trusted_Connection=True;";
        }

        public static string GetImagenComida(string tipo)
        {
            try
            {
                using (DataBaseManager.connection = new SqlConnection(DataBaseManager.stringConnection))
                {
                    string query = "SELECT * FROM comidas WHERE tipo_comida=@tipo";

                    SqlCommand command = new SqlCommand(query, connection);
                    command.Parameters.AddWithValue("tipo", tipo);
                    DataBaseManager.connection.Open();
                    SqlDataReader reader = command.ExecuteReader();

                    if (reader.Read())
                    {
                        return reader.GetString(2);
                    }
                }
                throw new ComidaInvalidaExeption("Comida inexistente");
            }
            catch (Exception ex)
            {
                throw new DataBaseManagerException("Error al intentar leer la BD", ex);
                FileManager.Guardar(ex.Message, "logs.txt", true);
            }
        }

        public static bool GuardarTicket<T>(string nombreEmpleado, T comida) where T : IComestible, new()
        {
            try
            {
                using (DataBaseManager.connection = new SqlConnection(DataBaseManager.stringConnection))
                {
                    string query = "INSERT INTO tickets (empleado,ticket)" +
                        "values (@empleado,@ticket); SELECT @@IDENTITY";

                    SqlCommand command = new SqlCommand(query, connection);
                    command.Parameters.AddWithValue("empleado", nombreEmpleado);
                    command.Parameters.AddWithValue("ticket", comida.Ticket);
                    DataBaseManager.connection.Open();
                    SqlDataReader reader = command.ExecuteReader();

                    reader.Read();
                    reader.GetDecimal(0);
                    return true;
                }
            }
            catch (Exception ex)
            {
                throw new DataBaseManagerException("Error al intentar escribir la BD", ex);
                FileManager.Guardar(ex.Message, "logs.txt", true);
            }
        }

    }
}
