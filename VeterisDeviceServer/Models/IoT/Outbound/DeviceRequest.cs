using Newtonsoft.Json;
using VeterisDeviceServer.Serialization;

namespace VeterisDeviceServer.Models.IoT.Outbound
{
    /// <summary>
    /// Mensaje de solicitud a dispositivo
    /// </summary>
    [JsonTypeName("request")]
    public class DeviceRequest
    {
        /// <summary>
        /// Mensaje de solicitud de identidad
        /// </summary>
        public const string IDENTITY_REQUEST = "identity";

        /// <summary>
        /// Mensaje de solicitud de estado
        /// </summary>
        public const string STATUS_REQUEST = "status";

        /// <summary>
        /// Base para solicitud de estado
        /// </summary>
        public static DeviceRequest Status
        {
            get
            {
                return new DeviceRequest
                {
                    SerialNumber = "",
                    Request = STATUS_REQUEST
                };
            }
        }

        /// <summary>
        /// Solicitud de identidad
        /// </summary>
        public static DeviceRequest Identity
        {
            get
            {
                return new DeviceRequest
                {
                    SerialNumber = "",
                    Request = IDENTITY_REQUEST
                };
            }
        }

        /// <summary>
        /// Numero de serie del dispositivo al cual realizar la solicitud
        /// </summary>
        [JsonProperty("serial", Required = Required.Always)]
        public string SerialNumber { get; set; }

        /// <summary>
        /// Solicitud a realizar
        /// </summary>
        [JsonProperty("request", Required = Required.Always)]
        public string Request { get; set; }
    }
}
