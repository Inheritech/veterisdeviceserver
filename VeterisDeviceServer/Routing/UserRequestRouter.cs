using VeterisDeviceServer.Configuration;
using VeterisDeviceServer.Devices;
using VeterisDeviceServer.Models.Users;
using VeterisDeviceServer.Services;
using VeterisDeviceServer.Users;

namespace VeterisDeviceServer.Routing
{
    /// <summary>
    /// Routeador de solicitudes de datos de usuarios
    /// </summary>
    class UserRequestRouter : IRouter
    {
        /// <summary>
        /// Solicitud de obtención de dispositivos
        /// </summary>
        public const string DATA_REQUEST_DEVICES = "getDevices";

        /// <summary>
        /// Ping request through TCP
        /// </summary>
        public const string PING = "ping";

        /// <summary>
        /// Gestor de usuarios
        /// </summary>
        private UserManager m_userManager;

        /// <summary>
        /// Servicio de datos de dispositivos
        /// </summary>
        private DevicesDataService m_devicesDataService;

        /// <summary>
        /// Inicializar router
        /// </summary>
        /// <param name="devices">Gestor de dispositivos</param>
        /// <param name="users">Gestor de usuarios</param>
        public void Init(DeviceManager devices, UserManager users)
        {
            m_devicesDataService = new DevicesDataService();

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
            e.OnDataRequest += User_OnDataRequest;
        }

        /// <summary>
        /// Al desconectarse un usuario
        /// </summary>
        /// <param name="sender">Emisor</param>
        /// <param name="e">Usuario</param>
        private void UserManager_OnUserDisconnected(object sender, User e)
        {
            e.OnDataRequest -= User_OnDataRequest;
        }

        /// <summary>
        /// Al recibir una solicitud de datos de un usuario
        /// </summary>
        /// <param name="sender">Emisor</param>
        /// <param name="e">Solicitud</param>
        private void User_OnDataRequest(object sender, UserDataRequest e)
        {
            if (sender is User user) {
                if (e.DataRequest == DATA_REQUEST_DEVICES) {
                    user.Write(m_devicesDataService.GetSerializableDevices());
                }
                if (e.DataRequest == PING) {
                    var credential = ConfigurationManager.Get<IServerCredential>();
                    var response = new PingResponse(credential != null ? credential.ServerId : "");
                    user.Write(response);
                }
            }
        }
    }
}
