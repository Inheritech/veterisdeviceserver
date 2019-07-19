using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using VeterisDeviceServer.Events;

namespace VeterisDeviceServer.Helper
{
    /// <summary>
    /// Clase de ayuda para ejecutar acciones en caso de lograr deserializar
    /// la estructura JSON especificada correctamente
    /// </summary>
    public static class Json
    {

        /// <summary>
        /// Realizar parsing de JSON y retornar default(T) en caso de error
        /// </summary>
        /// <typeparam name="T">Tipo de estructura a deserializar</typeparam>
        /// <param name="json">JSON a convertir</param>
        public static T Parse<T>(string json, JsonSerializerSettings settings = null)
        {
            try {
                T result = JsonConvert.DeserializeObject<T>(json, settings);
                return result;
            } catch (Exception e) {
                return default(T);
            }
        }

        /// <summary>
        /// Realizar parsing de JSON y ejecutar la acción provista con una estructura
        /// de argumentos de evento para modelos en caso de exito
        /// </summary>
        /// <typeparam name="T">Tipo de la estructura a deserializar</typeparam>
        /// <param name="json">JSON a convertir</param>
        /// <param name="action">Accion a ejecutar</param>
        public static void ParseEvent<T>(string json, Action<ModelEventArgs<T>> action, JsonSerializerSettings settings = null)
        {
            T model = Parse<T>(json, settings);
            if (!EqualityComparer<T>.Default.Equals(model, default(T))) {
                ModelEventArgs<T> args = new ModelEventArgs<T>(model);
                action(args);
            }
        }

        /// <summary>
        /// Realizar parsing de JSON y ejecutar la acción provista con el modelo creado
        /// </summary>
        /// <typeparam name="T">Tipo de la estructura a deserializar</typeparam>
        /// <param name="json">JSON a convertir</param>
        /// <param name="action">Accion a ejecutar</param>
        public static void Parse<T>(string json, Action<T> action, JsonSerializerSettings settings = null)
        {
            T model = Parse<T>(json, settings);
            if (!EqualityComparer<T>.Default.Equals(model, default(T))) {
                action(model);
            }
        }
    }
}
