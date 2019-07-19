using System;
using Veteris.DomoticServer.Models;
using VeterisDeviceServer.Channels;
using VeterisDeviceServer.Diagnostics;
using VeterisDeviceServer.Helper;
using VeterisDeviceServer.Models.Accessibility;
using VeterisDeviceServer.Models.IoT.Outbound;
using VeterisDeviceServer.Models.Users;
using VeterisDeviceServer.Repositories;

namespace VeterisDeviceServer.Users
{
    /// <summary>
    /// Usuario conectado al sistema
    /// </summary>
    public sealed class User
    {
        /// <summary>
        /// Evento en caso que el usuario pida una actualización de un dispositivo
        /// </summary>
        public event EventHandler<DeviceUpdate> OnRequestsUpdate;

        /// <summary>
        /// Evento en caso que el usuario realice una solicitud de datos
        /// </summary>
        public event EventHandler<UserDataRequest> OnDataRequest;

        /// <summary>
        /// Evento en caso que el usuario pida configurar un dispositivo
        /// </summary>
        public event EventHandler<DeviceConfiguration> OnConfigureDevice;

        /// <summary>
        /// Evento en caso que el usuario pida traducir un dispositivo
        /// </summary>
        public event EventHandler<DeviceTranslation> OnTranslateDevice;

        /// <summary>
        /// Canal de comunicación utilizado
        /// </summary>
        public IChannel Channel { get; }

        /// <summary>
        /// Acceso de este usuario
        /// </summary>
        public ServerUserAccess Access { get; private set; }

        /// <summary>
        /// Inicializar usuario conectado con un canal de comunicación
        /// </summary>
        /// <param name="channel">Canal</param>
        public User(IChannel channel)
        {
            Channel = channel;
            Channel.OnMessage += Channel_OnMessage;
        }

        /// <summary>
        /// Al recibir un mensaje por el canal
        /// </summary>
        /// <param name="sender">Emisor</param>
        /// <param name="e">Mensaje</param>
        private void Channel_OnMessage(object sender, string e)
        {
            if (Access == null) {
                Json.Parse<ServerUserAccess>(e, a =>
                {
                    using (ServerUserAccessRepository repo = new ServerUserAccessRepository()) {
                        Database.Models.ServerUserAccess savedAccess = repo.GetAccess(a.UserIdentifier, a.Identifier);
                        if (savedAccess != null) {
                            Access = a;
                            Debug.WriteLine(this, "Se autorizo a un usuario en el sistema", VerbosityLevel.Debug);
                        } else {
                            Debug.WriteLine(this, "No se pudo autorizar a un usuario en el sistema", VerbosityLevel.Debug);
                        }
                    }
                });
            }
            Json.Parse<DeviceUpdate>(e, (u) =>
            {
                OnRequestsUpdate?.Invoke(this, u);
            });
            Json.Parse<DeviceConfiguration>(e, (c) =>
            {
                OnConfigureDevice?.Invoke(this, c);
            });
            Json.Parse<DeviceTranslation>(e, (t) =>
            {
                OnTranslateDevice?.Invoke(this, t);
            });
            Json.Parse<UserDataRequest>(e, (r) =>
            {
                OnDataRequest?.Invoke(this, r);
            });
        }

        /// <summary>
        /// Escribir objeto al usuario
        /// </summary>
        /// <typeparam name="T">Tipo del objeto</typeparam>
        /// <param name="obj">Objeto</param>
        public void Write<T>(T obj)
        {
            if (Channel == null)
                return;

            Channel.Write(obj);
        }
    }
}
