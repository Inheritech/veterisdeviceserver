using Config.Net;

namespace VeterisDeviceServer.Configuration
{
    /// <summary>
    /// Configuración del adaptador Socket
    /// </summary>
    public interface ISocketAdapterConfiguration
    {
        /// <summary>
        /// Hacer override del SSL y deshabilitarlo
        /// </summary>
        [Option(Alias = "OverrideDisableSSL", DefaultValue = true)]
        bool OverrideSecure { get; set; }
    }
}
