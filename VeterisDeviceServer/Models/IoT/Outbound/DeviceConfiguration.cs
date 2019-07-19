using Newtonsoft.Json;
using System.Collections.Generic;
using VeterisDeviceServer.Serialization;

namespace VeterisDeviceServer.Models.IoT.Outbound
{
    /// <summary>
    /// Estructura de configuración
    /// </summary>
    [JsonTypeName("config")]
    public class DeviceConfiguration
    {
        /// <summary>
        /// Numero de serie del dispositivo
        /// </summary>
        [JsonProperty("serial", Required = Required.Always)]
        public string SerialNumber { get; set; }

        /// <summary>
        /// Configuración del dispositivo
        /// </summary>
        [JsonProperty("config", Required = Required.Always)]
        public Dictionary<string, object> Configuration { get; set; }
    }
}
