using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace VeterisDeviceServer.Database.Models
{
    /// <summary>
    /// Rol de usuario sobre un servidor
    /// </summary>
    public enum UserRole
    {
        /// <summary>
        /// Administrador del sistema
        /// </summary>
        Administrator,
        /// <summary>
        /// Usuario minimo del sistema
        /// </summary>
        User
    }

    /// <summary>
    /// Modelo de acceso para usuario de servidor
    /// </summary>
    public class ServerUserAccess
    {
        /// <summary>
        /// Identificador del acceso
        /// </summary>
        [Key, Column("id"), DatabaseGenerated(DatabaseGeneratedOption.None)]
        public string Identifier { get; set; }

        /// <summary>
        /// identificador del servidor
        /// </summary>
        [Column("server_identifier")]
        public string ServerIdentifier { get; set; }

        /// <summary>
        /// Identificador del usuario
        /// </summary>
        [Column("user_identifier")]
        public string UserIdentifier { get; set; }

        /// <summary>
        /// Rol de usuario
        /// </summary>
        [JsonConverter(typeof(StringEnumConverter))]
        [Column("role")]
        public UserRole Role { get; set; }

        /// <summary>
        /// Convertir de modelo a API a modelo de base de datos
        /// </summary>
        /// <param name="access">Modelo a convertir</param>
        public static ServerUserAccess FromApiModel(Veteris.DomoticServer.Models.ServerUserAccess access)
        {
            return new ServerUserAccess
            {
                Identifier = access.Identifier,
                Role = (UserRole)(int)access.Role,
                ServerIdentifier = access.ServerIdentifier,
                UserIdentifier = access.UserIdentifier
            };
        }
    }
}
