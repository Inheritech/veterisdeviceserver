using Newtonsoft.Json;

namespace VeterisDeviceServer.Channels.Mesh
{
    /// <summary>
    /// Mensaje base de la red malla
    /// </summary>
    public class MeshMessage
    {
        /// <summary>
        /// Capa de la red malla
        /// </summary>
        [JsonProperty("layer", Required = Required.DisallowNull)]
        public int Layer { get; set; }

        /// <summary>
        /// Dirección Mac de la cual viene o a la cual va el mensaje
        /// </summary>
        [JsonProperty("mac", Required = Required.Always)]
        public string Mac { get; set; }
    }

    /// <summary>
    /// Mensaje base de la red malla
    /// </summary>
    public class MeshMessage<T> : MeshMessage
    {

        /// <summary>
        /// Datos contenidos en el mensaje
        /// </summary>
        [JsonProperty("data", Required = Required.Always)]
        public MeshData Data { get; set; }

        /// <summary>
        /// Constructor vacío
        /// </summary>
        public MeshMessage() {}

        /// <summary>
        /// Crear nuevo mensaje de la red malla
        /// </summary>
        /// <param name="mac">Dirección MAC a la que se dirige</param>
        /// <param name="data">Objeto a contener</param>
        /// <param name="layer">Capa de la red mesh</param>
        public MeshMessage(string mac, T data, int layer = 1)
        {
            Mac = mac;
            Data = new MeshData(data);
            Layer = layer;
        }

        /// <summary>
        /// Clase wrapper de datos para la red malla
        /// </summary>
        public class MeshData
        {
            /// <summary>
            /// Datos a contener
            /// </summary>
            [JsonProperty(Required = Required.Always)]
            public T Value { get; set; }

            /// <summary>
            /// Constructor vacío
            /// </summary>
            public MeshData() {}

            /// <summary>
            /// Construir contenedor de datos
            /// </summary>
            /// <param name="data">Datos a contener</param>
            public MeshData(T data)
            {
                Value = data;
            }
        }
    }
}
