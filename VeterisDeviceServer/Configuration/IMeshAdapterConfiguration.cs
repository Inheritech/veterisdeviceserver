using Config.Net;

namespace VeterisDeviceServer.Configuration
{
    /// <summary>
    /// Configuración del adaptador de red malla
    /// </summary>
    public interface IMeshAdapterConfiguration
    {
        /// <summary>
        /// Puerto en el que se escuchara por conexiones mesh
        /// </summary>
        [Option(Alias = "ListenPort", DefaultValue = 500)]
        int ListenPort { get; set; }

        /// <summary>
        /// Eliminar clientes conectados a la mesh al conectarse otro nodo raíz
        /// </summary>
        [Option(Alias = "KillOnConnect", DefaultValue = false)]
        bool KillOnConnect { get; set; }
    }
}
