using Newtonsoft.Json;
using System;
using VeterisDeviceServer.Diagnostics;
using WebSocketSharp;
using WebSocketSharp.Server;

namespace VeterisDeviceServer.Channels.Unity
{
    /// <summary>
    /// Servidor de comunicación con clientes de Unity
    /// </summary>
    internal sealed class UnityCommServer
    {
        /// <summary>
        /// Al recibir un mensaje del cliente
        /// </summary>
        public event EventHandler<string> OnMessage;

        /// <summary>
        /// Al destruirse el cliente
        /// </summary>
        public event EventHandler OnClientKilled;

        /// <summary>
        /// Cliente actualmente conectado
        /// </summary>
        public WebSocket CurrentUnityClient
        {
            get
            {
                return m_currentUnityClient;
            }
            private set
            {
                if (m_currentUnityClient != null) {
                    if (m_currentUnityClient.IsAlive && value != null)
                        return;

                    OnClientKilled?.Invoke(this, new EventArgs());
                    UnregisterClientEvents(m_currentUnityClient);
                }

                if (value != null) {
                    m_currentUnityClient = value;
                    RegisterClientEvents(value);
                }
            }
        }

        /// <summary>
        /// Servidor WebSocket interno
        /// </summary>
        private WebSocketServer m_webSocketServer;

        /// <summary>
        /// Conexión actual a un cliente de Unity
        /// </summary>
        private WebSocket m_currentUnityClient;

        /// <summary>
        /// Inicializar servidor de clientes de Unity
        /// </summary>
        /// <param name="port">Puerto a utilizar</param>
        public UnityCommServer(int port = 409)
        {
            m_webSocketServer = new WebSocketServer(port);
            m_webSocketServer.AddWebSocketService("/", () =>
            {
                var unityBehaviour = new UnityBehaviour();

                unityBehaviour.OnClientOpen += UnityBehaviour_OnClientOpen;
                unityBehaviour.OnClientClosed += UnityBehaviour_OnClientClosed;

                return unityBehaviour;
            });
            m_webSocketServer.Start();
        }

        /// <summary>
        /// Escribir un objeto al cliente Unity
        /// </summary>
        /// <typeparam name="T">Tipo del objeto enviar</typeparam>
        /// <param name="unityId">Identificador de Unity</param>
        /// <param name="obj">Objeto a escribir</param>
        public void Write<T>(string unityId, T obj)
        {
            if (m_currentUnityClient == null)
                return;

            var msg = new UnityMessage<T>(unityId, obj);

            string serialized = JsonConvert.SerializeObject(msg);

            Debug.WriteLine(this, "Escribiendo mensaje por el servidor de Unity: " + serialized, VerbosityLevel.Debug);

            m_currentUnityClient.Send(serialized);
        }

        /// <summary>
        /// Al abrirse una conexión WebSocket
        /// </summary>
        /// <param name="sender">Emisor</param>
        /// <param name="e">Conexión WebSocket</param>
        private void UnityBehaviour_OnClientOpen(object sender, WebSocket e)
        {
            CurrentUnityClient = e;
            Debug.WriteLine(this, "Se ha conectado un cliente Unity", VerbosityLevel.Info);
        }

        /// <summary>
        /// Al cerrarse una conexión WebSocket
        /// </summary>
        /// <param name="sender">Emisor</param>
        /// <param name="e">Conexión WebSocket cerrada</param>
        private void UnityBehaviour_OnClientClosed(object sender, WebSocket e)
        {
            if (CurrentUnityClient == e) {
                Debug.WriteLine(this, "El cliente Unity se ha desconectado", VerbosityLevel.Info);
                CurrentUnityClient = null;
            }
        }

        /// <summary>
        /// Registrar eventos de un cliente Unity
        /// </summary>
        /// <param name="ws">Conexión WebSocket</param>
        private void RegisterClientEvents(WebSocket ws)
        {
            ws.OnMessage += CurrentClient_OnMessage;
        }

        /// <summary>
        /// Deregistrar evnetos de un cliente Unity
        /// </summary>
        /// <param name="ws">Conexión WebSocket</param>
        private void UnregisterClientEvents(WebSocket ws)
        {
            ws.OnMessage -= CurrentClient_OnMessage;
        }

        /// <summary>
        /// Al recibir un mensaje del cliente actual
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CurrentClient_OnMessage(object sender, MessageEventArgs e)
        {
            if (!e.IsText) return;

            OnMessage?.Invoke(this, e.Data);
        }

        /// <summary>
        /// Funcionamiento del servidor para simulaciones de Unity
        /// </summary>
        private class UnityBehaviour : WebSocketBehavior
        {
            /// <summary>
            /// Al abrirse una conexión socket
            /// </summary>
            public event EventHandler<WebSocket> OnClientOpen;

            /// <summary>
            /// Al cerrarse una conexión socket
            /// </summary>
            public event EventHandler<WebSocket> OnClientClosed;

            /// <summary>
            /// Al abrirse una conexión WebSocket
            /// </summary>
            protected override void OnOpen()
            {
                if (OnClientOpen == null)
                    return;

                Debug.WriteLine(this, "Se establecio una nueva conexión Unity: " + ID, VerbosityLevel.Default);

                OnClientOpen?.Invoke(this, Context.WebSocket);
            }

            /// <summary>
            /// Al cerrarse la conexión WebSocket
            /// </summary>
            /// <param name="e">Información del cierre</param>
            protected override void OnClose(CloseEventArgs e)
            {
                if (OnClientClosed == null)
                    return;

                Debug.WriteLine(this, "Se desconecto correctamente una conexión socket: " + e.Reason, VerbosityLevel.Default);

                OnClientClosed?.Invoke(this, Context.WebSocket);
            }

            /// <summary>
            /// Al ocurrir un error en la conexión WebSocket
            /// </summary>
            /// <param name="e"></param>
            protected override void OnError(ErrorEventArgs e)
            {
                if (OnClientClosed == null)
                    return;

                Debug.WriteLine(this, "Ocurrio un error en una conexión socket: " + e.Message, VerbosityLevel.Warning);

                OnClientClosed?.Invoke(this, Context.WebSocket);
            }
        }
    }
}
