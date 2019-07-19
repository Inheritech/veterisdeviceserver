using Newtonsoft.Json;
using System.Collections.Generic;
using VeterisDeviceServer.Devices;
using VeterisDeviceServer.Models.Accessibility;
using VeterisDeviceServer.Models.IoT.Inbound;

namespace VeterisDeviceServer.Serialization
{
    /// <summary>
    /// Dispositivo serializable
    /// </summary>
    public class SerializableDevice
    {
        /// <summary>
        /// Identidad del dispositivo
        /// </summary>
        [JsonProperty("identity")]
        public DeviceIdentity Identity { get; set; }

        /// <summary>
        /// Estado del dispositivo
        /// </summary>
        [JsonProperty("status")]
        public DeviceStatus Status { get; set; }

        /// <summary>
        /// Valores de configuración actuales del dispositivo
        /// </summary>
        [JsonProperty("config", Required = Required.DisallowNull, NullValueHandling = NullValueHandling.Ignore)]
        public Dictionary<string, object> Configurations { get; set; }

        /// <summary>
        /// Valores de traducción actuales del dispositivo
        /// </summary>
        [JsonProperty("translations", Required = Required.DisallowNull, NullValueHandling = NullValueHandling.Ignore)]
        public DeviceTranslation Translations { get; set; }

        /// <summary>
        /// Construir dispositivo serializable en base a dispositivo
        /// </summary>
        /// <param name="device">Dispositivo</param>
        public SerializableDevice(Device device)
        {
            Identity = device.Identity;
            Status = device.Status;
        }

    }
}
