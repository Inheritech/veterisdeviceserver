using System;

namespace VeterisDeviceServer.Channels
{
    /// <summary>
    /// Canal de comunicaciones con un dispositivo
    /// </summary>
    public interface IChannel : IDisposable
    {
        /// <summary>
        /// Al recibir un mensaje desde este canal
        /// </summary>
        event EventHandler<string> OnMessage;

        /// <summary>
        /// Escribir al canal
        /// </summary>
        /// <param name="obj">Objeto a escribir</param>
        void Write<T>(T obj);
    }
}