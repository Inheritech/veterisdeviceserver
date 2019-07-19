using Config.Net;

namespace VeterisDeviceServer.Configuration
{
    /// <summary>
    /// Configuración de conexión a base de datos
    /// </summary>
    public interface IDatabaseConfiguration
    {
        /// <summary>
        /// Datos de conexión a la base de datos
        /// </summary>
        [Option(Alias = "DBFilePath", DefaultValue = "VeterisDeviceServer.db")]
        string DBFilePath { get; set; }
    }
}
