using Entidades.Exceptions;
using Entidades.Interfaces;
using Entidades.Modelos;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Entidades.Files
{

    public static class FileManager
    {
        private static string path;

        static FileManager()
        {
            FileManager.ValidaExixtenciaDeDirectorio();
        }

        private static void ValidaExixtenciaDeDirectorio()
        {
            path = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
            string rutaCompleta = Path.Combine(path, "SP_07122023_BATHORY_GERMAN_2C");
            try
            {
                if (!Directory.Exists(rutaCompleta))
                {
                    Directory.CreateDirectory(rutaCompleta);
                }
            }
            catch (Exception ex)
            {
                throw new FileManagerException("Error al crear el directorio", ex);
                FileManager.Guardar(ex.Message, "logs.txt", true);
            }
        }

        public static void Guardar(string data, string nombreArchivo, bool append)
        {
            string rutaCompleta = Path.Combine(path, nombreArchivo);
            try
            {
                using (StreamWriter sw = new StreamWriter(rutaCompleta, append))
                {
                    sw.WriteLine(data);
                }
            }
            catch (Exception ex)
            {
                throw new FileManagerException("Error al guardar archivo", ex);
                FileManager.Guardar(ex.Message, "logs.txt", true);
            }
        }

        public static bool Serializar<T>(T elemento, string nombreArchivo) where T : class
        {
            JsonSerializerOptions options = new JsonSerializerOptions();
            options.WriteIndented = true;
            string objetoJson = JsonSerializer.Serialize(elemento, options);

            FileManager.Guardar(objetoJson, nombreArchivo, false);
            return true;
        }

    }
}

