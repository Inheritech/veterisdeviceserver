using Config.Net;

namespace VeterisDeviceServer.Configuration
{
    /// <summary>
    /// Configuración para el servidor Web de API
    /// </summary>
    public interface IWebServerConfiguration
    {
        /// <summary>
        /// Puerto para HTTP
        /// </summary>
        [Option(Alias = "HttpPort", DefaultValue = 218)]
        int HttpPort { get; set; }

        /// <summary>
        /// Puerto para HTTPS
        /// </summary>
        [Option(Alias = "HttpsPort", DefaultValue = 219)]
        int HttpsPort { get; set; }

        /// <summary>
        /// Habilitar modulo de archivos estaticos
        /// </summary>
        [Option(Alias = "EnableFilesModule", DefaultValue = false)]
        bool EnableFilesModule { get; set; }

        /// <summary>
        /// Ruta al directorio para archivos estaticos
        /// </summary>
        [Option(Alias = "FilesModulePath", DefaultValue = "./WWWFiles")]
        string FilesModulePath { get; set; }
    }
}
