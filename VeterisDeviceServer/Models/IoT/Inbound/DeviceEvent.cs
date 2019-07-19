using Newtonsoft.Json;
using System.Collections.Generic;
using VeterisDeviceServer.Serialization;

namespace VeterisDeviceServer.Models.IoT.Inbound
{
    /// <summary>
    /// Evento de dispositivo
    /// </summary>
    [JsonTypeName("event")]
    public class DeviceEvent
    {
        /// <summary>
        /// Numero de serie del dispositivo
        /// </summary>
        [JsonProperty("serial", Required = Required.Always)]
        public string SerialNumber { get; set; }

        /// <summary>
        /// Nombre del evento
        /// </summary>
        [JsonProperty("name", Required = Required.Always)]
        public string Name { get; set; }

        /// <summary>
        /// Argumentos del evento
        /// </summary>
        [JsonProperty("arguments", Required = Required.Always)]
        public Dictionary<string, object> Arguments { get; set; }

        /// <summary>
        /// Opcional: Serial del dispositivo destino del evento
        /// </summary>
        [JsonProperty("target", Required = Required.DisallowNull, NullValueHandling = NullValueHandling.Ignore)]
        public string Target { get; set; }
    }
}
