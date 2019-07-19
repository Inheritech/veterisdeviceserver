using Newtonsoft.Json;
using System.Linq;
using VeterisDeviceServer.Database;
using VeterisDeviceServer.Database.Models;
using VeterisDeviceServer.Devices;
using VeterisDeviceServer.Helper;
using VeterisDeviceServer.Models.IoT.Outbound;

namespace VeterisDeviceServer.Repositories
{
    /// <summary>
    /// Repositorio de configuraciones de dispositivos
    /// </summary>
    public class DeviceConfigurationRepository
    {
        /// <summary>
        /// Contexto de base de datos de servidor de dispositivos
        /// </summary>
        private readonly DeviceServerContext m_context;

        /// <summary>
        /// Inicializar repositorio
        /// </summary>
        public DeviceConfigurationRepository()
        {
            m_context = new DeviceServerContext();
        }

        /// <summary>
        /// Determina si un dispositivo tiene una configuración guardada
        /// </summary>
        /// <param name="device"></param>
        public bool HasConfiguration(Device device)
        {
            if (device.Identity == null) return false;

            return HasConfiguration(device.Identity.SerialNumber);
        }

        /// <summary>
        /// Determina si un numero de serie está registrado con una configuración guardada
        /// </summary>
        /// <param name="serialNumber">Numero de Serie</param>
        public bool HasConfiguration(string serialNumber)
        {
            return m_context.SerializedDeviceConfigurations.Any(sdc => sdc.DeviceSerialNumberId == serialNumber);
        }

        /// <summary>
        /// Obtener configuración de un dispositivo
        /// </summary>
        /// <param name="device">Dispositivo</param>
        public DeviceConfiguration GetConfiguration(Device device)
        {
            if (device.Identity == null) return null;

            return GetConfiguration(device.Identity.SerialNumber);
        }

        /// <summary>
        /// Obtener configuración de un dispositivo en base a su numero de serie
        /// </summary>
        /// <param name="serialNumber">Numero de Serie</param>
        public DeviceConfiguration GetConfiguration(string serialNumber)
        {
            SerializedDeviceConfiguration deviceConfiguration = 
                m_context
                .SerializedDeviceConfigurations
                .SingleOrDefault(sdc => sdc.DeviceSerialNumberId == serialNumber);

            if (deviceConfiguration == null) return null;

            return Json.Parse<DeviceConfiguration>(deviceConfiguration.SerializedConfiguration);
        }

        /// <summary>
        /// Eliminar configuración guardada
        /// </summary>
        /// <param name="device">Dispositivo</param>
        public bool DeleteConfiguration(Device device)
        {
            if (device.Identity == null) return false;

            return DeleteConfiguration(device.Identity.SerialNumber);
        }

        /// <summary>
        /// Eliminar configuración guaradada
        /// </summary>
        /// <param name="serialNumber">Numero de Serie</param>
        public bool DeleteConfiguration(string serialNumber)
        {
            m_context
                .SerializedDeviceConfigurations
                .RemoveRange(
                    m_context
                    .SerializedDeviceConfigurations
                    .Where(sdc => sdc.DeviceSerialNumberId == serialNumber)
                );

            return m_context.SaveChanges() > 0;
        }

        /// <summary>
        /// Guardar configuración de dispositivo
        /// </summary>
        /// <param name="device">Dispositivo</param>
        /// <param name="config">Configuración</param>
        public bool SaveConfiguration(Device device, DeviceConfiguration config)
        {
            if (device.Identity == null) return false;

            return SaveConfiguration(device.Identity.SerialNumber, config);
        }

        /// <summary>
        /// Guardar configuración
        /// </summary>
        /// <param name="serialNumber">Numero de Serie</param>
        /// <param name="config">Configuración</param>
        /// <returns></returns>
        public bool SaveConfiguration(string serialNumber, DeviceConfiguration config)
        {
            DeleteConfiguration(serialNumber);

            m_context.SerializedDeviceConfigurations.Add(new SerializedDeviceConfiguration
            {
                DeviceSerialNumberId = serialNumber,
                SerializedConfiguration = JsonConvert.SerializeObject(config)
            });

            return m_context.SaveChanges() > 0;
        }

    }
}
