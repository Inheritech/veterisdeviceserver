using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace VeterisDeviceServer.Database.Models
{
    /// <summary>
    /// Traducción serializada de un dispositivo
    /// </summary>
    [Table("serialized_device_trans")]
    public class SerializedDeviceTranslation
    {
        /// <summary>
        /// Identificador del dispositivo
        /// </summary>
        [Key, Column("id"), DatabaseGenerated(DatabaseGeneratedOption.None)]
        public string DeviceSerialNumberId { get; set; }

        /// <summary>
        /// Traducción serializada en JSON
        /// </summary>
        [Column("serialized")]
        public string SerializedTranslation { get; set; }
    }
}
