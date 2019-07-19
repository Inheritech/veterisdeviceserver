using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace VeterisDeviceServer.Database.Models
{
    public enum ServerAccessRole
    {
        Owner,
        Administrator,
        User
    }

    /// <summary>
    /// Usuarios registrados en el sistema
    /// </summary>
    [Table("user_identities")]
    public class UserIdentity
    {
        /// <summary>
        /// Longitud del identificador de la llave
        /// </summary>
        public const int SERVER_KEY_LENGTH = 128;

        /// <summary>
        /// Identificador del usuario
        /// </summary>
        [Key, Column("id"), DatabaseGenerated(DatabaseGeneratedOption.None)]
        public string UserId { get; set; }

        /// <summary>
        /// Rol que tiene el usuario sobre el servidor
        /// </summary>
        [Column("role")]
        public ServerAccessRole Role { get; set; }

        /// <summary>
        /// Identificador de la llave
        /// </summary>
        [Column("key_id"), DatabaseGenerated(DatabaseGeneratedOption.None), StringLength(SERVER_KEY_LENGTH)]
        public string KeyId { get; set; }

        /// <summary>
        /// Llave privada del usuario para este sistema
        /// </summary>
        [Column("key")]
        public string PrivateKey { get; set; }

    }
}
