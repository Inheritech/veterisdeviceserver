using Config.Net;

namespace VeterisDeviceServer.Configuration
{
    /// <summary>
    /// Datos de credenciales del servidor
    /// </summary>
    public interface IServerCredential
    {
        /// <summary>
        /// Identificador de servidor para esta credencial
        /// </summary>
        [Option(Alias = "ServerIdentifier")]
        string ServerId { get; set; }

        /// <summary>
        /// Identificador del servidor
        /// </summary>
        [Option(Alias = "CredentialIdentifier")]
        string CredentialId { get; set; }

        /// <summary>
        /// Secreto del servidor
        /// </summary>
        [Option(Alias = "CredentialSecret")]
        string CredentialSecret { get; set; }
    }
}
