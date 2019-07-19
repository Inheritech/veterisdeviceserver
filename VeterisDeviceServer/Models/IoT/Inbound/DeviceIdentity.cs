using Newtonsoft.Json;
using System.Collections.Generic;
using VeterisDeviceServer.Serialization;

namespace VeterisDeviceServer.Models.IoT.Inbound
{
    /// <summary>
    /// Modelo de identidad de dispositivo IoT
    /// </summary>
    [JsonTypeName("identity")]
    public class DeviceIdentity
    {
        /// <summary>
        /// Numero de serie del dispositivo
        /// </summary>
        [JsonProperty("serial", Required = Required.Always)]
        public string SerialNumber { get; set; }

        /// <summary>
        /// Propiedades del dispositivo
        /// </summary>
        [JsonProperty("props", Required = Required.Always)]
        public Dictionary<string, string> Properties { get; set; }

        /// <summary>
        /// Eventos a los que escucha este dispositivo
        /// </summary>
        [JsonProperty("listens", Required = Required.DisallowNull, NullValueHandling = NullValueHandling.Ignore)]
        public List<string> ListensTo { get; set; }

        /// <summary>
        /// Configuraciones disponibles del dispositivo
        /// </summary>
        [JsonProperty("config", Required = Required.DisallowNull, NullValueHandling = NullValueHandling.Ignore)]
        public Dictionary<string, string> Configurations { get; set; }

    }
}
