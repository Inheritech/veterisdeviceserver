using System;
using System.Collections.Generic;
using VeterisDeviceServer.Diagnostics;

namespace VeterisDeviceServer.Channels.Socket
{
    /// <summary>
    /// Gestor de canales socket
    /// </summary>
    internal class SocketChannelManager : IChannelManager
    {
        /// <summary>
        /// Al conectarse un canal nuevo
        /// </summary>
        public event EventHandler<IChannel> OnChannelConnected;

        /// <summary>
        /// Al desconectarse un canal
        /// </summary>
        public event EventHandler<IChannel> OnChannelDisconnected;

        /// <summary>
        /// Servidor de sockets
        /// </summary>
        private SocketServer m_socketServer;

        /// <summary>
        /// Canales conectados actualmente
        /// </summary>
        private Dictionary<string, IChannel> m_channels;

        /// <summary>
        /// Puerto por default para el servidor
        /// </summary>
        private const int DEFAULT_PORT = 555;

        /// <summary>
        /// Inicializar el gestor de canales socket
        /// </summary>
        public SocketChannelManager()
        {
            m_socketServer = new SocketServer(DEFAULT_PORT);
            m_socketServer.OnSocketOpen += SocketServer_OnSocketOpen;
            m_socketServer.OnSocketClose += SocketServer_OnSocketClose;
            m_channels = new Dictionary<string, IChannel>();
            Debug.WriteLine(this, "Gestor de canales Socket iniciado. Recibiendo conexiones en el puerto " + DEFAULT_PORT, VerbosityLevel.Info);
        }

        /// <summary>
        /// Al cerrarse una conexión Socket
        /// </summary>
        /// <param name="sender">Emisor</param>
        /// <param name="e">Datos de conexión</param>
        private void SocketServer_OnSocketClose(object sender, SocketConnection e)
        {
            if (!m_channels.ContainsKey(e.ConnectionId)) {
                Debug.WriteLine(this, $"Se recibio un mensaje de desconexión para el Socket {e.ConnectionId} pero el canal no existia", VerbosityLevel.Debug);
                return;
            }

            IChannel channel = m_channels[e.ConnectionId];
            OnChannelDisconnected?.Invoke(this, channel);
            Debug.WriteLine(this, "Se desconecto un canal Socket");
        }

        /// <summary>
        /// Al abrirse una conexión Socket
        /// </summary>
        /// <param name="sender">Emisor</param>
        /// <param name="e">Datos de conexión</param>
        private void SocketServer_OnSocketOpen(object sender, SocketConnection e)
        {
            SocketChannel channel = new SocketChannel(e);
            m_channels.Add(e.ConnectionId, channel);
            OnChannelConnected?.Invoke(this, channel);
        }
    }
}
