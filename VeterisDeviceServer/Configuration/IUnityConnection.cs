using Config.Net;

namespace VeterisDeviceServer.Configuration
{
    /// <summary>
    /// Configuración de simulación de Unity
    /// </summary>
    public interface IUnityConnection
    {
        /// <summary>
        /// Dirección de la simulación a la cual conectar
        /// </summary>
        [Option(Alias = "ListenPort", DefaultValue = 409)]
        int Port { get; set; }
    }
}
