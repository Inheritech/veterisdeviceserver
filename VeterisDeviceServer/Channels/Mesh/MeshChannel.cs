using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using VeterisDeviceServer.Diagnostics;
using VeterisDeviceServer.Helper;

namespace VeterisDeviceServer.Channels.Mesh
{
    /// <summary>
    /// Canal de comunicación de red malla
    /// </summary>
    internal class MeshChannel : IChannel
    {
        /// <summary>
        /// Al recibir un mensaje
        /// </summary>
        public event EventHandler<string> OnMessage;

        /// <summary>
        /// Capa en la que se encuentra el dispositivo
        /// </summary>
        public int Layer { get; }

        /// <summary>
        /// Dirección MAC del canal
        /// </summary>
        public string MacAddress { get; }

        /// <summary>
        /// Servidor TCP al cual escribir
        /// </summary>
        private readonly MeshTcpServer m_tcpServer;

        /// <summary>
        /// Crear nuevo canal
        /// </summary>
        /// <param name="mac">Dirección MAC</param>
        /// <param name="tcpServer">Servidor al cual escribir</param>
        /// <param name="layer">Capa del dispositivo</param>
        public MeshChannel(string mac, MeshTcpServer tcpServer, int layer = 1)
        {
            MacAddress = mac;
            m_tcpServer = tcpServer;
            m_tcpServer.OnMessage += TcpServer_OnMessage;
            Layer = layer;
        }

        /// <summary>
        /// Al recibir un mensaje desde el servidor TCP
        /// </summary>
        /// <param name="sender">Emisor</param>
        /// <param name="e">Mensaje</param>
        private void TcpServer_OnMessage(object sender, string e)
        {
            MeshMessage meshMessage = Json.Parse<MeshMessage>(e);
            if (meshMessage != null) {
                if (meshMessage.Mac == this.MacAddress) {
                    JObject jObj = JObject.Parse(e);
                    if (jObj.ContainsKey("data")) {
                        JToken data = jObj["data"];
                        if (data.HasValues && !(data.First.Path == "connection_event")) {
                            JToken firstDataProp = data.First;

                            // Bajar al siguiente nivel de propiedades en caso que
                            // existan
                            if (firstDataProp.HasValues)
                                firstDataProp = firstDataProp.First;

                            string msg = firstDataProp.ToString();

                            Debug.WriteLine(this, "Procesando mensaje: " + msg, VerbosityLevel.Debug);

                            OnMessage?.Invoke(this, msg);
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Routear mensaje por este canal y emitirlo
        /// </summary>
        /// <param name="e">Mensaje</param>
        [Obsolete("Bad Architecture")]
        public void RouteMessage(string e)
        {
            OnMessage?.Invoke(this, e);
        }

        /// <summary>
        /// Router objeto por este canal y emitirlo
        /// </summary>
        /// <typeparam name="T">Tipo del objeto</typeparam>
        /// <param name="obj">Objeto</param>
        [Obsolete("Bad Architecture")]
        public void RouteMessage<T>(T obj)
        {
            RouteMessage(JsonConvert.SerializeObject(obj));
        }

        /// <summary>
        /// Escribir al canal
        /// </summary>
        /// <param name="obj">Objeto a escribir</param>
        public void Write<T>(T obj)
        {
            if (m_tcpServer == null) {
                Debug.WriteLine(this, "Servidor TCP no establecido, no se pudo escribir el mensaje", VerbosityLevel.Error);
                return;
            }

            m_tcpServer.Write(MacAddress, obj, Layer);
        }

        /// <summary>
        /// Destructor
        /// </summary>
        ~MeshChannel()
        {
            Dispose();
        }

        /// <summary>
        /// Disponer de los recursos
        /// </summary>
        public void Dispose()
        {
            m_tcpServer.OnMessage -= TcpServer_OnMessage;
            Debug.WriteLine(this, "Canal destruido", VerbosityLevel.Debug);
        }
    }
}
