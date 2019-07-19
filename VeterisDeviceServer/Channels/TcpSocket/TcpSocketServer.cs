using System;
using System.Text;
using WatsonTcp;

namespace VeterisDeviceServer.Channels.TcpSocket
{
    /// <summary>
    /// Servidor de comunicaciones TCP Socket
    /// </summary>
    internal class TcpSocketServer
    {
        /// <summary>
        /// Al conectarse un cliente
        /// </summary>
        public event EventHandler<string> OnTcpConnected;

        /// <summary>
        /// Al desconectarse un cliente
        /// </summary>
        public event EventHandler<string> OnTcpDisconnected;

        /// <summary>
        /// Al recibirse un mensaje desde un cliente TCP
        /// </summary>
        public event EventHandler<TcpMessage> OnTcpMessage;

        /// <summary>
        /// Servidor de conexiones TCP
        /// </summary>
        private WatsonTcpServer m_server;

        /// <summary>
        /// Inicializar servidor
        /// </summary>
        /// <param name="port">Puerto en el cual escuchar</param>
        public TcpSocketServer(int port)
        {
            m_server = new WatsonTcpServer(
                "0.0.0.0",
                port,
                Tcp_Connected,
                Tcp_Disconnected,
                Tcp_Message,
                true
            );
        }

        /// <summary>
        /// Al recibirse un mensaje por TCP
        /// </summary>
        /// <param name="ipPort">IP / Puerto fuente</param>
        /// <param name="data">Información</param>
        private bool Tcp_Message(string ipPort, byte[] data)
        {
            string msg = Encoding.ASCII.GetString(data);
            OnTcpMessage?.Invoke(this, new TcpMessage(ipPort, msg));
            return true;
        }

        /// <summary>
        /// Al conectarse un cliente
        /// </summary>
        /// <param name="ipPort">IP / Puerto</param>
        private bool Tcp_Connected(string ipPort)
        {
            OnTcpConnected?.Invoke(this, ipPort);
            return true;
        }

        /// <summary>
        /// Al desconectarse un cliente
        /// </summary>
        /// <param name="ipPort">IP / Puerto</param>
        private bool Tcp_Disconnected(string ipPort)
        {
            OnTcpDisconnected.Invoke(this, ipPort);
            return true;
        }

        /// <summary>
        /// Escribir al servidor
        /// </summary>
        /// <param name="ipPort">IP / Puerto destinatario</param>
        /// <param name="msg">Mensaje a enviar</param>
        public void Write(string ipPort, string msg)
        {
            byte[] data = Encoding.ASCII.GetBytes(msg);
            m_server.Send(ipPort, data);
        }

    }

    /// <summary>
    /// Estructura de mensaje TCP
    /// </summary>
    internal struct TcpMessage
    {
        /// <summary>
        /// IP / Puerto de donde se origino el mensaje
        /// </summary>
        public string Source { get; }

        /// <summary>
        /// Mensaje recibido
        /// </summary>
        public string Message { get; }

        /// <summary>
        /// Crear estructura de mensaje TCP
        /// </summary>
        /// <param name="source">Fuente</param>
        /// <param name="msg">Mensaje</param>
        public TcpMessage(string source, string msg)
        {
            Source = source;
            Message = msg;
        }

    }
}
