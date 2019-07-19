using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using VeterisDeviceServer.Channels;
using VeterisDeviceServer.Diagnostics;
using VeterisDeviceServer.Models.IoT.Outbound;

namespace VeterisDeviceServer.Devices
{
    /// <summary>
    /// Gestor de dispositivos
    /// </summary>
    public class DeviceManager
    {
        /// <summary>
        /// Instancia Singleton
        /// </summary>
        public static DeviceManager Instance { get; private set; }

        /// <summary>
        /// Al conectarse un dispositivo
        /// </summary>
        public event EventHandler<Device> OnDeviceConnected;

        /// <summary>
        /// Al desconectarse un dispositivo
        /// </summary>
        public event EventHandler<Device> OnDeviceDisconnected;

        /// <summary>
        /// Obtener dispositivos actuales
        /// </summary>
        public ReadOnlyCollection<Device> Devices
        {
            get
            {
                return new ReadOnlyCollection<Device>(m_devices);
            }
        }

        /// <summary>
        /// Dispositivos actuales
        /// </summary>
        private List<Device> m_devices;

        /// <summary>
        /// Gestores de canales registrados
        /// </summary>
        private List<IChannelManager> m_channelManagers;

        /// <summary>
        /// Construir gestor de dispositivos
        /// </summary>
        public DeviceManager()
        {
            m_channelManagers = new List<IChannelManager>();
            m_devices = new List<Device>();

            if (Instance == null)
                Instance = this;
        }

        /// <summary>
        /// Añadir gestor de canales de comunicación
        /// </summary>
        /// <typeparam name="T">Tipo del gestor</typeparam>
        public T AddChannelManager<T>()
            where T : IChannelManager, new()
        {
            if (HasChannelManager<T>()) {
                Debug.WriteLine(this, "No se puede registrar un gestor de tipo " + typeof(T).Name + " porque ya hay uno registrado", VerbosityLevel.Warning);
                return default; // Mejor manera de lidiar con errores?
            }

            T manager = new T();

            manager.OnChannelConnected += Manager_OnChannelConnected;
            manager.OnChannelDisconnected += Manager_OnChannelDisconnected;

            m_channelManagers.Add(manager);
            return manager;
        }

        /// <summary>
        /// Al conectarse un nuevo canal
        /// </summary>
        /// <param name="sender">Emisor</param>
        /// <param name="e">Canal</param>
        private void Manager_OnChannelConnected(object sender, IChannel e)
        {
            Device device = new Device(e);
            m_devices.Add(device);
            OnDeviceConnected?.Invoke(this, device);
            device.CheckIntegrity();
        }

        /// <summary>
        /// Al desconectarse un canal
        /// </summary>
        /// <param name="sender">Emisor</param>
        /// <param name="e">Canal</param>
        private void Manager_OnChannelDisconnected(object sender, IChannel e)
        {
            Device[] devices = m_devices.Where(d => d.Channel == e).ToArray();
            foreach(Device device in devices) {
                m_devices.Remove(device);
                OnDeviceDisconnected?.Invoke(this, device);
            }
        }

        /// <summary>
        /// Determinar si un tipo de gestor de canales está registrado
        /// </summary>
        /// <typeparam name="T">Tipo del gestor</typeparam>
        public bool HasChannelManager<T>()
            where T : IChannelManager
        {
            return m_channelManagers.Any(cm => cm.GetType() == typeof(T));
        }

        /// <summary>
        /// Enviar objeto a dispositivo
        /// </summary>
        /// <typeparam name="T">Tipo de objeto</typeparam>
        /// <param name="serial">Numero de serie del dispositivo</param>
        /// <param name="obj">Objeto</param>
        public void Send<T>(string serial, T obj)
        {
            Device device = m_devices.SingleOrDefault(d => d.Identity != null && d.Identity.SerialNumber == serial);
            if (device != null) {
                Debug.WriteLine(this, $"Enviando actualización al dispositivo " + device.Identity.SerialNumber, VerbosityLevel.Debug);
                device.Channel.Write(obj);
            } else {
                Debug.WriteLine(this, $"No se pudo enviar un mensaje a {serial} porque no está registrado o no tiene identidad", VerbosityLevel.Debug);
            }
        }

        /// <summary>
        /// Obtener dispositivo según su numero de serie
        /// </summary>
        /// <param name="serial">Numero de serie</param>
        /// <returns></returns>
        public Device Get(string serial)
        {
            return m_devices.SingleOrDefault(d => d.Identity?.SerialNumber == serial);
        }

    }
}
