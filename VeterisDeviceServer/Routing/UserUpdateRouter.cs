using VeterisDeviceServer.Devices;
using VeterisDeviceServer.Models.IoT.Outbound;
using VeterisDeviceServer.Users;

namespace VeterisDeviceServer.Routing
{
    /// <summary>
    /// Router de mensajes de actualización
    /// </summary>
    public class UserUpdateRouter : IRouter
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
            m_userManager = users;
            m_userManager.OnUserConnected += UserManager_OnUserConnected;
            m_userManager.OnUserDisconnected += UserManager_OnUserDisconnected;
        }

        /// <summary>
        /// Al conectarse un usuario
        /// </summary>
        /// <param name="sender">Emisor</param>
        /// <param name="e">Usuario</param>
        private void UserManager_OnUserConnected(object sender, User e)
        {
            e.OnRequestsUpdate += User_OnRequestsUpdate;
        }

        /// <summary>
        /// Al desconectarse un usuario
        /// </summary>
        /// <param name="sender">Emisor</param>
        /// <param name="e">Usuario</param>
        private void UserManager_OnUserDisconnected(object sender, User e)
        {
            e.OnRequestsUpdate -= User_OnRequestsUpdate;
        }

        /// <summary>
        /// Al recibir una solicitud de actualización de un usuario
        /// </summary>
        /// <param name="sender">Emisor</param>
        /// <param name="e">Actualización de dispositivo</param>
        private void User_OnRequestsUpdate(object sender, DeviceUpdate e)
        {
            m_deviceManager.Send(e.SerialNumber, e);
        }
    }
}
