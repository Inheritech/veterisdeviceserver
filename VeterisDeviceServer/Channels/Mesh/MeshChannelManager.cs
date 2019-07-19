using System;
using System.Collections.Generic;
using VeterisDeviceServer.Configuration;
using VeterisDeviceServer.Diagnostics;
using VeterisDeviceServer.Helper;
using VeterisDeviceServer.Models.IoT.Inbound;

namespace VeterisDeviceServer.Channels.Mesh
{
    /// <summary>
    /// Gestor de canales mesh
    /// </summary>
    internal class MeshChannelManager : IChannelManager
    {
        /// <summary>
        /// Al conectarse un canal mesh
        /// </summary>
        public event EventHandler<IChannel> OnChannelConnected;

        /// <summary>
        /// Al desconectarse un canal mesh
        /// </summary>
        public event EventHandler<IChannel> OnChannelDisconnected;

        /// <summary>
        /// Canales conectados actualmente
        /// </summary>
        private Dictionary<string, IChannel> m_channels;

        /// <summary>
        /// Servidor para la red mesh
        /// </summary>
        private MeshTcpServer m_meshServer;

        /// <summary>
        /// Construir gestor de canales mesh
        /// </summary>
        public MeshChannelManager()
        {
            var configuration = ConfigurationManager.Get<IMeshAdapterConfiguration>();
            m_meshServer = new MeshTcpServer(configuration.ListenPort, configuration.KillOnConnect);
            m_meshServer.OnMessage += MeshServer_OnMessage;
            m_meshServer.OnMeshKilled += MeshServer_OnMeshKilled;
            m_channels = new Dictionary<string, IChannel>();
            Debug.WriteLine(this, "Gestor de canales mesh iniciado. Recibiendo conexiones en el puerto " + configuration.ListenPort, VerbosityLevel.Info);
        }

        /// <summary>
        /// Al desconectarse toda la red mesh
        /// </summary>
        /// <param name="sender">Emisor</param>
        /// <param name="e">Evento</param>
        private void MeshServer_OnMeshKilled(object sender, EventArgs e)
        {
            foreach (IChannel channel in m_channels.Values) {
                OnChannelDisconnected?.Invoke(this, channel);
                channel.Dispose();
            }
            m_channels = new Dictionary<string, IChannel>();
            Debug.WriteLine(this, "Canales purgados", VerbosityLevel.Debug);
        }

        /// <summary>
        /// Crear canal de comunicación mesh
        /// </summary>
        /// <param name="mac">Dirección MAC del canal</param>
        /// <param name="layer">Capa del dispositivo</param>
        private void CreateChannel(string mac, int layer = 1)
        {
            if (m_channels.ContainsKey(mac)) {
                Debug.WriteLine(this, $"Se está intentando conectar la dirección {mac} pero ya se está registrada, se ignorara el nuevo intento", VerbosityLevel.Warning);
                DeviceIntegrityRequest request = new DeviceIntegrityRequest();
                MeshChannel routingChannel = m_channels[mac] as MeshChannel;
                routingChannel.RouteMessage(request);
                return;
            }
            Debug.WriteLine(this, "Canal de comunicación creado: " + mac, VerbosityLevel.Debug);
            IChannel channel = new MeshChannel(mac, m_meshServer, layer);
            m_channels.Add(mac, channel);
            OnChannelConnected?.Invoke(this, channel);
        }

        /// <summary>
        /// Eliminar canal de comunicación mesh
        /// </summary>
        /// <param name="mac">Dirección MAC del canal</param>
        private void DeleteChannel(string mac)
        {
            if (m_channels.ContainsKey(mac)) {
                IChannel channel = m_channels[mac];
                channel.Dispose();
                m_channels.Remove(mac);
                OnChannelDisconnected?.Invoke(this, channel);
                Debug.WriteLine(this, "Canal de comunicación eliminado: " + mac, VerbosityLevel.Info);
            } else {
                Debug.WriteLine(this, "El canal requerido a eliminar no existe: " + mac, VerbosityLevel.Warning);
            }
        }

        /// <summary>
        /// Al recibir un mensaje de la red mesh
        /// </summary>
        /// <param name="sender">Emisor</param>
        /// <param name="e">Mensaje</param>
        private void MeshServer_OnMessage(object sender, string e)
        {
            Json.Parse<MeshMessage<ConnectionEvent>>(e, (cm) =>
            {
                switch (cm.Data.Value) {
                    case ConnectionEvent.Connected:
                        CreateChannel(cm.Mac, cm.Layer);
                        break;
                    case ConnectionEvent.Disconnected:
                        DeleteChannel(cm.Mac);
                        break;
                }
            }, settings: MeshTcpServer.SerializationSettings);
        }
    }
}
