using Newtonsoft.Json;
using System.Collections.Generic;
using VeterisDeviceServer.Serialization;

namespace VeterisDeviceServer.Models.IoT.Outbound
{
    /// <summary>
    /// Mensaje de actualización de dispositivo
    /// </summary>
    [JsonTypeName("update")]
    public class DeviceUpdate
    {
        /// <summary>
        /// Numero de serie del dispositivo a actualizar
        /// </summary>
        [JsonProperty("serial", Required = Required.Always)]
        public string SerialNumber { get; set; }

        /// <summary>
        /// Propiedades a actualizar
        /// </summary>
        [JsonProperty("update", Required = Required.Always)]
        public Dictionary<string, object> PropertiesUpdate { get; set;}

    }
}
