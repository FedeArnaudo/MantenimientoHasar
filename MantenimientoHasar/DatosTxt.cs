using System;
using System.Collections.Generic;
using System.IO;
using System.Windows;

namespace MantenimientoHasar
{
    internal class DatosTxt
    {
        private readonly string baseDirectory;
        private readonly string directoryPath;
        public static readonly string fileProy = "Ruta.txt";
        public static readonly string fileDelete = "Archivos_A_Borrar.txt";
        private readonly string filePathProy;
        private readonly string filePathDelete;
        private static readonly DatosTxt instance = new DatosTxt();
        private DatosTxt()
        {
            // Obtener el directorio de ejecución del programa
            baseDirectory = AppDomain.CurrentDomain.BaseDirectory;

            // Combinar el directorio base con el nombre del directorio especificado
            directoryPath = Path.Combine(baseDirectory, "Ruta_Y_Archivos_A_Borrar");

            // Combinar el directorio de destino con el nombre del archivo especificado
            filePathProy = Path.Combine(directoryPath, fileProy);

            // Combinar el directorio de destino con el nombre del archivo especificado
            filePathDelete = Path.Combine(directoryPath, fileDelete);
        }
        public static DatosTxt InstanciaDatosTxt()
        {
            return instance;
        }
        public void VerificarCarpeta()
        {
            // Verificar si la carpeta existe, y si no, crearla
            if (!Directory.Exists(directoryPath))
            {
                _ = Directory.CreateDirectory(directoryPath);
            }
            VerificarArchivoRuta();
            VerificarArchivoDeEliminacion();
        }
        public void VerificarArchivoRuta()
        {
            // Verificar si el archivo existe, y si no, crearlo
            if (!File.Exists(filePathProy))
            {
                // Crear el archivo vacío
                using (File.Create(filePathProy)) { }
            }
        }
        public void VerificarArchivoDeEliminacion()
        {
            // Verificar si el archivo existe, y si no, crearlo
            if (!File.Exists(filePathDelete))
            {
                // Crear el archivo vacío
                using (File.Create(filePathDelete)) { }

                File.WriteAllText(filePathDelete, "Agregue los archivos a Eliminar:");
            }
        }
        public void Escribir(string archivo, string line)
        {
            bool contiene = false;
            string filePath = "";

            switch (archivo)
            {
                case "Ruta.txt":
                    filePath = filePathProy;
                    break;
                case "Archivos_A_Borrar.txt":
                    filePath = filePathDelete;
                    break;
                default:
                    break;
            }

            // Leer todo el contenido del archivo si existe
            string[] existingLines = File.Exists(filePath) ? File.ReadAllLines(filePath) : new string[0];

            // Crear una lista para almacenar las nuevas líneas
            List<string> lines = new List<string>();

            foreach (string s in existingLines)
            {
                if (line.Equals(s))
                {
                    contiene = true;
                }
            }

            if (contiene == false)
            {
                // Añadir las líneas existentes al final de la nueva lista
                lines.AddRange(existingLines);
                lines.Add(line);

                // Escribir todas las líneas de vuelta en el archivo
                File.WriteAllLines(filePath, lines);
            }
        }
        public List<string> Listar(string archivo)
        {
            List<string> archivos = new List<string>();
            string filePath = "";

            switch (archivo)
            {
                case "Ruta.txt":
                    filePath = filePathProy;
                    break;
                case "Archivos_A_Borrar.txt":
                    filePath = filePathDelete;
                    break;
                default:
                    break;
            }
            // Leer todo el contenido del archivo si existe
            string[] existingLines = File.Exists(filePath) ? File.ReadAllLines(filePath) : new string[0];

            archivos.AddRange(existingLines);

            return archivos;
        }
        public void EliminarArchivos(string ruta)
        {
            List<string> archivos = Listar(fileDelete);
            try
            {
                // Verifica si el directorio existe
                if (Directory.Exists(ruta))
                {
                    // Obtén la lista de carpetas en el directorio
                    List<string> carpetas = ObtenerCarpetas(ruta);

                    for (int i = 0; i < archivos.Count; i++)
                    {
                        if (i > 0)
                        {
                            string archivo = archivos[i];
                            // Borra el Dll
                            foreach (string carpeta in carpetas)
                            {
                                Borrar(carpeta, archivo);
                            }
                        }
                    }
                }
                else
                {
                    _ = MessageBox.Show($"El directorio {ruta} no existe.");
                }
            }
            catch (Exception ex)
            {
                _ = MessageBox.Show($"Error al obtener las carpetas: {ex.Message}");
            }
        }
        private List<string> ObtenerCarpetas(string rutaDirectorio)
        {
            // Utiliza la clase Directory para obtener la lista de carpetas
            string[] carpetasArray = Directory.GetDirectories(rutaDirectorio);

            // Convierte el array en una lista
            List<string> carpetas = new List<string>(carpetasArray);

            return carpetas;
        }
        private void Borrar(string carpeta, string nombreArchivo)
        {
            // Obtener todos los archivos en el directorio
            string[] archivos = Directory.GetFiles(carpeta);

            foreach (string archivo in archivos)
            {
                // Verifica si el archivo existe
                if (Path.GetFileName(archivo).StartsWith(nombreArchivo.Substring(0, 8).ToLower()) || Path.GetFileName(archivo).Equals(nombreArchivo))
                {
                    // Borra el archivo
                    File.Delete(archivo);
                }
            }
        }
    }
}
