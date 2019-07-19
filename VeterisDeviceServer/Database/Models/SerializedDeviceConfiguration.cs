using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace VeterisDeviceServer.Database.Models
{
    /// <summary>
    /// Configuración serializada de un dispositivo
    /// </summary>
    [Table("serialized_device_config")]
    public class SerializedDeviceConfiguration
    {
        /// <summary>
        /// Identificador del dispositivo
        /// </summary>
        [Key, Column("id"), DatabaseGenerated(DatabaseGeneratedOption.None)]
        public string DeviceSerialNumberId { get; set; }

        /// <summary>
        /// Configuración serializada en JSON
        /// </summary>
        [Column("serialized")]
        public string SerializedConfiguration { get; set; }
    }
}
