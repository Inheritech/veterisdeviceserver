using System.Collections.ObjectModel;
using VeterisDeviceServer.Devices;
using VeterisDeviceServer.Models.Accessibility;
using VeterisDeviceServer.Models.IoT.Outbound;
using VeterisDeviceServer.Repositories;
using VeterisDeviceServer.Serialization;

namespace VeterisDeviceServer.Services
{
    /// <summary>
    /// Servicio de datos de dispositivos
    /// </summary>
    public class DevicesDataService
    {
        /// <summary>
        /// Repositorio de configuraciones
        /// </summary>
        private DeviceConfigurationRepository m_configRepo;

        /// <summary>
        /// Repositorio de traducciones
        /// </summary>
        private DeviceTranslationRepository m_transRepo;

        /// <summary>
        /// Inicializar servicio de datos de dispositivos
        /// </summary>
        public DevicesDataService()
        {
            m_configRepo = new DeviceConfigurationRepository();
            m_transRepo = new DeviceTranslationRepository();
        }

        /// <summary>
        /// Obtener dispositivos serializables
        /// </summary>
        public SerializableDevices GetSerializableDevices()
        {
            if (DeviceManager.Instance == null)
                return null;

            ReadOnlyCollection<Device> devices = DeviceManager.Instance.Devices;

            SerializableDevices sDevices = new SerializableDevices();

            foreach (Device dev in devices) {
                if (dev.Identity != null && dev.Status != null) {
                    DeviceConfiguration config = m_configRepo.GetConfiguration(dev.Identity.SerialNumber);
                    DeviceTranslation trans = m_transRepo.GetTranslation(dev.Identity.SerialNumber);
                    SerializableDevice device = new SerializableDevice(dev);
                    if (config != null) {
                        device.Configurations = config.Configuration;
                    }
                    if (trans != null) {
                        device.Translations = trans;
                    }
                    sDevices.Devices.Add(device);
                }
            }

            return sDevices;
        }
    }
}
