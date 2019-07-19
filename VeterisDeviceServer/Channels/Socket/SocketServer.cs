using System;
using System.Security.Authentication;
using VeterisDeviceServer.Configuration;
using VeterisDeviceServer.Diagnostics;
using VeterisDeviceServer.Helper;
using WebSocketSharp;
using WebSocketSharp.Server;

namespace VeterisDeviceServer.Channels.Socket
{
    /// <summary>
    /// Servidor de conexiones socket
    /// </summary>
    internal class SocketServer
    {
        /// <summary>
        /// Al abrirse la conexión socket
        /// </summary>
        public event EventHandler<SocketConnection> OnSocketOpen;

        /// <summary>
        /// Al cerrarse la conexión socket
        /// </summary>
        public event EventHandler<SocketConnection> OnSocketClose;

        /// <summary>
        /// Servidor WebSocket interno
        /// </summary>
        private WebSocketServer m_webSocketServer;

        /// <summary>
        /// Inicializar servidor socket en un puerto determinado
        /// </summary>
        /// <param name="port">Puerto a utilizar</param>
        public SocketServer(int port = 555)
        {
            var config = ConfigurationManager.Get<ISocketAdapterConfiguration>();
            bool secure = Helper.SSL.Status == SSLCertificateStatus.Loaded && !config.OverrideSecure;
            m_webSocketServer = new WebSocketServer(port, secure);
            if (secure) {
                m_webSocketServer.SslConfiguration.EnabledSslProtocols = SslProtocols.Tls | SslProtocols.Tls11 | SslProtocols.Tls12;
                m_webSocketServer.SslConfiguration.ServerCertificate = Helper.SSL.GlobalCertificate;
                Debug.WriteLine(this, "Inicializando SocketServer con SSL", VerbosityLevel.Info);
            }

            m_webSocketServer.AddWebSocketService("/", () =>
            {
                var socketBehaviour = new SocketBehaviour();

                socketBehaviour.OnSocketOpen += (s, e) => OnSocketOpen?.Invoke(this, e);
                socketBehaviour.OnSocketClose += (s, e) => OnSocketClose?.Invoke(this, e);

                return socketBehaviour;
            });
            m_webSocketServer.Start();
        }

        /// <summary>
        /// Funcionamiento del servidor Socket
        /// </summary>
        private class SocketBehaviour : WebSocketBehavior
        {
            /// <summary>
            /// Al abrirse la conexión socket
            /// </summary>
            public event EventHandler<SocketConnection> OnSocketOpen;

            /// <summary>
            /// Al cerrarse la conexión socket
            /// </summary>
            public event EventHandler<SocketConnection> OnSocketClose;

            /// <summary>
            /// Datos de conexión socket propios
            /// </summary>
            private SocketConnection m_selfSocketConnection;

            /// <summary>
            /// Al abrirse la conexión socket
            /// </summary>
            protected override void OnOpen()
            {
                if (OnSocketOpen == null)
                    return;

                Debug.WriteLine(this, "Se establecio una nueva conexión socket: " + ID, VerbosityLevel.Default);

                SocketConnection socket = SelfSocketConnection();
                OnSocketOpen?.Invoke(this, socket);
            }

            /// <summary>
            /// Al cerrarse la conexión socket
            /// </summary>
            /// <param name="e">Información del cierre</param>
            protected override void OnClose(CloseEventArgs e)
            {
                if (OnSocketClose == null)
                    return;

                Debug.WriteLine(this, "Se desconecto correctamente una conexión socket: " + e.Reason, VerbosityLevel.Default);

                SocketConnection socket = SelfSocketConnection();
                OnSocketClose?.Invoke(this, socket);
            }

            /// <summary>
            /// Al ocurrir un error en la conexión socket
            /// </summary>
            /// <param name="e"></param>
            protected override void OnError(ErrorEventArgs e)
            {
                if (OnSocketClose == null)
                    return;

                Debug.WriteLine(this, "Ocurrio un error en una conexión socket: " + e.Message, VerbosityLevel.Warning);

                SocketConnection socket = SelfSocketConnection();
                OnSocketClose?.Invoke(this, socket);
            }

            /// <summary>
            /// Obtener socket connection propio
            /// </summary>
            /// <returns></returns>
            protected SocketConnection SelfSocketConnection()
            {
                if (m_selfSocketConnection == null)
                    m_selfSocketConnection = new SocketConnection(ID, Context.WebSocket);

                return m_selfSocketConnection;
            }
        }
    }

    /// <summary>
    /// Datos de conexión socket
    /// </summary>
    public class SocketConnection
    {
        /// <summary>
        /// Identificador de la conexión
        /// </summary>
        public string ConnectionId { get; }

        /// <summary>
        /// Conexión socket
        /// </summary>
        public WebSocket WebSocket { get; }

        /// <summary>
        /// Inicializar datos de conexión socket
        /// </summary>
        /// <param name="id">Identificador</param>
        /// <param name="sock">Conexión socket</param>
        public SocketConnection(string id, WebSocket sock)
        {
            ConnectionId = id;
            WebSocket = sock;
        }
    }
}
