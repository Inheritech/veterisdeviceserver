using System;

namespace VeterisDeviceServer.Events
{
    /// <summary>
    /// Delegado de manejo de evento de modelo
    /// </summary>
    /// <typeparam name="T">Tipo del modelo</typeparam>
    /// <param name="sender">Emisor</param>
    /// <param name="args">Argumentos</param>
    public delegate void ModelEventHandler<T>(object sender, ModelEventArgs<T> args);

    /// <summary>
    /// Clase generica de argumentos de evento en caso de recibir un modelo JSON
    /// </summary>
    public class ModelEventArgs<T> : EventArgs
    {
        /// <summary>
        /// Modelo recibido
        /// </summary>
        public T Model { get; }

        /// <summary>
        /// Construir argumentos de evento de modelo
        /// </summary>
        /// <param name="model">Modelo recibido</param>
        public ModelEventArgs(T model)
        {
            Model = model;
        }
    }
}
