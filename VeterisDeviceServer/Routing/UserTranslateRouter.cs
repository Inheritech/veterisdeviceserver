using VeterisDeviceServer.Devices;
using VeterisDeviceServer.Diagnostics;
using VeterisDeviceServer.Models.Accessibility;
using VeterisDeviceServer.Repositories;
using VeterisDeviceServer.Users;

namespace VeterisDeviceServer.Routing
{
    class UserTranslateRouter : IRouter
    {
        /// <summary>
        /// Gestor de usuarios
        /// </summary>
        private UserManager m_userManager;

        /// <summary>
        /// Repositorio de traducciones de dispositivos
        /// </summary>
        private DeviceTranslationRepository m_transRepo;

        /// <summary>
        /// Inicializar router
        /// </summary>
        /// <param name="devices">Gestor de dispositivos</param>
        /// <param name="users">Gestor de usuarios</param>
        public void Init(DeviceManager devices, UserManager users)
        {
            m_transRepo = new DeviceTranslationRepository();

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
            e.OnTranslateDevice += User_OnTranslateDevice;
        }

        /// <summary>
        /// Al desconectarse un usuario
        /// </summary>
        /// <param name="sender">Emisor</param>
        /// <param name="e">Usuario</param>
        private void UserManager_OnUserDisconnected(object sender, User e)
        {
            e.OnTranslateDevice -= User_OnTranslateDevice;
        }

        /// <summary>
        /// Al recibir una solicitud de traducción del usuario
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void User_OnTranslateDevice(object sender, DeviceTranslation e)
        {
            if (m_transRepo.SaveTranslation(e.SerialNumber, e)) {
                Debug.WriteLine(this, "Traducción almacenada en base de datos", VerbosityLevel.Debug);
            } else {
                Debug.WriteLine(this, "No se pudo almacenar la configuración en base de datos", VerbosityLevel.Warning);
            }
        }
    }
}
