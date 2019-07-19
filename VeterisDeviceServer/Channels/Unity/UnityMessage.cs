using Newtonsoft.Json;

namespace VeterisDeviceServer.Channels.Unity
{
    /// <summary>
    /// Contenedor para mensajes a la simulación de Unity
    /// </summary>
    public sealed class UnityMessage<T> : UnityMessage
    {
        /// <summary>
        /// Información para el dispositivo
        /// </summary>
        [JsonProperty("data", Required = Required.Always)]
        public T Data { get; set; }

        /// <summary>
        /// Inicializar mensaje vacío
        /// </summary>
        public UnityMessage() { }

        /// <summary>
        /// Inicializar con datos
        /// </summary>
        /// <param name="obj">Objeto de datos</param>
        public UnityMessage(string unityId, T obj)
        {
            UnityId = unityId;
            Data = obj;
        }
    }

    /// <summary>
    /// Información base para mensajes a la simulación de Unity
    /// </summary>
    public class UnityMessage
    {
        /// <summary>
        /// Identificador utilizado por el adaptador de Unity para referirse al dispositivo destino
        /// </summary>
        [JsonProperty("unity_id", Required = Required.Always)]
        public string UnityId { get; set; }
    }
}
