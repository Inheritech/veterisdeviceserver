using System;
using System.Collections.Generic;
using System.Text;
using VeterisDeviceServer.Configuration;
using VeterisDeviceServer.Diagnostics;
using WatsonTcp;

namespace VeterisDeviceServer.Channels.TcpSocket
{
    /// <summary>
    /// Gestor de canales TCP Socket
    /// </summary>
    internal class TcpSocketChannelManager : IChannelManager
    {
        /// <summary>
        /// Al conectarse un cliente TCP Socket
        /// </summary>
        public event EventHandler<IChannel> OnChannelConnected;

        /// <summary>
        /// Al desconectarse un cliente TCP Socket
        /// </summary>
        public event EventHandler<IChannel> OnChannelDisconnected;

        /// <summary>
        /// Canales conectados actualmente
        /// </summary>
        private Dictionary<string, IChannel> m_channels;

        /// <summary>
        /// Servidor de comunicaciones TCP Socket
        /// </summary>
        private TcpSocketServer m_server;

        /// <summary>
        /// Inicializar gestor de canales TCP Socket
        /// </summary>
        public TcpSocketChannelManager()
        {
            m_channels = new Dictionary<string, IChannel>();
            var config = ConfigurationManager.Get<ITcpSocketConfiguration>();
            m_server = new TcpSocketServer(config.ListenPort);
            m_server.OnTcpConnected += TcpServer_OnTcpConnected;
            m_server.OnTcpDisconnected += TcpServer_OnTcpDisconnected;
            Debug.WriteLine(this, "Gestor de canales TCP Socket inicializado, escuchando en el puerto: " + config.ListenPort, VerbosityLevel.Info);
        }

        /// <summary>
        /// Al desconectarse un cliente TCP
        /// </summary>
        /// <param name="sender">Emisor</param>
        /// <param name="e">IP / Puerto</param>
        private void TcpServer_OnTcpDisconnected(object sender, string e)
        {
            RemoveChannel(e);
        }

        /// <summary>
        /// Al conectarse un cliente TCP
        /// </summary>
        /// <param name="sender">Emisor</param>
        /// <param name="e">IP / Puerto</param>
        private void TcpServer_OnTcpConnected(object sender, string e)
        {
            AddChannel(e);
        }

        /// <summary>
        /// Añadir canal
        /// </summary>
        /// <param name="ipPort">IP / Puerto</param>
        private void AddChannel(string ipPort)
        {
            if (m_channels.ContainsKey(ipPort)) {
                Debug.WriteLine(this, "Se está tratando de conectar un nuevo canal desde una conexión ya registrada, suprimiendo...", VerbosityLevel.Warning);
                return;
            }

            IChannel channel = new TcpSocketChannel(m_server, ipPort);
            m_channels.Add(ipPort, channel);
            OnChannelConnected?.Invoke(this, channel);
            Debug.WriteLine(this, "Se conecto un nuevo canal TCP Socket", VerbosityLevel.Debug);
        }

        /// <summary>
        /// Eliminar canal
        /// </summary>
        /// <param name="ipPort">IP / Puerto</param>
        private void RemoveChannel(string ipPort)
        {
            if (!m_channels.ContainsKey(ipPort)) {
                Debug.WriteLine(this, "No se pudo eliminar un canal TCP Socket ya que el IP / Puerto no están registrado", VerbosityLevel.Warning);
                return;
            }

            IChannel channel = m_channels[ipPort];
            m_channels.Remove(ipPort);
            OnChannelDisconnected?.Invoke(this, channel);
            Debug.WriteLine(this, "Se elimino un canal TCP Socket correctamente", VerbosityLevel.Debug);
        }
    }
}
