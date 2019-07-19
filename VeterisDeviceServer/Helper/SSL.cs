using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Runtime.InteropServices;
using System.Security.Cryptography.X509Certificates;
using Veteris.Domotic.Interaction.Helper;
using VeterisDeviceServer.Configuration;
using VeterisDeviceServer.Diagnostics;
using VeterisDeviceServer.SSL;

namespace VeterisDeviceServer.Helper
{
    /// <summary>
    /// Estado actual del certificado global
    /// </summary>
    public enum SSLCertificateStatus
    {
        /// <summary>
        /// No cargado
        /// </summary>
        NotLoaded,
        /// <summary>
        /// Ruta invalida
        /// </summary>
        InvalidPath,
        /// <summary>
        /// No existe
        /// </summary>
        NotExists,
        /// <summary>
        /// Cargado
        /// </summary>
        Loaded
    }

    /// <summary>
    /// Clase de ayuda para manejar certificados SSL
    /// </summary>
    public static class SSL
    {
        /// <summary>
        /// Nombre base para el archivo CA de soporte ATS de Veteris
        /// </summary>
        public const string FILENAME_CA = "VeterisATS";

        /// <summary>
        /// Contraseña por default para la autoridad de certificación
        /// </summary>
        public const string PASSWORD_DEFAULT_CA = "localdeployment";

        /// <summary>
        /// Ruta al directorio de certificados
        /// </summary>
        public const string DIRECTORY_CERTS = "./Certs/";

        /// <summary>
        /// Nombre de la variable de entorno que se refiere a la configuración de OpenSSL
        /// </summary>
        public const string ENV_OPENSSL_CONF = "OPENSSL_CONF";

        /// <summary>
        /// Ruta relativa al archivo de configuración de OpenSSL
        /// </summary>
        public const string CONF_OPENSSL_DEFAULT = "./Binaries/openssl.cnf";

        /// <summary>
        /// Estado actual del certificado global
        /// </summary>
        public static SSLCertificateStatus Status
        {
            get
            {
                if (!m_loadTried) {
                    LoadConfiguration();
                }

                return m_status;
            }
        }

        /// <summary>
        /// Certificado global del sistema
        /// </summary>
        public static X509Certificate2 GlobalCertificate
        {
            get
            {
                if (!m_loadTried) {
                    LoadConfiguration();
                }

                return m_globalCert;
            }
        }

        /// <summary>
        /// Estado actual del certificado global
        /// </summary>
        private static SSLCertificateStatus m_status;

        /// <summary>
        /// Certificado global del sistema
        /// </summary>
        private static X509Certificate2 m_globalCert;

        /// <summary>
        /// Determina si se ha intentado cargar el certificado
        /// </summary>
        private static bool m_loadTried;

        /// <summary>
        /// Inicializar datos estaticos
        /// </summary>
        static SSL()
        {
            string currentDir = Directory.GetCurrentDirectory();
            Environment.SetEnvironmentVariable(ENV_OPENSSL_CONF, Path.Combine(currentDir, CONF_OPENSSL_DEFAULT));
            Debug.WriteLine("SSL", "Establecida ruta a OpenSSL como: " + Environment.GetEnvironmentVariable(ENV_OPENSSL_CONF), VerbosityLevel.Debug);
        }

        /// <summary>
        /// Tratar de cargar el certificado según la configuración global
        /// </summary>
        public static void LoadConfiguration()
        {
            m_loadTried = true;
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12 | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls;
            var configuration = ConfigurationManager.Get<ISSLConfiguration>();
            if (configuration.Secure) {
                if (configuration.AutoGenerate) {
                    Debug.WriteLine("SSLAutogen", "Inicializando SSL Autogen", VerbosityLevel.Info);
                    AutoGenerateCertificate();
                } else {
                    LoadCertificateFromConfiguration(configuration);
                }
            }
        }

        /// <summary>
        /// Generar y/o utilizar un certificado generado automaticamente
        /// </summary>
        public static void AutoGenerateCertificate()
        {
            IPAddress main = NetworkUtilities.GetMainLocalIPAddress();
            Debug.WriteLine("SSLAutogen", "Revisando certificado SSL para dirección IP principal: " + main.ToString(), VerbosityLevel.Info);
            CheckAutoGenerateCertificate(main);
        }

        /// <summary>
        /// Revisar si un certificado existe para determinada dirección IP
        /// </summary>
        /// <param name="address">Dirección IP</param>
        public static void CheckAutoGenerateCertificate(IPAddress address)
        {
            string filepath = GetAutogenCertFilepath(address);
            if (!File.Exists(filepath)) {
                AutoGenerateCertificate(address);
            }
            m_globalCert = new X509Certificate2(filepath, PASSWORD_DEFAULT_CA);
            m_status = SSLCertificateStatus.Loaded;
            Debug.WriteLine("SSLAutogen", $"Certificado para '{address}' generado y cargado", VerbosityLevel.Info);
        }

        /// <summary>
        /// Generar certificado nuevo
        /// </summary>
        /// <param name="address">Dirección IP</param>
        public static string AutoGenerateCertificate(IPAddress address)
        {
            bool usePipe = RuntimeInformation.IsOSPlatform(OSPlatform.Windows);
            string baseName = address.ToString();
            Debug.WriteLine("SSLAutogen", "Generando llave para nuevo certificado de IP: " + address, VerbosityLevel.Debug);
            string key = OpenSSL.GenerateKey(GetCert(OpenSSL.FilenamePrivateKey(baseName)), maxWait: 10000);
            Debug.WriteLine("SSLAutogen", "Generando solicitud de firmado para nuevo certificado de IP: " + address, VerbosityLevel.Debug);
            string csr = OpenSSL.GenerateCsr(GetCert(OpenSSL.FilenameCsr(baseName)), key, new CertificateSubject
            {
                CommonName = baseName,
                Country = "MX",
                State = "Aguascalientes",
                Locality = "Aguascalientes",
                Organization = "Veteris de Aguascalientes",
                OrganizationUnit = "Automated Server Deployment"
            }, maxWait: 10000);
            string ca = GetCert(FILENAME_CA + OpenSSL.EXT_CERTIFICATE_AUTHORITY);
            string caKey = GetCert(FILENAME_CA + OpenSSL.EXT_PRIVATE_KEY);
            Debug.WriteLine("SSLAutogen", "Generando certificado de IP: " + address, VerbosityLevel.Debug);
            string crt = OpenSSL.GenerateCrt(GetCert(OpenSSL.FilenameCrt(baseName)), csr, ca, caKey, PASSWORD_DEFAULT_CA, maxWait: 10000, usePipe: usePipe).Result;
            Debug.WriteLine("SSLAutogen", "Convirtiendo certificado de IP: " + address + ", a formato PKCS12", VerbosityLevel.Debug);
            string pfx = OpenSSL.ConvertToPkcs12(GetCert(OpenSSL.FilenamePkcs12(baseName)), crt, key, PASSWORD_DEFAULT_CA, maxWait: 10000, usePipe: usePipe).Result;
            Debug.WriteLine("SSLAutogen", "Se creo el certificado PKCS12 de IP: " + address, VerbosityLevel.Debug);
            return pfx;
        }

        /// <summary>
        /// Cargar certificado SSL desde la ruta establecida en configuración
        /// </summary>
        /// <param name="configuration">Configuración</param>
        public static void LoadCertificateFromConfiguration(ISSLConfiguration configuration)
        {
            if (string.IsNullOrEmpty(configuration.CertificatePath)) {
                Debug.WriteLine("No se ha establecido algun certificado en la configuración de SSL", VerbosityLevel.Warning);
                m_status = SSLCertificateStatus.InvalidPath;
            } else {
                if (File.Exists(configuration.CertificatePath)) {
                    bool usePassword = !string.IsNullOrEmpty(configuration.CertificatePassword);
                    if (usePassword) {
                        m_globalCert = new X509Certificate2(configuration.CertificatePath, configuration.CertificatePassword);
                    } else {
                        m_globalCert = new X509Certificate2(configuration.CertificatePath);
                    }
                    Debug.WriteLine("Establecido certificado global del sistema", VerbosityLevel.Info);
                    m_status = SSLCertificateStatus.Loaded;
                } else {
                    Debug.WriteLine($"No se puede cargar el certificado '{configuration.CertificatePath}' porque no existe", VerbosityLevel.Error);
                    m_status = SSLCertificateStatus.NotExists;
                }
            }
        }

        /// <summary>
        /// Obtener ruta a un certificado generado automaticamente
        /// </summary>
        /// <param name="address">Dirección IP utilizada</param>
        public static string GetAutogenCertFilepath(IPAddress address)
        {
            return DIRECTORY_CERTS + OpenSSL.FilenamePkcs12(address.ToString());
        }

        /// <summary>
        /// Generar ruta a un archivo del directorio de certificados
        /// </summary>
        /// <param name="filename">Nombre del archivo</param>
        /// <returns>Ruta generada</returns>
        public static string GetCert(string filename)
        {
            return DIRECTORY_CERTS + filename;
        }
    }
}
