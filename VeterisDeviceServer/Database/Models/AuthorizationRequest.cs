using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace VeterisDeviceServer.Database.Models
{
    /// <summary>
    /// Solicitudes de autorización de dispositivos
    /// </summary>
    [Table("authorization_requests")]
    public class AuthorizationRequest
    {
        /// <summary>
        /// Identificador de la autorización
        /// </summary>
        [Key, Column("id")]
        public long AuthorizationRequestId { get; set; }

        /// <summary>
        /// Numero de Serie del dispositivo
        /// </summary>
        [Column("serial_number")]
        public string DeviceSerialNumber { get; set; }

        /// <summary>
        /// Fecha de creación
        /// </summary>
        [Column("created_at")]
        public DateTime CreatedAt { get; set; }

    }
}
