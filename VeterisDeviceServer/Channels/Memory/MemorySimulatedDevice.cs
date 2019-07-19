using System;
using System.Collections.Generic;
using System.Text;

namespace VeterisDeviceServer.Channels.Memory
{
    /// <summary>
    /// <para>Dispositivo simulado en memoria</para>
    /// <para>Esta clase responde como un dispositivo IoT, emite identidad y estado según necesario</para>
    /// </summary>
    internal class MemorySimulatedDevice : Dictionary<string, object>
    {
        /// <summary>
        /// Numero de serie
        /// </summary>
        public string SerialNumber { get; set; }

        /// <summary>
        /// Inicializar dispositivo con
        /// numero de serie
        /// </summary>
        /// <param name="serialNumber">Numero de serie a utilizar</param>
        public MemorySimulatedDevice(string serialNumber) {
            SerialNumber = serialNumber;
        }
    }
}
