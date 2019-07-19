using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using VeterisDeviceServer.Helper;
using VeterisDeviceServer.Security;

namespace VeterisDeviceServer.SSL
{
    /// <summary>
    /// Fuerza de una llave privada
    /// </summary>
    public enum KeyStrength
    {
        Bits512 = 512,
        Bits1024 = 1024,
        Bits2048 = 2048,
        Bits4096 = 4096
    }

    /// <summary>
    /// Clase de acceso a OpenSSL
    /// </summary>
    public static class OpenSSL
    {
        /// <summary>
        /// Extensión de una llave privada
        /// </summary>
        public const string EXT_PRIVATE_KEY = ".key";

        /// <summary>
        /// Extensión de una solicitud de firmado de certificado
        /// </summary>
        public const string EXT_SIGNING_REQUEST = ".csr";

        /// <summary>
        /// Extensión de un certificado
        /// </summary>
        public const string EXT_CERTIFICATE = ".crt";

        /// <summary>
        /// Extensión de un certificado PKCS12
        /// </summary>
        public const string EXT_PKCS12 = ".pfx";

        /// <summary>
        /// Extensión utilizada para autoridades de certificación auto-firmadas de Veteris
        /// </summary>
        public const string EXT_CERTIFICATE_AUTHORITY = ".pem";

        /// <summary>
        /// Ruta al ejecutable binario de OpenSSL para Windows
        /// </summary>
        public const string BIN_WINDOWS = "./Binaries/openssl.exe";

        /// <summary>
        /// Ruta al ejecutable binario de OpenSSL para Linux
        /// </summary>
        public const string BIN_LINUX = "./Binaries/openssl";

        /// <summary>
        /// Obtener ruta al ejecutable de OpenSSL para la plataforma actual
        /// </summary>
        public static string PlatformBinaryPath
        {
            get
            {
                if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows)) {
                    return BIN_WINDOWS;
                }
                return BIN_LINUX;
            }
        }

        /// <summary>
        /// Generar llave privada
        /// </summary>
        /// <param name="filename">Ruta para la llave</param>
        /// <param name="strength">Fuerza de la llave privada</param>
        /// <param name="maxWait">Máximo tiempo a esperar por el proceso de creación</param>
        /// <returns>Retorna el nombre del archivo generado o <see cref="null"/> en caso que no se haya podido generar en el tiempo establecido</returns>
        public static string GenerateKey(string filename, KeyStrength strength = KeyStrength.Bits2048, int maxWait = 2000)
        {
            Process sslProcess = CreateProcess()
                .WithArguments($"genrsa -out \"{filename}\" {(int)strength}");
            sslProcess.Start();
            if (sslProcess.WaitForExit(maxWait)) {
                return filename;
            } else {
                sslProcess.Kill();
                Files.DeleteIfExists(filename);
                return null;
            }
        }

        /// <summary>
        /// Generar solicitud de firmado de certificado
        /// </summary>
        /// <param name="requestfilename">Ruta para la solicitud</param>
        /// <param name="keyFilename">Ruta para la llave a utilizar</param>
        /// <param name="subj">Sujeto de la solicitud</param>
        /// <param name="maxWait">Máximo tiempo a esperar por el proceso de creación</param>
        /// <returns>Ruta a la solicitud de firmado creada en caso de exito o null en caso de error</returns>
        public static string GenerateCsr(string requestfilename, string keyFilename, CertificateSubject subj, int maxWait = 1000)
        {
            if (!File.Exists(keyFilename))
                throw new FileNotFoundException("No existe el archivo llave a utilizar", keyFilename);

            Process sslProcess = CreateProcess()
                .WithArguments($"req -new -key \"{keyFilename}\" -out \"{requestfilename}\" -subj \"{subj.Serialize()}\"");
            sslProcess.Start();
            if (sslProcess.WaitForExit(maxWait)) {
                return requestfilename;
            } else {
                sslProcess.Kill();
                Files.DeleteIfExists(requestfilename);
                return null;
            }
        }

        /// <summary>
        /// Generar certificado SSL
        /// </summary>
        /// <param name="crt">Ruta para el certificado</param>
        /// <param name="csr">Ruta a la solicitud de firmado a utilizar</param>
        /// <param name="ca">Ruta al certificado de la autoridad de certificación a utilizar</param>
        /// <param name="caKey">Ruta a la llave de la autoridad de certificación a utilizar</param>
        /// <param name="caPass">Contraseña de la llave de la autoridad de certificación a utilizar</param>
        /// <param name="days">Dias que durara el certificado antes de expirar</param>
        /// <param name="usePipe">Establecer si se debe utilizar un conducto nombrado para compartir la contraseña</param>
        /// <returns>Ruta al certificado creado en caso de exito o null en caso de error</returns>
        public static async Task<string> GenerateCrt(string crt, string csr, string ca, string caKey, string caPass, int days = 1825, int maxWait = 2000, bool usePipe = true)
        {
            if (!File.Exists(csr))
                throw new FileNotFoundException("No existe la solicitud de firmado provista", csr);

            if (!File.Exists(ca))
                throw new FileNotFoundException("No existe el certificado de CA provisto", ca);

            if (!File.Exists(caKey))
                throw new FileNotFoundException("No existe la llave del certificado de CA provista", caKey);

            string passAccess = null;

            PasswordPipe pipe = null;
            if (usePipe) {
                pipe = new PasswordPipe(caPass);
                caPass = null;
                passAccess = $"file:{pipe.Path}";
            } else {
                passAccess = $"pass:{caPass}";
            }

            Process sslProcess = CreateProcess()
                .WithArguments($"x509 -req -in \"{csr}\" -CA \"{ca}\" -CAkey \"{caKey}\" -passin \"{passAccess}\" -CAcreateserial -out \"{crt}\" -days {days} -sha256");
            sslProcess.Start();
            if (usePipe) {
                await pipe.WaitRead();
            }
            if (sslProcess.WaitForExit(maxWait)) {
                return crt;
            } else {
                sslProcess.Kill();
                Files.DeleteIfExists(crt);
                return null;
            }
        }


        /// <summary>
        /// Convertir certificado y llave a un certificado PKCS12
        /// </summary>
        /// <param name="pfx">Ruta para el certificado generado</param>
        /// <param name="crt">Certificado de entrada</param>
        /// <param name="key">Llave de para el certificado de entrada</param>
        /// <param name="pass">Contraseña a utilizar</param>
        /// <param name="maxWait">Máximo tiempo a esperar por el proceso de conversión</param>
        /// <param name="usePipe">Establecer si se debe utilizar un conducto nombrado para compartir la contraseña</param>
        /// <returns>Ruta al certificado creado o null en caso que haya fallado el proceso</returns>
        public static async Task<string> ConvertToPkcs12(string pfx, string crt, string key, string pass, int maxWait = 1000, bool usePipe = true)
        {
            if (!File.Exists(crt))
                throw new FileNotFoundException("No existe el certificado provisto", crt);

            if (!File.Exists(key))
                throw new FileNotFoundException("No existe la llave de certificado provista", key);

            string passAccess = null;

            PasswordPipe pipe = null;
            if (usePipe) {
                pipe = new PasswordPipe(pass);
                pass = null;
                passAccess = $"file:{pipe.Path}";
            } else {
                passAccess = $"pass:{pass}";
            }

            Process sslProcess = CreateProcess()
                .WithArguments($"pkcs12 -export -out \"{pfx}\" -inkey \"{key}\" -in \"{crt}\" -password \"{passAccess}\"");
            sslProcess.Start();
            if (usePipe) {
                await pipe.WaitRead();
            }
            if (sslProcess.WaitForExit(maxWait)) {
                return crt;
            } else {
                sslProcess.Kill();
                Files.DeleteIfExists(crt);
                return null;
            }
        }

        /// <summary>
        /// Añadir extensión a un nombre base para generar un nombre de archivo completo para
        /// una llave privada
        /// </summary>
        /// <param name="name">Nombre base</param>
        public static string FilenamePrivateKey(string name)
        {
            return name + EXT_PRIVATE_KEY;
        }

        /// <summary>
        /// Añadir extensión a un nombre base para generar un nombre de archivo completo para
        /// una solicitud de firmado
        /// </summary>
        /// <param name="name">Nombre base</param>
        public static string FilenameCsr(string name)
        {
            return name + EXT_SIGNING_REQUEST;
        }

        /// <summary>
        /// Añadir extensión a un nombre base para generar un nombre de archivo completo para un
        /// certificado
        /// </summary>
        /// <param name="name">Nombre base</param>
        public static string FilenameCrt(string name)
        {
            return name + EXT_CERTIFICATE;
        }

        /// <summary>
        /// Añadir extensión a un nombre base para generar un nombre de archivo completo para un
        /// certificado PKCS12
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public static string FilenamePkcs12(string name)
        {
            return name + EXT_PKCS12;
        }

        /// <summary>
        /// Crear proceso sin iniciar de OpenSSL
        /// </summary>
        public static Process CreateProcess()
        {
            ProcessStartInfo startInfo = new ProcessStartInfo
            {
                FileName = PlatformBinaryPath
            };

            Process process = new Process
            {
                StartInfo = startInfo
            };

            return process;
        }

        /// <summary>
        /// Añadir argumentos a un proceso
        /// </summary>
        /// <param name="process">Proceso</param>
        /// <param name="args">Argumentos</param>
        public static Process WithArguments(this Process process, string args)
        {
            process.StartInfo.Arguments = args;
            return process;
        }
    }
}
