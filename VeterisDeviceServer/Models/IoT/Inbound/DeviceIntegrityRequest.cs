using Newtonsoft.Json;
using System.Runtime.Serialization;
using VeterisDeviceServer.Serialization;

namespace VeterisDeviceServer.Models.IoT.Inbound
{
    /// <summary>
    /// Enumeración de eventos de integridad de un dispositivo
    /// </summary>
    [JsonTypeName("integrity")]
    public enum IntegrityEvent
    {
        /// <summary>
        /// Solicitud de revisión de integridad
        /// </summary>
        [EnumMember(Value = "check")]
        Check
    }

    /// <summary>
    /// Solicitud de integridad de un dispositivo
    /// </summary>
    public sealed class DeviceIntegrityRequest
    {
        /// <summary>
        /// Evento de integridad ocurrido
        /// </summary>
        [JsonProperty("integrity", Required = Required.Always)]
        public IntegrityEvent IntegrityEvent { get; set; }
    }
}
