using System.Collections.Generic;
using System.Linq;
using Veteris.DomoticServer;
using VeterisDeviceServer.Devices;
using VeterisDeviceServer.Diagnostics;
using VeterisDeviceServer.Models.IoT.Inbound;
using VeterisDeviceServer.Users;

namespace VeterisDeviceServer.Routing
{
    class DeviceEventRouter : IRouter
    {
        /// <summary>
        /// Gestor de usuarios
        /// </summary>
        private UserManager m_userManager;

        /// <summary>
        /// Gestor de dispositivos
        /// </summary>
        private DeviceManager m_deviceManager;

        /// <summary>
        /// Inicializar router
        /// </summary>
        /// <param name="devices">Gestor de dispositivos</param>
        /// <param name="users">Gestor de usuarios</param>
        public void Init(DeviceManager devices, UserManager users)
        {
            m_deviceManager = devices;
            m_deviceManager.OnDeviceConnected += DeviceManager_OnDeviceConnected;
            m_deviceManager.OnDeviceDisconnected += DeviceManager_OnDeviceDisconnected;
            m_userManager = users;
        }

        /// <summary>
        /// Al conectarse un dispositivo
        /// </summary>
        /// <param name="sender">Emisor</param>
        /// <param name="e">Dispositivo</param>
        private void DeviceManager_OnDeviceConnected(object sender, Device e)
        {
            e.OnEvent += Device_OnEvent;
        }

        /// <summary>
        /// Al desconectarse un dispositivo
        /// </summary>
        /// <param name="sender">Emisor</param>
        /// <param name="e">Dispositivo</param>
        private void DeviceManager_OnDeviceDisconnected(object sender, Device e)
        {
            e.OnEvent -= Device_OnEvent;
        }

        /// <summary>
        /// Al emitirse un evento desde un dispositivo
        /// </summary>
        /// <param name="sender">Emisor</param>
        /// <param name="e">Argumentos</param>
        private void Device_OnEvent(object sender, DeviceEvent e)
        {
            Debug.WriteLine(this, "Se recibio un evento: " + e.Name, VerbosityLevel.Info);
            HandleCloudEvent(e);
            if (e.Target != null) {
                Device target = m_deviceManager.Get(e.Target);
                if (target == null) {
                    Debug.WriteLine(this, $"No se pudo enviar un evento '{e.Name}' a '{e.Target}' porque no está registrado", VerbosityLevel.Warning);
                    return;
                }
                if (!target.Identity.ListensTo.Contains(e.Name)) {
                    Debug.WriteLine(this, $"No se pudo enviar un evento '{e.Name}' a '{e.Target}' porque no escucha al evento", VerbosityLevel.Info);
                    return;
                }
                Debug.WriteLine(this, $"Redirigiendo evento '{e.Name}' de {e.SerialNumber} dirigido a '{e.Target}'", VerbosityLevel.Default);
                target.Channel.Write(e);
            } else {
                Device[] listeners = m_deviceManager.Devices.Where(d => d.Identity != null && d.Identity.ListensTo.Contains(e.Name)).ToArray();
                if (listeners.Length == 0) {
                    Debug.WriteLine(this, $"No se emitira el evento '{e.Name}' porque no tiene dispositivos que lo escuchen", VerbosityLevel.Info);
                    return;
                }
                Debug.WriteLine(this, $"Emitiendo evento '{e.Name}' de '{e.SerialNumber}' a {listeners.Length} dispositivos que lo escuchan", VerbosityLevel.Default);
                foreach (Device device in listeners) {
                    device.Channel.Write(e);
                }
            }
        }

        /// <summary>
        /// Manejar evento que podria corresponderle a la nube
        /// </summary>
        /// <param name="e">Evento</param>
        private void HandleCloudEvent(DeviceEvent e) {
            if (e.Name == "house.security") {
                if (e.Arguments.ContainsKey("open")) {
                    if (e.Arguments["open"] is bool openArgument && openArgument) {
                        EmitCloudEvent("house.open");
                    }
                }
            }
        }

        /// <summary>
        /// Emitir evento a la nube
        /// </summary>
        /// <param name="name">Nombre del evento</param>
        private void EmitCloudEvent(string name) {
            Debug.WriteLine(this, $"Emitiendo evento a la nube de tipo: {name}", VerbosityLevel.Info);
            var cloudEmitResult = Domotic.Cloud.Events.SendEvent(name);
            Debug.WriteLine(this, $"Resultado de emisión de evento: {cloudEmitResult.StatusCode}");
        }
    }
}
