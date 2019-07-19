using System;

namespace VeterisDeviceServer.Channels
{
    /// <summary>
    /// Interfaz de manejo para canales de comunicación con dispositivos
    /// </summary>
    public interface IChannelManager
    {
        /// <summary>
        /// Al conectarse un nuevo canal de comunicación
        /// </summary>
        event EventHandler<IChannel> OnChannelConnected;

        /// <summary>
        /// Al desconectarse un canal
        /// </summary>
        event EventHandler<IChannel> OnChannelDisconnected;
    }
}
