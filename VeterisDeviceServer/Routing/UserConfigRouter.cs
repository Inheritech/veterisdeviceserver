using VeterisDeviceServer.Devices;
using VeterisDeviceServer.Diagnostics;
using VeterisDeviceServer.Models.IoT.Inbound;
using VeterisDeviceServer.Models.IoT.Outbound;
using VeterisDeviceServer.Repositories;
using VeterisDeviceServer.Users;

namespace VeterisDeviceServer.Routing
{
    /// <summary>
    /// Router de mensajes de configuración
    /// </summary>
    public class UserConfigRouter : IRouter
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
        /// Repositorio de configuraciones de dispositivos
        /// </summary>
        private DeviceConfigurationRepository m_configRepo;

        /// <summary>
        /// Inicializar router
        /// </summary>
        /// <param name="devices">Gestor de dispositivos</param>
        /// <param name="users">Gestor de usuarios</param>
        public void Init(DeviceManager devices, UserManager users)
        {
            m_configRepo = new DeviceConfigurationRepository();

            m_deviceManager = devices;
            m_deviceManager.OnDeviceConnected += DeviceManager_OnDeviceConnected;
            m_deviceManager.OnDeviceDisconnected += DeviceManager_OnDeviceDisconnected;

            m_userManager = users;
            m_userManager.OnUserConnected += UserManager_OnUserConnected;
            m_userManager.OnUserDisconnected += UserManager_OnUserDisconnected;
        }

        /// <summary>
        /// Al conectarse un dispositivo
        /// </summary>
        /// <param name="sender">Emisor</param>
        /// <param name="e">Dispositivo</param>
        private void DeviceManager_OnDeviceConnected(object sender, Device e)
        {
            e.OnIdentity += Device_OnIdentity;
        }

        /// <summary>
        /// Al establecerse la identidad del dispositivo
        /// </summary>
        /// <param name="sender">Emisor</param>
        /// <param name="e">Identidad</param>
        private void Device_OnIdentity(object sender, DeviceIdentity e)
        {
            if (sender is Device device) {
                device.OnIdentity -= Device_OnIdentity;
                DeviceConfiguration config = m_configRepo.GetConfiguration(e.SerialNumber);
                if (config != null) {
                    Debug.WriteLine(this, "Enviando configuración guardada al dispositivo: " + e.SerialNumber, VerbosityLevel.Debug);
                    device.Channel.Write(config);
                }
            }
        }

        /// <summary>
        /// Al desconectarse un dispositivo
        /// </summary>
        /// <param name="sender">Emisor</param>
        /// <param name="e">Dispositivo</param>
        private void DeviceManager_OnDeviceDisconnected(object sender, Device e)
        {
            e.OnIdentity -= Device_OnIdentity;
        }

        /// <summary>
        /// Al conectarse un usuario
        /// </summary>
        /// <param name="sender">Emisor</param>
        /// <param name="e">Usuario</param>
        private void UserManager_OnUserConnected(object sender, User e)
        {
            e.OnConfigureDevice += User_OnConfigureDevice;
        }

        /// <summary>
        /// Al desconectarse un usuario
        /// </summary>
        /// <param name="sender">Emisor</param>
        /// <param name="e">Usuario</param>
        private void UserManager_OnUserDisconnected(object sender, User e)
        {
            e.OnConfigureDevice -= User_OnConfigureDevice;
        }

        /// <summary>
        /// Al recibir una solicitud de configuración del usuario
        /// </summary>
        /// <param name="sender">Emisor</param>
        /// <param name="e">Configuración</param>
        private void User_OnConfigureDevice(object sender, DeviceConfiguration e)
        {
            if (m_configRepo.SaveConfiguration(e.SerialNumber, e)) {
                Debug.WriteLine(this, "Configuración almacenada en base de datos", VerbosityLevel.Debug);
            } else {
                Debug.WriteLine(this, "No se pudo almacenar la configuración en base de datos", VerbosityLevel.Warning);
            }
            m_deviceManager.Send(e.SerialNumber, e);
        }
    }
}
