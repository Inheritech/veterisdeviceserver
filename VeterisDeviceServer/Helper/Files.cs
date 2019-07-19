using System.IO;

namespace VeterisDeviceServer.Helper
{
    /// <summary>
    /// Clase de apoyo para manejo de archivos
    /// </summary>
    public static class Files
    {
        /// <summary>
        /// Eliminar archivo en caso que exista
        /// </summary>
        /// <param name="filename">Nombre del archivo</param>
        public static void DeleteIfExists(string filename)
        {
            if (File.Exists(filename)) {
                File.Delete(filename);
            }
        }
    }
}
