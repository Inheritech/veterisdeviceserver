using Newtonsoft.Json;
using System.Linq;
using VeterisDeviceServer.Database;
using VeterisDeviceServer.Database.Models;
using VeterisDeviceServer.Devices;
using VeterisDeviceServer.Helper;
using VeterisDeviceServer.Models.Accessibility;

namespace VeterisDeviceServer.Repositories
{
    /// <summary>
    /// Repositorio de traducciones de dispositivos
    /// </summary>
    public class DeviceTranslationRepository
    {
        /// <summary>
        /// Contexto de base de datos de servidor de dispositivos
        /// </summary>
        private readonly DeviceServerContext m_context;

        /// <summary>
        /// Inicializar repositorio
        /// </summary>
        public DeviceTranslationRepository()
        {
            m_context = new DeviceServerContext();
        }

        /// <summary>
        /// Determina si un dispositivo tiene una traducción guardada
        /// </summary>
        /// <param name="device">Dispositivo</param>
        /// <returns></returns>
        public bool HasTranslation(Device device)
        {
            if (device.Identity == null) return false;
            return HasTranslation(device.Identity.SerialNumber);
        }

        /// <summary>
        /// Determina si un numero de serie está registrado con una traducción guardada
        /// </summary>
        /// <param name="serialNumber">Numero de Serie</param>
        public bool HasTranslation(string serialNumber)
        {
            return m_context.SerializedDeviceTranslations.Any(sdt => sdt.DeviceSerialNumberId == serialNumber);
        }

        /// <summary>
        /// Obtener traducción de un dispositivo
        /// </summary>
        /// <param name="device">Dispositivo</param>
        /// <returns></returns>
        public DeviceTranslation GetTranslation(Device device)
        {
            if (device.Identity == null) return null;

            return GetTranslation(device.Identity.SerialNumber);
        }

        /// <summary>
        /// Obtener traducción de un dispositivo en base a su numero de serie
        /// </summary>
        /// <param name="serialNumber">Numero de Serie</param>
        public DeviceTranslation GetTranslation(string serialNumber)
        {
            SerializedDeviceTranslation deviceTranslation =
                m_context
                .SerializedDeviceTranslations
                .SingleOrDefault(sdt => sdt.DeviceSerialNumberId == serialNumber);

            if (deviceTranslation == null) return null;

            return Json.Parse<DeviceTranslation>(deviceTranslation.SerializedTranslation);
        }

        /// <summary>
        /// Eliminar traducción guardada
        /// </summary>
        /// <param name="device">Dispositivo</param>
        public bool DeleteTranslation(Device device)
        {
            if (device.Identity == null) return false;

            return DeleteTranslation(device.Identity.SerialNumber);
        }

        /// <summary>
        /// Eliminar traducción guaradada
        /// </summary>
        /// <param name="serialNumber">Numero de Serie</param>
        public bool DeleteTranslation(string serialNumber)
        {
            m_context
                .SerializedDeviceTranslations
                .RemoveRange(
                    m_context
                    .SerializedDeviceTranslations
                    .Where(sdt => sdt.DeviceSerialNumberId == serialNumber)
                );

            return m_context.SaveChanges() > 0;
        }

        /// <summary>
        /// Guardar traducción de dispositivo
        /// </summary>
        /// <param name="device">Dispositivo</param>
        /// <param name="translation">Traducción</param>
        public bool SaveTranslation(Device device, DeviceTranslation translation)
        {
            if (device.Identity == null) return false;

            return SaveTranslation(device.Identity.SerialNumber, translation);
        }

        /// <summary>
        /// Guardar configuración
        /// </summary>
        /// <param name="serialNumber">Numero de Serie</param>
        /// <param name="translation">Traducción</param>
        public bool SaveTranslation(string serialNumber, DeviceTranslation translation)
        {
            DeleteTranslation(serialNumber);

            m_context.SerializedDeviceTranslations.Add(new SerializedDeviceTranslation
            {
                DeviceSerialNumberId = serialNumber,
                SerializedTranslation = JsonConvert.SerializeObject(translation)
            });

            return m_context.SaveChanges() > 0;
        }
    }
}
