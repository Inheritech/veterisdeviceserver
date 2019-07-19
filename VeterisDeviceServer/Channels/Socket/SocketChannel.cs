using Newtonsoft.Json;
using System;
using VeterisDeviceServer.Diagnostics;
using WebSocketSharp;

namespace VeterisDeviceServer.Channels.Socket
{
    /// <summary>
    /// Canal para usuarios conectados por Socket local
    /// </summary>
    internal class SocketChannel : IChannel
    {
        /// <summary>
        /// Al recibir un mensaje en este canal
        /// </summary>
        public event EventHandler<string> OnMessage;

        /// <summary>
        /// Identificador de socket
        /// </summary>
        public string SocketID { get; }

        /// <summary>
        /// Conexión WebSocket del canal
        /// </summary>
        private WebSocket m_webSocket;

        /// <summary>
        /// Inicializar canal socket
        /// </summary>
        /// <param name="conn">Conexión socket a utilizar</param>
        public SocketChannel(SocketConnection conn)
        {
            SocketID = conn.ConnectionId;
            m_webSocket = conn.WebSocket;
            m_webSocket.OnMessage += WebSocket_OnMessage;
            Debug.WriteLine(this, "Canal Socket creado", VerbosityLevel.Debug);
        }

        /// <summary>
        /// Al recibir un mensaje WebSocket
        /// </summary>
        /// <param name="sender">Emisor</param>
        /// <param name="e">Argumentos</param>
        private void WebSocket_OnMessage(object sender, MessageEventArgs e)
        {
            if (OnMessage == null)
                return;

            if (!e.IsText)
                return;

            Debug.WriteLine(this, "Recibido mensaje por Socket: " + e.Data, VerbosityLevel.Debug);
            OnMessage.Invoke(this, e.Data);
        }

        /// <summary>
        /// Escribir al canal
        /// </summary>
        /// <typeparam name="T">TIpo de objeto</typeparam>
        /// <param name="obj">Objeto</param>
        public void Write<T>(T obj)
        {
            if (m_webSocket == null)
                return;

            if (m_webSocket.ReadyState != WebSocketState.Open) {
                Debug.WriteLine(this, $"No se pudo escribir al canal Socket '{SocketID}' porque su estado es: {m_webSocket.ReadyState.ToString()}");
                return;
            }

            string serialized = JsonConvert.SerializeObject(obj);
            m_webSocket.Send(serialized);
            Debug.WriteLine(this, $"Escrito mensaje por Socket: {serialized}", VerbosityLevel.Debug);
        }

        /// <summary>
        /// Destructor
        /// </summary>
        ~SocketChannel()
        {
            Dispose();
        }

        /// <summary>
        /// Disponer de los recursos usados
        /// </summary>
        public void Dispose()
        {
            m_webSocket = null;
        }
    }
}
