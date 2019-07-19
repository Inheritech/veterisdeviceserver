using System;
using System.Collections.Generic;
using VeterisDeviceServer.Diagnostics;
using System.Net;
using System.Net.Http;
using System.Net.WebSockets;
using System.Threading;
using System.Threading.Tasks;
using Veteris.Domotic.Interaction.Helper;

namespace Veteris.Domotic.Interaction.Helper
{
    /// <summary>
    /// Clase de barrido de subred para encontrar servidor automaticamente en la red
    /// </summary>
    public partial class NetworkSweeper
    {

        /// <summary>
        /// Protocolos de red disponibles para el barrido
        /// </summary>
        public enum Protocol
        {
            HTTP,
            HTTPS,
            WS,
            WSS
        }

        public bool Running { get; private set; }

        protected List<IPAddress> _subNetwork;

        protected int _port;
        protected int _timeout;

        /// <summary>
        /// Crear un nuevo Network Sweeper que revisara toda la subred para encontrar
        /// dispositivos compatibles con el servidor
        /// </summary>
        /// <param name="port">Puerto de los dispositivos</param>
        /// <param name="upperBound">Numero maximo de direcciones por subred</param>
        /// <param name="fastStart">Determina si se debe calcular la dirección IP de inicio en base a la dirección local</param>
        public NetworkSweeper(int port, int timeout = 500, int upperBound = 255, bool fastStart = false)
        {
            _subNetwork = NetworkUtilities.GetSubNet(upperBound, fastStart);
            _port = port;
            _timeout = timeout;
        }

        /// <summary>
        /// Realizar barrido de red en el objeto de barrido actual
        /// </summary>
        /// <returns>Tarea de barrido</returns>
        public async Task Sweep()
        {
            if (Running)
                return;

            Running = true;

            foreach (IPAddress address in _subNetwork)
            {
                if (await CheckAddress(address, _timeout)) {
                    Debug.WriteLine("Found Address: " + address.ToString());
                    RaiseStatusUpdateEvent(address, finished: false, found: true);
                } else {
                    Debug.WriteLine("Checked Address: " + address.ToString());
                    RaiseStatusUpdateEvent(address, finished: false, found: false);
                }
            }

            RaiseStatusUpdateEvent(null, finished: true, found: false);
            Running = false;
        }

        /// <summary>
        /// Revisar si una dirección determinada es el servidor
        /// </summary>
        /// <param name="address">Dirección a revisar</param>
        /// <param name="timeout">Tiempo máximo de espera</param>
        /// <returns>Tarea asincrona de revision</returns>
        protected async Task<bool> CheckAddress(IPAddress address, int timeout = 500)
        {
            Uri webSocketUrl = GenerateUrl(address);
            using (WebSocketTester webSocket = new WebSocketTester(webSocketUrl)) {
                return await webSocket.TestConnection(timeout);
            }
        }

        /// <summary>
        /// Generar URL a websocket
        /// </summary>
        /// <param name="address">Dirección IP sobre la cual generar la URL</param>
        /// <returns>URL al recurso de acuerdo a la IP provista</returns>
        protected Uri GenerateUrl(IPAddress address)
        {
            return new Uri($"ws://{address}:{_port}/");
        }

        /// <summary>
        /// Clase para probar conexiones WebSocket
        /// </summary>
        private class WebSocketTester : IDisposable
        {

            private ClientWebSocket m_webSocket;
            private CancellationTokenSource m_cancellationTokenSource;

            private readonly Uri m_webSocketUrl;

            public WebSocketTester(Uri webSocketUrl)
            {
                m_webSocketUrl = webSocketUrl;
                m_webSocket = new ClientWebSocket();
            }

            /// <summary>
            /// Probar conexión WebSocket
            /// </summary>
            /// <param name="timeout">Tiempo máximo de espera por conexión</param>
            /// <returns></returns>
            public async Task<bool> TestConnection(int timeout = 500)
            {
                try {
                    m_cancellationTokenSource = new CancellationTokenSource(timeout);
                    await m_webSocket.ConnectAsync(m_webSocketUrl, m_cancellationTokenSource.Token);
                    return m_webSocket.State == WebSocketState.Open;
                } catch (Exception) {
                    return false;
                }
            }

            public void Dispose()
            {
                m_webSocket.Dispose();
                m_cancellationTokenSource.Dispose();
            }
        }

    }
}
