using VeterisDeviceServer.Devices;
using VeterisDeviceServer.Models.IoT.Inbound;
using VeterisDeviceServer.Users;

namespace VeterisDeviceServer.Routing
{
    class DeviceStatusRouter : IRouter
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
            e.OnStatus += Device_OnStatus;
        }

        /// <summary>
        /// Al desconectarse un dispositivo
        /// </summary>
        /// <param name="sender">Emisor</param>
        /// <param name="e">Dispositivo</param>
        private void DeviceManager_OnDeviceDisconnected(object sender, Device e)
        {
            e.OnStatus -= Device_OnStatus;
        }

        /// <summary>
        /// Al recibirse un estado de un dispositivo
        /// </summary>
        /// <param name="sender">Emisor</param>
        /// <param name="e">Estado</param>
        private void Device_OnStatus(object sender, DeviceStatus e)
        {
            m_userManager.Broadcast(e);
        }
    }
}
