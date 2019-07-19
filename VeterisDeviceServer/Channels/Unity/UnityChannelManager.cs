using System;
using System.Collections.Generic;
using VeterisDeviceServer.Configuration;
using VeterisDeviceServer.Diagnostics;
using VeterisDeviceServer.Helper;
using VeterisDeviceServer.Models.IoT.Inbound;
using WebSocketSharp;

namespace VeterisDeviceServer.Channels.Unity
{
    /// <summary>
    /// Gestor de canales de comunicacion de Unity ( Simulación )
    /// </summary>
    public class UnityChannelManager : IChannelManager
    {
        /// <summary>
        /// Al conectarse un canal
        /// </summary>
        public event EventHandler<IChannel> OnChannelConnected;

        /// <summary>
        /// Al desconectarse un canal
        /// </summary>
        public event EventHandler<IChannel> OnChannelDisconnected;

        /// <summary>
        /// Canales conectados actualmente
        /// </summary>
        private Dictionary<string, IChannel> m_channels;

        /// <summary>
        /// Servidor de comunicación para Unity
        /// </summary>
        private UnityCommServer m_commServer;

        public UnityChannelManager()
        {
            var configuration = ConfigurationManager.Get<IUnityConnection>();
            m_commServer = new UnityCommServer(configuration.Port);
            m_commServer.OnMessage += CommServer_OnMessage;
            m_commServer.OnClientKilled += CommServer_OnClientKilled;
            m_channels = new Dictionary<string, IChannel>();
            Debug.WriteLine(this, "Gestor de canales Unity iniciado. Recibiendo conexiones en el puerto: " + configuration.Port, VerbosityLevel.Info);
        }

        /// <summary>
        /// Al recibir un mensaje del cliente Unity
        /// </summary>
        /// <param name="sender">Emisor</param>
        /// <param name="e">Mensaje</param>
        private void CommServer_OnMessage(object sender, string e)
        {
            Json.Parse<UnityMessage<DeviceConnection>>(e, (cm) =>
            {
                switch (cm.Data.ConnectionEvent) {
                    case ConnectionEvent.Connected:
                        CreateChannel(cm.UnityId);
                        break;
                    case ConnectionEvent.Disconnected:
                        DeleteChannel(cm.UnityId);
                        break;
                }
            });
        }

        /// <summary>
        /// Crear canal de comunicación Unity
        /// </summary>
        /// <param name="unityId">Identificador de Unity para el canal</param>
        private void CreateChannel(string unityId)
        {
            Debug.WriteLine(this, "Canal de comunicación creado: " + unityId, VerbosityLevel.Debug);
            IChannel channel = new UnityChannel(unityId, m_commServer);
            m_channels.Add(unityId, channel);
            OnChannelConnected?.Invoke(this, channel);
        }

        private void DeleteChannel(string unityId)
        {
            if (m_channels.ContainsKey(unityId)) {
                IChannel channel = m_channels[unityId];
                channel.Dispose();
                m_channels.Remove(unityId);
                OnChannelDisconnected?.Invoke(this, channel);
                Debug.WriteLine(this, "Canal de comunicación eliminado: " + unityId, VerbosityLevel.Info);
            } else {
                Debug.WriteLine(this, "El canal requerido a eliminar no existe: " + unityId, VerbosityLevel.Warning);
            }
        }

        /// <summary>
        /// Al desconectarse el cliente de Unity
        /// </summary>
        /// <param name="sender">Emisor</param>
        /// <param name="e">Evento</param>
        private void CommServer_OnClientKilled(object sender, EventArgs e)
        {
            foreach(IChannel channel in m_channels.Values) {
                OnChannelDisconnected?.Invoke(this, channel);
                channel.Dispose();
            }
            m_channels = new Dictionary<string, IChannel>();
            Debug.WriteLine(this, "Canales purgados", VerbosityLevel.Debug);
        }

    }
}
