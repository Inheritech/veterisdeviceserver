using Newtonsoft.Json;
using System.Collections.Generic;

namespace VeterisDeviceServer.Serialization
{
    /// <summary>
    /// Conjunto de dispositivos serializables
    /// </summary>
    public class SerializableDevices
    {
        /// <summary>
        /// Dispositivos
        /// </summary>
        [JsonProperty("devices")]
        public List<SerializableDevice> Devices { get; set; }

        public SerializableDevices()
        {
            Devices = new List<SerializableDevice>();
        }
    }
}
