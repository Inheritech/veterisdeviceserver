using System;
using System.IO;
using System.IO.Pipes;
using System.Runtime.InteropServices;
using System.Security;
using System.Threading.Tasks;

namespace VeterisDeviceServer.Security
{
    /// <summary>
    /// Clase para compartir un secreto como una contraseña con otro proceso
    /// </summary>
    public class PasswordPipe
    {
        /// <summary>
        /// Nombre del conducto
        /// </summary>
        public string Name { get; }

        /// <summary>
        /// Ruta al conducto
        /// </summary>
        public string Path
        {
            get
            {
                if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows)) {
                    return $@"\\.\pipe\{Name}";
                } else {
                    return Name;
                }
            }
        }

        /// <summary>
        /// Conducto de información por el cual enviar el secreto
        /// </summary>
        private NamedPipeServerStream _namedPipe;

        /// <summary>
        /// Secreto a compartir por este conducto
        /// </summary>
        private SecureString _secret;

        /// <summary>
        /// Construir conducto nombrado para compartir un secreto
        /// </summary>
        /// <param name="secret">Secreto a compartir</param>
        public PasswordPipe(string secret)
        {
            _secret = new SecureString();
            foreach (char c in secret) {
                _secret.AppendChar(c);
            }
            _secret.MakeReadOnly();
            Name = GetRandomName();
            _namedPipe = new NamedPipeServerStream(Name, PipeDirection.Out, 1);
        }

        /// <summary>
        /// Esperar a que el contenido sea leido
        /// </summary>
        public async Task WaitRead()
        {
            await _namedPipe.WaitForConnectionAsync();
            StreamWriter writer = new StreamWriter(_namedPipe);
            IntPtr secureStringPointer = Marshal.SecureStringToBSTR(_secret);
            string secretInsecure = Marshal.PtrToStringBSTR(secureStringPointer);
            Marshal.ZeroFreeBSTR(secureStringPointer);
            await writer.WriteLineAsync(secretInsecure);
            await writer.FlushAsync();
            _namedPipe.WaitForPipeDrain();
            _secret.Dispose();
            _secret = null;
        }

        /// <summary>
        /// Obtener un nombre aleatorio para un conducto
        /// </summary>
        public static string GetRandomName()
        {
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows)) {
                return SecureRandom.RandomToken();
            } else {
                string pipeName = SecureRandom.RandomToken(16);
                string pipeExt = SecureRandom.RandomToken(16);
                return pipeName + "." + pipeExt;
            }
        }
    }
}
