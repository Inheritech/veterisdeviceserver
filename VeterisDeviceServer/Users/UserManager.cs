using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using VeterisDeviceServer.Channels;
using VeterisDeviceServer.Diagnostics;

namespace VeterisDeviceServer.Users
{
    /// <summary>
    /// Gestor de usuarios
    /// </summary>
    public class UserManager
    {
        /// <summary>
        /// Instancia Singleton
        /// </summary>
        public static UserManager Instance { get; private set; }

        /// <summary>
        /// Al conectarse un usuario
        /// </summary>
        public event EventHandler<User> OnUserConnected;

        /// <summary>
        /// Al desconectarse un usuario
        /// </summary>
        public event EventHandler<User> OnUserDisconnected;

        /// <summary>
        /// Obtener usuarios actuales
        /// </summary>
        public ReadOnlyCollection<User> Users
        {
            get
            {
                return new ReadOnlyCollection<User>(m_users);
            }
        }

        /// <summary>
        /// Usuarios actuales
        /// </summary>
        private List<User> m_users;

        /// <summary>
        /// Gestores de canales registrados
        /// </summary>
        private List<IChannelManager> m_channelManagers;

        /// <summary>
        /// Construir gestor de usuarios
        /// </summary>
        public UserManager()
        {
            m_channelManagers = new List<IChannelManager>();
            m_users = new List<User>();

            if (Instance == null)
                Instance = this;
        }

        public void AddChannelManager<T>()
            where T : IChannelManager, new()
        {
            if (HasChannelManager<T>()) {
                Debug.WriteLine(this, "No se puede registrar un gestor de tipo " + typeof(T).Name + " porque ya hay uno registrado", VerbosityLevel.Warning);
                return;
            }

            IChannelManager manager = new T();

            manager.OnChannelConnected += Manager_OnChannelConnected;
            manager.OnChannelDisconnected += Manager_OnChannelDisconnected;

            m_channelManagers.Add(manager);
        }

        /// <summary>
        /// Al conectarse un nuevo canal
        /// </summary>
        /// <param name="sender">Emisor</param>
        /// <param name="e">Canal</param>
        private void Manager_OnChannelConnected(object sender, IChannel e)
        {
            lock (m_users) {
                User user = new User(e);
                m_users.Add(user);
                OnUserConnected?.Invoke(this, user);
            }
        }

        /// <summary>
        /// Al desconectarse un canal
        /// </summary>
        /// <param name="sender">Emisor</param>
        /// <param name="e">Canal</param>
        private void Manager_OnChannelDisconnected(object sender, IChannel e)
        {
            lock (m_users) {
                User[] users = m_users.Where(u => u.Channel == e).ToArray();
                foreach (User user in users) {
                    m_users.Remove(user);
                    OnUserDisconnected?.Invoke(this, user);
                }
            }
        }

        /// <summary>
        /// Determinar si un tipo de gestor de canales está registrado
        /// </summary>
        /// <typeparam name="T">Tipo del gestor</typeparam>
        public bool HasChannelManager<T>()
            where T : IChannelManager
        {
            return m_channelManagers.Any(cm => cm.GetType() == typeof(T));
        }

        /// <summary>
        /// Emitir objeto a todos los usuarios
        /// </summary>
        /// <typeparam name="T">Tipo del objeto</typeparam>
        /// <param name="obj">Objeto</param>
        public void Broadcast<T>(T obj)
        {
            lock (m_users) {
                foreach (User user in m_users) {
                    user.Write(obj);
                }
            }
        }
    }
}
