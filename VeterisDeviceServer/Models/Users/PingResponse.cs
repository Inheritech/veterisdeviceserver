using Newtonsoft.Json;

namespace VeterisDeviceServer.Models.Users
{
    /// <summary>
    /// Respuesta de Ping
    /// </summary>
    public class PingResponse
    {
        /// <summary>
        /// Constante de nombre de servidor
        /// </summary>
        public const string SERVER_NAME = "VeterisDeviceServer";

        /// <summary>
        /// Identificador del servidor
        /// </summary>
        [JsonProperty("id", NullValueHandling = NullValueHandling.Include)]
        public string Id { get; set; }

        /// <summary>
        /// Nombre del servidor
        /// </summary>
        [JsonProperty("server_name")]
        public string ServerName { get; set; }

        /// <summary>
        /// Inicializar respuesta de ping
        /// </summary>
        /// <param name="id">Identificador de servidor en caso que exista</param>
        /// <param name="serverName">Nombre del tipo de servidor</param>
        public PingResponse(string id, string serverName = SERVER_NAME)
        {
            Id = id;
            ServerName = serverName;
        }
    }
}
