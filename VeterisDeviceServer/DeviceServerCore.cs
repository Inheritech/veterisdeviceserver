using Microsoft.EntityFrameworkCore;
using Mono.Zeroconf;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Runtime.InteropServices;
using Veteris.Domotic.Interaction.Helper;
using Veteris.DomoticServer;
using Veteris.DomoticServer.Configuration;
using VeterisDeviceServer.Configuration;
using VeterisDeviceServer.Controllers;
using VeterisDeviceServer.Database;
using VeterisDeviceServer.Database.Models;
using VeterisDeviceServer.Devices;
using VeterisDeviceServer.Diagnostics;
using VeterisDeviceServer.Helper;
using VeterisDeviceServer.Repositories;
using VeterisDeviceServer.Routing;
using VeterisDeviceServer.Users;
using Unosquare.Labs.EmbedIO;
using Unosquare.Labs.EmbedIO.Modules;

namespace VeterisDeviceServer
{
    /// <summary>
    /// Nucleo del servidor de dispositivos
    /// </summary>
    public class DeviceServerCore
    {

        /// <summary>
        /// Puerto de API local ( Para manejo de configuración )
        /// </summary>
        public const int LOCAL_API_PORT = 218;

#if LOCAL_CLOUD
        public const string DOMOTIC_API_DOMAIN = "http://localhost:5000/api";
#else
        public const string DOMOTIC_API_DOMAIN = "https://domotic.Veteris.tech/api";
#endif

        /// <summary>
        /// Dominio para queries multicast mDNS
        /// </summary>
        public const string MULTICAST_DOMAIN = "Veterisdeviceserver";

        public const int ZEROCONF_PORT = 3669;

        /// <summary>
        /// Gestor de dispositivos
        /// </summary>
        internal DeviceManager DeviceManager { get; private set; }

        /// <summary>
        /// Gestor de usuarios
        /// </summary>
        internal UserManager UserManager { get; private set; }

        /// <summary>
        /// Servidor de API para configuración
        /// </summary>
        internal WebServer WebServer { get; private set; }

        /// <summary>
        /// Routers registrados
        /// </summary>
        private List<IRouter> m_routers;

        /// <summary>
        /// Servicio multicast
        /// </summary>
        private RegisterService _zeroconf;

        /// <summary>
        /// Constructor
        /// </summary>
        public DeviceServerCore()
        {
            InitDeviceManager();
            InitUserManager();
            InitConfiguration();
            InitDatabase();
            m_routers = new List<IRouter>();

            InitCloudData(DOMOTIC_API_DOMAIN);
        }

        /// <summary>
        /// Inicializar datos de la nube
        /// </summary>
        private void InitCloudData(string domain = DomoticCloud.BASE_DOMAIN)
        {
            var credential = ConfigurationManager.Get<IServerCredential>();
            if (credential != null && !string.IsNullOrEmpty(credential.CredentialId)) {
                Domotic.InitCloud(new ServerCredential
                {
                    CredentialId = credential.CredentialId,
                    CredentialSecret = credential.CredentialSecret
                }, domain);
            }
            try {
                using (var context = new DeviceServerContext()) {
                    context.ServerUserAccesses.RemoveRange(context.ServerUserAccesses.ToList());
                    var accesses = Domotic.Cloud.UserAccesses.GetServerAccesses();
                    if (accesses != null) {
                        Debug.WriteLine(this, $"Se obtuvieron {accesses.Count} accesos desde la nube", VerbosityLevel.Info);
                        using (ServerUserAccessRepository accessRepository = new ServerUserAccessRepository()) {
                            accessRepository.SaveAccesses(accesses);
                        }
                    } else {
                        Debug.WriteLine(this, "No se pudieron obtener los accesos de usuarios desde la nube", VerbosityLevel.Info);
                    }
                }
            } catch (Exception) {
                Debug.WriteLine(this, "No se pudieron obtener los accesos de usuarios desde la nube", VerbosityLevel.Error);
            }
        }

        /// <summary>
        /// Inicializar base de datos
        /// </summary>
        private void InitDatabase()
        {
            using (var context = new DeviceServerContext()) {
                context.Database.Migrate();
            }
        }

        /// <summary>
        /// Inicializar configuración
        /// </summary>
        private void InitConfiguration()
        {
            ConfigurationManager.Register<IServerCredential>();
            ConfigurationManager.Register<IDatabaseConfiguration>();
            ConfigurationManager.Register<IUnityConnection>();
            ConfigurationManager.Register<IMeshAdapterConfiguration>();
            ConfigurationManager.Register<IWebServerConfiguration>();
            ConfigurationManager.Register<ISSLConfiguration>();
            ConfigurationManager.Register<ISocketAdapterConfiguration>();
            ConfigurationManager.Register<ITcpSocketConfiguration>();
        }

        /// <summary>
        /// Inicializar gestor de dispositivos
        /// </summary>
        private void InitDeviceManager()
        {
            if (DeviceManager == null) {
                DeviceManager = new DeviceManager();
            }
        }

        /// <summary>
        /// Inicializar gestor de usuarios
        /// </summary>
        private void InitUserManager()
        {
            if (UserManager == null) {
                UserManager = new UserManager();
            }
        }

        /// <summary>
        /// Habilitar API local para configuración
        /// </summary>
        public DeviceServerCore EnableApi()
        {
            if (WebServer == null) {
                ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12 | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls;

                var configuration = ConfigurationManager.Get<IWebServerConfiguration>();
                var sslConfiguration = ConfigurationManager.Get<ISSLConfiguration>();

                WebServerOptions opts = ConfigureWebServer(configuration, sslConfiguration);
                WebServer = new WebServer(opts);

                WebServer.RegisterModule(new WebApiModule());
                WebServer.Module<WebApiModule>().RegisterController<SystemController>();

                if (configuration.EnableFilesModule) {
                    if (Directory.Exists(configuration.FilesModulePath)) {
                        Debug.WriteLine(this, "Habilitando modulo de archivos para el WebServer", VerbosityLevel.Debug);
                        WebServer.RegisterModule(new StaticFilesModule(configuration.FilesModulePath));
                    } else {
                        Debug.WriteLine(this, "La ruta al directorio de archivos estaticos no es correcta", VerbosityLevel.Debug);
                    }
                }

                WebServer.RunAsync();
            }
            return this;
        }

        /// <summary>
        /// Enviar baliza de dirección IP local
        /// </summary>
        public DeviceServerCore SendBeacon() {
            Debug.WriteLine(this, "Emitiendo baliza a la nube...", VerbosityLevel.Info);
            var result = Domotic.Cloud.Beacons.SendBeacon(NetworkUtilities.GetMainLocalIPAddress());
            if (result.IsSuccessful) {
                Debug.WriteLine(this, "Se establecio la baliza correctamente", VerbosityLevel.Info);
            } else {
                Debug.WriteLine(this, "Ocurrio un error al establecer la baliza: " + result.StatusCode, VerbosityLevel.Warning);
            }
            return this;
        }

        /// <summary>
        /// Habilitar emisión de servicio multicast para
        /// ser encontrado de forma inmediata por clientes
        /// mDNS
        /// </summary>
        public DeviceServerCore EnableMulticastBroadcast() {
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows)) {
                try {
                    _zeroconf = new RegisterService {
                        Name = "Veteris Device Server",
                        RegType = $"_{MULTICAST_DOMAIN}._tcp",
                        ReplyDomain = "local.",
                        Port = ZEROCONF_PORT
                    };

                    _zeroconf.Register();
                    Debug.WriteLine(this, "Broadcast de multicast iniciado para servicio: " + _zeroconf.RegType, VerbosityLevel.Info);
                } catch(Exception e) {
                    Debug.WriteLine(this, "No se pudo inicializar el servicio multicast: " + e.Message);
                }
            } else {
                Debug.WriteLine(this, "El broadcast de multicast local solo está soportado en Windows", VerbosityLevel.Warning);
            }
            return this;
        }

        /// <summary>
        /// Crear configuración para el servidor WebServer en base a la configuración provista
        /// </summary>
        /// <param name="configuration">Estructura de configuración</param>
        private WebServerOptions ConfigureWebServer(IWebServerConfiguration serverConfiguration, ISSLConfiguration configuration)
        {
            WebServerOptions opts;
            List<string> urlPrefixes = new List<string>
            {
                $"http://*:{serverConfiguration.HttpPort}/"
            };
            if (configuration.Secure && Helper.SSL.Status == SSLCertificateStatus.Loaded) {

                urlPrefixes.Add($"https://*:{serverConfiguration.HttpsPort}/");
                opts = new WebServerOptions(urlPrefixes.ToArray())
                {
                    Certificate = Helper.SSL.GlobalCertificate
                };
            } else {
                opts = new WebServerOptions(urlPrefixes.ToArray());
            }
            return opts;
        }

        /// <summary>
        /// Añadir un router de terminado tipo
        /// </summary>
        /// <typeparam name="T">Tipo del router</typeparam>
        public void AddRouter<T>()
            where T : IRouter, new()
        {
            if (HasRouter<T>()) {
                Debug.WriteLine(this, "No se puede añadir un router de tipo " + typeof(T).Name + " porque ya hay un router de ese tipo registrado", VerbosityLevel.Warning);
            }

            IRouter r = new T();
            r.Init(DeviceManager, UserManager);
            m_routers.Add(r);
            Debug.WriteLine(this, "Añadido router de tipo: " + typeof(T).Name, VerbosityLevel.Info);
        }

        /// <summary>
        /// Revisar si el nucleo contiene un Router del tipo determinado
        /// </summary>
        /// <typeparam name="T">Tipo del router</typeparam>
        public bool HasRouter<T>()
            where T : IRouter
        {
            return m_routers.Any(r => r.GetType() == typeof(T));
        }

    }
}
