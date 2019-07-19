using Config.Net;

namespace VeterisDeviceServer.Configuration
{
    /// <summary>
    /// Configuración de SSL
    /// </summary>
    public interface ISSLConfiguration
    {
        /// <summary>
        /// Determina si se debe usar el modo seguro de la API
        /// </summary>
        [Option(Alias = "Secure", DefaultValue = true)]
        bool Secure { get; set; }

        /// <summary>
        /// Determina si se debe generar automaticamente un certificado SSL para el servidor
        /// </summary>
        [Option(Alias = "AutoGenerate", DefaultValue = true)]
        bool AutoGenerate { get; set; }

        /// <summary>
        /// Ruta al certificado a utilizar en el servidor
        /// </summary>
        [Option(Alias = "CertificatePath", DefaultValue = "")]
        string CertificatePath { get; set; }

        /// <summary>
        /// Contraseña del certificado del servidor
        /// </summary>
        [Option(Alias = "CertificatePassword", DefaultValue = "")]
        string CertificatePassword { get; set; }
    }
}
