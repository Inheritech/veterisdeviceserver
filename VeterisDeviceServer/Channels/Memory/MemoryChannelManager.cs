using System;
using System.Collections.Generic;
using VeterisDeviceServer.Diagnostics;

namespace VeterisDeviceServer.Channels.Memory
{
    /// <summary>
    /// Gestor de canales en memoria
    /// </summary>
    public class MemoryChannelManager : IChannelManager
    {
        /// <summary>
        /// Al añadirse un dispositivo en memoria
        /// </summary>
        public event EventHandler<IChannel> OnChannelConnected;

        /// <summary>
        /// Al removerse un dispositivo en memoria
        /// </summary>
        public event EventHandler<IChannel> OnChannelDisconnected;

        /// <summary>
        /// Canales conectados actualmente
        /// </summary>
        private HashSet<IChannel> m_channels;

        /// <summary>
        /// Inicializar gestor
        /// </summary>
        public MemoryChannelManager() {
            m_channels = new HashSet<IChannel>();
            Debug.WriteLine(this, "Gestor de canales en memoria inicializado", VerbosityLevel.Info);
        }

        /// <summary>
        /// Añadir canal
        /// </summary>
        /// <param name="channel">Canal a añadir</param>
        public void AddChannel(IChannel channel) {
            if (m_channels.Contains(channel)) return;

            m_channels.Add(channel);
            OnChannelConnected?.Invoke(this, channel);
            Debug.WriteLine(this, "Canal en memoria añadido: " + channel.GetType().FullName, VerbosityLevel.Debug);
        }

        /// <summary>
        /// Eliminar canal
        /// </summary>
        /// <param name="channel">Canal a eliminar</param>
        public void RemoveChannel(IChannel channel) {
            if (!m_channels.Contains(channel)) return;

            m_channels.Remove(channel);
            OnChannelDisconnected?.Invoke(this, channel);
            Debug.WriteLine(this, "Canal en memoria eliminado: " + channel.GetType().FullName, VerbosityLevel.Debug);
        }

        /// <summary>
        /// Metodo fluent para añadir canal
        /// </summary>
        /// <param name="channel">Canal a añadir</param>
        public MemoryChannelManager Add(IChannel channel) {
            AddChannel(channel);
            return this;
        }

        /// <summary>
        /// Metodo fluent para eliminar canal
        /// </summary>
        /// <param name="channel">Canal a eliminar</param>
        public MemoryChannelManager Remove(IChannel channel) {
            RemoveChannel(channel);
            return this;
        }
    }
}
