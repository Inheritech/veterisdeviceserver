using Config.Net;
using System;
using System.Collections.Generic;
using System.IO;

namespace VeterisDeviceServer.Configuration
{
    /// <summary>
    /// Clase de soporte para manejo de configuración
    /// </summary>
    public class ConfigurationManager
    {
        /// <summary>
        /// Instancia del manejador de configuración
        /// </summary>
        public static ConfigurationManager Instance
        {
            get
            {
                if (_instance == null)
                    _instance = new ConfigurationManager();

                return _instance;
            }
        }

        /// <summary>
        /// Instancia del manejador de configuración
        /// </summary>
        private static ConfigurationManager _instance;

        /// <summary>
        /// Directorio base para configuraciones
        /// </summary>
        public const string BASE_DIRECTORY = "./Configuration";

        /// <summary>
        /// Extensión de archivos JSON utilizada
        /// </summary>
        public const string JSON_FILE_EXT = ".json";

        /// <summary>
        /// Constructor
        /// </summary>
        private ConfigurationManager()
        {
        }

        /// <summary>
        /// Registrar configuración en el manager
        /// </summary>
        /// <typeparam name="T">Tipo de la configuración</typeparam>
        public static void Register<T>()
            where T : class
        {
            Instance.RegisterType<T>();
        }

        /// <summary>
        /// Obtener configuración actual de determinado tipo
        /// </summary>
        /// <typeparam name="T">Tipo de configuración</typeparam>
        public static T Get<T>()
            where T : class
        {
            return Instance.GetCurrent<T>();
        }

        /// <summary>
        /// Registrar configuración en el manager
        /// </summary>
        /// <typeparam name="T">Tipo de la configuración</typeparam>
        public void RegisterType<T>()
            where T : class
        {
            Type t = typeof(T);

            string jsonFilePath = JsonFilePath<T>();

            if (!File.Exists(jsonFilePath)) {
                T setts = GetCurrent<T>();
                WriteDefault(setts);
            }
        }

        /// <summary>
        /// Obtener acceso a configuración de determinado tipo
        /// </summary>
        /// <typeparam name="T">Tipo de configuración</typeparam>
        public T GetCurrent<T>()
            where T : class
        {
            Type t = typeof(T);

            string jsonFilePath = JsonFilePath<T>();

            T settings = new ConfigurationBuilder<T>()
                .UseJsonFile(jsonFilePath)
                .Build();

            return settings;
        }

        /// <summary>
        /// Obtener la ruta a un archivo JSON de configuración
        /// según la ruta base y el tipo de estructura
        /// </summary>
        /// <typeparam name="T">Tipo de estructura</typeparam>
        /// <returns>Ruta al archivo</returns>
        public string JsonFilePath<T>()
        {
            string filename = typeof(T).Name + JSON_FILE_EXT;
            return Path.Join(BASE_DIRECTORY, filename);
        }

        /// <summary>
        /// Escribir configuración default para un tipo de configuración
        /// </summary>
        /// <typeparam name="T">Tipo de configuración</typeparam>
        /// <param name="def">Objeto por default</param>
        public void WriteDefault<T>(T def)
        {
            Type t = typeof(T);
            var props = t.GetProperties();
            foreach (var prop in props) {
                if (prop.CanWrite) {
                    var setter = prop.GetSetMethod();
                    setter.Invoke(def, new object[] { prop.GetValue(def) });
                }
            }
        }

    }
}
