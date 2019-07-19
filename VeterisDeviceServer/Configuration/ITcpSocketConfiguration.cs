using Config.Net;

namespace VeterisDeviceServer.Configuration
{
    /// <summary>
    /// Configuración del gestor de canales TCP Socket
    /// </summary>
    public interface ITcpSocketConfiguration
    {
        /// <summary>
        /// Puerto en el cual escuchar
        /// </summary>
        [Option(Alias = "ListenPort", DefaultValue = 666)]
        int ListenPort { get; set; }
    }
}
