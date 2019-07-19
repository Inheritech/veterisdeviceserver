using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;
using VeterisDeviceServer.Configuration;
using VeterisDeviceServer.Diagnostics;
using VeterisDeviceServer.Serialization;
using WatsonTcp;

namespace VeterisDeviceServer.Channels.Mesh
{
    /// <summary>
    /// Servidor de conexiones mesh TCP
    /// </summary>
    public class MeshTcpServer
    {
        /// <summary>
        /// Al desconectarse toda la malla
        /// </summary>
        public event EventHandler OnMeshKilled;

        /// <summary>
        /// Al recibir un mensaje
        /// </summary>
        public event EventHandler<string> OnMessage;

        /// <summary>
        /// Configuración de serialización que el servidor utiliza
        /// </summary>
        public static JsonSerializerSettings SerializationSettings { get; private set; }

        /// <summary>
        /// Servidor TCP de conexiones
        /// </summary>
        private WatsonTcpServer m_tcpServer;

        /// <summary>
        /// Desconectar otros clientes al conectarse uno nuevo
        /// </summary>
        private bool m_killOnConnect;

        /// <summary>
        /// IP/Puerto del cliente raíz conectado actualmente
        /// </summary>
        private string m_connectedRootNode;

        /// <summary>
        /// Cola de mensajes
        /// </summary>
        private Queue<string> m_msgQueue = new Queue<string>();

        /// <summary>
        /// Seguro de sincronización de mensajes
        /// </summary>
        private object m_msgLock = new object();

        /// <summary>
        /// Inicializar servidor TCP para la red malla
        /// </summary>
        /// <param name="port">Puerto para el servidor</param>
        /// <param name="killOnConnect">Definir si se debe desconectar a otros clientes al conectarse uno nuevo</param>
        public MeshTcpServer(int port, bool killOnConnect = false)
        {
            m_killOnConnect = killOnConnect;
            m_tcpServer = new WatsonTcpServer(
                "0.0.0.0",
                port,
                TcpServer_OnNodeConnected,
                TcpServer_OnNodeDisconnected,
                TcpServer_OnNodeMessage,
                true
            );
            if (SerializationSettings == null) {
                SerializationSettings = new JsonSerializerSettings
                {
                    ContractResolver = new TypeNameContractResolver()
                };
            }
        }

        /// <summary>
        /// Al conectarse un cliente TCP al servidor
        /// </summary>
        /// <param name="ipPort">IP:Puerto</param>
        private bool TcpServer_OnNodeConnected(string ipPort)
        {
            if (m_connectedRootNode != null) {
                if (m_killOnConnect) {
                    m_tcpServer.DisconnectClient(m_connectedRootNode);
                    m_connectedRootNode = null;
                } else {
                    if (!m_tcpServer.IsClientConnected(m_connectedRootNode)) {
                        m_connectedRootNode = null;
                    }
                }
            }
            if (m_connectedRootNode == null) {
                m_connectedRootNode = ipPort;
                Debug.WriteLine(this, "Se ha conectado un nodo raíz al servidor TCP para MDF: " + ipPort, VerbosityLevel.Info);
            } else {
                m_tcpServer.DisconnectClient(ipPort);
                Debug.WriteLine(this, "Se ha intentado conectar un nuevo nodo raíz al servidor TCP mientras que ya habia uno registrado, se ha desconectado la nueva conexión: " + ipPort, VerbosityLevel.Warning);
            }
            return true;
        }
        /// <summary>
        /// Al desconectarse un cliente TCP del servidor
        /// </summary>
        /// <param name="ipPort">IP:Puerto</param>
        private bool TcpServer_OnNodeDisconnected(string ipPort)
        {
            lock (m_msgLock) {
                if (m_connectedRootNode != ipPort)
                    return true;

                m_connectedRootNode = null;
                m_msgQueue.Clear();

                Debug.WriteLine(this, "Se ha desconectado el nodo raíz del servidor TCP: " + ipPort, VerbosityLevel.Info);

                OnMeshKilled?.Invoke(this, new EventArgs());
            }

            return true;
        }

        /// <summary>
        /// Al recibir un mensaje por TCP
        /// </summary>
        /// <param name="ipPort">IP:Puerto</param>
        /// <param name="data">Datos</param>
        private bool TcpServer_OnNodeMessage(string ipPort, byte[] data)
        {
            lock (m_msgQueue) {
                string message = Encoding.ASCII.GetString(data);
                Debug.WriteLine(this, "(1 / 2) Se recibio un mensaje TCP: " + message, VerbosityLevel.Debug);
                m_msgQueue.Enqueue(message);
            }

            lock (m_msgLock) {
                if (m_connectedRootNode == null)
                    return true;

                if (m_msgQueue.Count == 0)
                    return true;

                lock (m_msgQueue) {
                    while (m_msgQueue.Count > 0) {
                        string msg = m_msgQueue.Dequeue();

                        Debug.WriteLine(this, "(2 / 2) Se emitira un mensaje TCP: " + msg, VerbosityLevel.Debug);

                        OnMessage?.Invoke(this, msg);
                    }
                }
            }

            return true;
        }

        /// <summary>
        /// Escribir un objeto a la red mesh
        /// </summary>
        /// <param name="mac">Dirección MAC a la cual escribir</param>
        /// <param name="obj">Objeto a escribir</param>
        /// <param name="layer">Capa de la red mesh</param>
        public void Write<T>(string mac, T obj, int layer = 1)
        {
            if (m_connectedRootNode == null)
                return;

            var msg = new MeshMessage<T>(mac, obj, layer);

            string serialized = JsonConvert.SerializeObject(msg, SerializationSettings);

            Debug.WriteLine(this, "Escribiendo mensaje TCP: " + serialized, VerbosityLevel.Debug);

            m_tcpServer.Send(m_connectedRootNode, Encoding.ASCII.GetBytes(serialized), supressHeader: true);
        }
    }
}
