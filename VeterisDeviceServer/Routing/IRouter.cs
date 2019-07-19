using VeterisDeviceServer.Devices;
using VeterisDeviceServer.Users;

namespace VeterisDeviceServer.Routing
{
    /// <summary>
    /// Interfaz de routeo de mensajes
    /// </summary>
    public interface IRouter
    {
        /// <summary>
        /// Inicializar router
        /// </summary>
        /// <param name="devices">Gestor de dispositivos</param>
        /// <param name="users">Gestor de usuarios</param>
        void Init(DeviceManager devices, UserManager users);
    }
}
