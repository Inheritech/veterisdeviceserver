using Newtonsoft.Json;
using System.Collections.Generic;
using VeterisDeviceServer.Serialization;

namespace VeterisDeviceServer.Models.IoT.Inbound
{
    /// <summary>
    /// Solicitud de estado para dispositivo
    /// </summary>
    [JsonTypeName("status")]
    public class DeviceStatus
    {
        /// <summary>
        /// Numero de serie del dispositivo
        /// </summary>
        [JsonProperty("serial", Required = Required.Always)]
        public string SerialNumber { get; set; }

        /// <summary>
        /// Estado actual
        /// </summary>
        [JsonProperty("status", Required = Required.Always)]
        public Dictionary<string, object> Status { get; set; }
    }
}
