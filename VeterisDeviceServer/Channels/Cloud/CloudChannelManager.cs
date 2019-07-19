using Newtonsoft.Json;
using PusherClient;
using System;
using System.Collections.Generic;
using Veteris.DomoticServer;
using VeterisDeviceServer.Configuration;
using VeterisDeviceServer.Database.Models;
using VeterisDeviceServer.Diagnostics;
using VeterisDeviceServer.Helper;
using VeterisDeviceServer.Repositories;

namespace VeterisDeviceServer.Channels.Cloud
{
    /// <summary>
    /// Gestor de canales en la nube
    /// </summary>
    public class CloudChannelManager : IChannelManager
    {

#if LOCAL_CLOUD
        public const string PUSHER_APP_KEY = "efac99e9c43266bdf67b";
#else
        public const string PUSHER_APP_KEY = "ad68e02a12a18702b1cc";
#endif

        /// <summary>
        /// Al conectarse un canal mesh
        /// </summary>
        public event EventHandler<IChannel> OnChannelConnected;

        /// <summary>
        /// Al desconectarse un canal mesh
        /// </summary>
        public event EventHandler<IChannel> OnChannelDisconnected;

        /// <summary>
        /// Canales conectados actualmente
        /// </summary>
        private Dictionary<string, IChannel> m_channels;

        /// <summary>
        /// Acceso a Pusher
        /// </summary>
        private Pusher _pusher;

        /// <summary>
        /// Canal de pusher utilizado
        /// </summary>
        private Channel _pusherChannel;

        /// <summary>
        /// Inicializar gestor
        /// </summary>
        public CloudChannelManager()
        {
            m_channels = new Dictionary<string, IChannel>();
            _pusher = new Pusher(PUSHER_APP_KEY, new PusherOptions
            {
                Authorizer = new PusherServerAuthorizer(Domotic.Cloud.CurrentDomain + "/pusher/authServer"),
                Cluster = "us2",
                Encrypted = true
            });
            _pusher.Connected += Pusher_Connected;
            _pusher.Error += Pusher_Error;
            _pusher.ConnectionStateChanged += Pusher_ConnectionStateChanged;
            _pusher.Connect();
        }

        /// <summary>
        /// Al conectarse Pusher
        /// </summary>
        /// <param name="sender"></param>
        private void Pusher_Connected(object sender)
        {
            var credential = ConfigurationManager.Get<IServerCredential>();
            if (credential == null) return;
            try {
                string channelName = "private-server-control-" + credential.ServerId;
                if (_pusher.Channels.ContainsKey(channelName)) {
                    _pusherChannel = _pusher.Channels[channelName];
                    return;
                } else {
                    _pusherChannel = _pusher.Subscribe(channelName);
                }
            } catch (Exception e) {
                Debug.WriteLine(this, "No se pudo iniciar el gestor de canales Cloud debido a un error de Pusher:", VerbosityLevel.Error);
                Debug.WriteLine(this, e.Message, VerbosityLevel.Error);
                return;
            }
            _pusherChannel.Bind("userAdd", (dynamic data) =>
            {
                Debug.WriteLine(this, "Recibido evento Pusher de adición de usuario " + JsonConvert.SerializeObject(data), VerbosityLevel.Debug);
                Json.Parse<ServerUserAccess>((string)data, s =>
                {
                    Debug.WriteLine(this, "Acceso recibido desde mensaje Pusher", VerbosityLevel.Debug);
                    using (ServerUserAccessRepository suaRepo = new ServerUserAccessRepository()) {
                        suaRepo.SaveAccess(s);
                    }
                });
            });
            using (ServerUserAccessRepository suaRepo = new ServerUserAccessRepository()) {
                List<ServerUserAccess> accesses = suaRepo.GetAccesses();
                foreach (ServerUserAccess access in accesses) {
                    AddChannel(access);
                }
            }
            Debug.WriteLine(this, "Gestor de canales Cloud inicializado", VerbosityLevel.Info);
        }

        private void Pusher_Error(object sender, PusherException error)
        {
            Debug.WriteLine(this, "Error de Pusher: " + error.ToString(), VerbosityLevel.Debug);
        }

        private void Pusher_ConnectionStateChanged(object sender, ConnectionState state)
        {
            Debug.WriteLine(this, "Estado de Pusher: " + state.ToString(), VerbosityLevel.Debug);
        }

        /// <summary>
        /// Añadir canal en base a acceso
        /// </summary>
        /// <param name="access">Acceso</param>
        private void AddChannel(ServerUserAccess access)
        {
            if (m_channels.ContainsKey(access.UserIdentifier)) return;

            CloudChannel channel = new CloudChannel(access, _pusherChannel);
            m_channels.Add(access.UserIdentifier, channel);
            OnChannelConnected?.Invoke(this, channel);
            Debug.WriteLine(this, "Canal por acceso añadido", VerbosityLevel.Debug);
        }

    }
}
