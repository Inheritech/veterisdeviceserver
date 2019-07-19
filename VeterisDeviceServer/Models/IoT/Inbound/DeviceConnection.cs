using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System.Runtime.Serialization;
using VeterisDeviceServer.Serialization;

namespace VeterisDeviceServer.Models.IoT.Inbound
{
    /// <summary>
    /// Enumeración de estados de conexión para gestores de canales
    /// </summary>
    [JsonConverter(typeof(StringEnumConverter))]
    [JsonTypeName("connection_event")]
    public enum ConnectionEvent
    {
        [EnumMember(Value = "connected")]
        Connected,
        [EnumMember(Value = "disconnected")]
        Disconnected
    }

    /// <summary>
    /// Modelo para evento de conexión de un dispositivo
    /// </summary>
    public sealed class DeviceConnection
    {
        /// <summary>
        /// Evento de conexión que ha ocurrido
        /// </summary>
        [JsonProperty("connection_event", Required = Required.Always)]
        public ConnectionEvent ConnectionEvent { get; set; }
    }
}
