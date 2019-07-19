using Newtonsoft.Json;
using PusherClient;
using System;
using Veteris.DomoticServer;
using VeterisDeviceServer.Database.Models;

namespace VeterisDeviceServer.Channels.Cloud
{
    /// <summary>
    /// Canal de comunicación Pusher
    /// </summary>
    internal sealed class CloudChannel : IChannel
    {
        /// <summary>
        /// Al recibir un mensaje
        /// </summary>
        public event EventHandler<string> OnMessage;

        /// <summary>
        /// Canal de pusher vinculado
        /// </summary>
        private Channel _pusherChannel;

        /// <summary>
        /// Acceso vinculado
        /// </summary>
        private ServerUserAccess _bindedAccess;

        /// <summary>
        /// Inicializar canal de nube
        /// </summary>
        /// <param name="access">Acceso</param>
        /// <param name="pusher">Instancia de Pusher</param>
        public CloudChannel(ServerUserAccess access, Channel pusherChannel)
        {
            _bindedAccess = access;
            _pusherChannel = pusherChannel;
            _pusherChannel.Bind(access.UserIdentifier, (dynamic data) =>
            {
                OnMessage?.Invoke(this, (string)data);
            });
        }

        /// <summary>
        /// Escribir al canal de la nube
        /// </summary>
        /// <typeparam name="T">Tipo de mensaje</typeparam>
        /// <param name="obj">Mensaje</param>
        public void Write<T>(T obj)
        {
            string json = JsonConvert.SerializeObject(obj);
            Domotic.Cloud.Pusher.SendMessage(_bindedAccess.UserIdentifier, json);
        }

        /// <summary>
        /// Disponer de recursos
        /// </summary>
        public void Dispose()
        {
            _pusherChannel.Unbind(_bindedAccess.UserIdentifier);
        }
    }
}
