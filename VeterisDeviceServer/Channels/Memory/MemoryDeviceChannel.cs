using System;
using System.Collections.Generic;
using System.Text;

namespace VeterisDeviceServer.Channels.Memory
{
    /// <summary>
    /// Canal de dispositivo en memoria
    /// </summary>
    internal sealed class MemoryDeviceChannel : IChannel
    {
        /// <summary>
        /// Al recibir un mensaje
        /// </summary>
        public event EventHandler<string> OnMessage;

        public void Dispose() {
            throw new NotImplementedException();
        }

        public void Write<T>(T obj) {
            throw new NotImplementedException();
        }
    }
}
