using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace VeterisDeviceServer.Channels.TcpSocket
{
    internal class TcpSocketChannel : IChannel
    {
        public event EventHandler<string> OnMessage;

        /// <summary>
        /// Servidor por el cual dirigir las comunicaciones
        /// </summary>
        private TcpSocketServer m_server;

        /// <summary>
        /// IP / Puerto del canal
        /// </summary>
        private string m_ipPort;

        /// <summary>
        /// Crear canal de comunicación TCP Socket
        /// </summary>
        /// <param name="server">Servidor a utilizar</param>
        /// <param name="ipPort">IP / Puerto</param>
        public TcpSocketChannel(TcpSocketServer server, string ipPort)
        {
            m_server = server;
            m_ipPort = ipPort;
            m_server.OnTcpMessage += TcpServer_OnTcpMessage;
        }

        /// <summary>
        /// Al recibir un mensaje desde el servidor TCP Socket
        /// </summary>
        /// <param name="sender">Emisor</param>
        /// <param name="e">Información del mensaje</param>
        private void TcpServer_OnTcpMessage(object sender, TcpMessage e)
        {
            if (e.Source == m_ipPort) {
                OnMessage?.Invoke(this, e.Message);
            }
        }

        /// <summary>
        /// Escribir al canal
        /// </summary>
        /// <typeparam name="T">Tipo de objeto</typeparam>
        /// <param name="obj">Objeto</param>
        public void Write<T>(T obj)
        {
            if (m_server == null) return;

            string serialized = JsonConvert.SerializeObject(obj);

            m_server.Write(m_ipPort, serialized);
        }

        /// <summary>
        /// Disponer de los recursos del canal
        /// </summary>
        public void Dispose()
        {
            m_server.OnTcpMessage -= TcpServer_OnTcpMessage;
        }
    }
}
