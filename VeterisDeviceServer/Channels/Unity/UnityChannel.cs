using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Text;
using VeterisDeviceServer.Diagnostics;
using VeterisDeviceServer.Helper;

namespace VeterisDeviceServer.Channels.Unity
{
    /// <summary>
    /// Canal de comunicación de Unity
    /// </summary>
    internal sealed class UnityChannel : IChannel
    {
        /// <summary>
        /// Al recibir un mensaje
        /// </summary>
        public event EventHandler<string> OnMessage;

        /// <summary>
        /// Identificador de Unity
        /// </summary>
        public string UnityId { get; }

        /// <summary>
        /// Servidor de comunicaciones de Unity
        /// </summary>
        private readonly UnityCommServer m_unityCommServer;

        /// <summary>
        /// Crear nuevo canal
        /// </summary>
        /// <param name="unityId">Identificador de Unity</param>
        /// <param name="commServer">Servidor de comunicaciones a utilizar</param>
        public UnityChannel(string unityId, UnityCommServer commServer)
        {
            UnityId = unityId;
            m_unityCommServer = commServer;
            m_unityCommServer.OnMessage += UnityCommServer_OnMessage;
        }

        /// <summary>
        /// Al recibir un mensaje desde el servidor Unity
        /// </summary>
        /// <param name="sender">Emisor</param>
        /// <param name="e">Mensaje</param>
        private void UnityCommServer_OnMessage(object sender, string e)
        {
            UnityMessage message = Json.Parse<UnityMessage>(e);
            if (message == null) return;

            if (message.UnityId != this.UnityId) return;
            JObject jObj = JObject.Parse(e);

            if (!jObj.ContainsKey("data")) return;
            JToken data = jObj["data"];

            if (!data.HasValues || data.First.Path == "connection_event") return;

            string msg = data.ToString();

            Debug.WriteLine(this, "Procesando mensaje Unity: " + msg, VerbosityLevel.Debug);

            OnMessage?.Invoke(this, msg);
        }

        /// <summary>
        /// Escribir al canal
        /// </summary>
        /// <typeparam name="T">Tipo de objeto a escribir</typeparam>
        /// <param name="obj">Objeto a escribir</param>
        public void Write<T>(T obj)
        {
            if (m_unityCommServer == null) {
                Debug.WriteLine(this, "Servidor Unity no establecido, no se pudo escribir el mensaje", VerbosityLevel.Warning);
                return;
            }

            m_unityCommServer.Write(UnityId, obj);
        }

        /// <summary>
        /// Destructor
        /// </summary>
        ~UnityChannel()
        {
            Dispose();
        }

        /// <summary>
        /// Disponer de los recursos
        /// </summary>
        public void Dispose()
        {
            m_unityCommServer.OnMessage -= UnityCommServer_OnMessage;
            Debug.WriteLine(this, "Canal destruido", VerbosityLevel.Debug);
        }
    }
}
