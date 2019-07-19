using Newtonsoft.Json;

namespace VeterisDeviceServer.Models.Users
{
    /// <summary>
    /// Solicitud de datos por parte de un usuario
    /// </summary>
    public class UserDataRequest
    {
        /// <summary>
        /// Valor de la solicitud de datos
        /// </summary>
        [JsonProperty("data_request", Required = Required.Always)]
        public string DataRequest { get; set; }
    }
}
