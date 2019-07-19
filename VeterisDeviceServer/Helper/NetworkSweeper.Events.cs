using System;
using System.Collections.Generic;
using System.Net;

namespace Veteris.Domotic.Interaction.Helper
{
    public partial class NetworkSweeper
    {

        /// <summary>
        /// Evento emitido cuando el barrido encontro un servidor
        /// </summary>
        public event SweepStatusUpdateEventHandler OnStatusUpdate;

        /// <summary>
        /// Emitir evento de IP revisada
        /// </summary>
        /// <param name="address">Dirección revisada</param>
        /// <param name="finished">Determina si ya ha terminado el barrido</param>
        /// <param name="found">Determina si se encontro el servidor</param>
        protected void RaiseStatusUpdateEvent(IPAddress address, bool finished, bool found)
        {
            OnStatusUpdate?.Invoke(this, new SweepStatusUpdateEventArgs(address, finished, found));
        }

    }

    #region EventHandlers

    /// <summary>
    /// Delegado de manejo sobre evento de servidor encontrado
    /// </summary>
    /// <param name="sender">Objeto que emite el evento</param>
    /// <param name="args">Argumentos del evento</param>
    public delegate void SweepStatusUpdateEventHandler(object sender, SweepStatusUpdateEventArgs args);

    #endregion

    #region EventArgs

    /// <summary>
    /// Argumentos de actualización de barrido de red
    /// </summary>
    public class SweepStatusUpdateEventArgs : EventArgs
    {

        public IPAddress CurrentNetworkAddress { get; }

        /// <summary>
        /// Determina si la IP fue encontrada
        /// </summary>
        public bool Found { get; }

        /// <summary>
        /// Determina si ya ha terminado el barrido
        /// </summary>
        public bool Finished { get; }

        /// <summary>
        /// Crear una nueva instancia de argumentos de evento de servidor no encontrado
        /// </summary>
        /// <param name="addresses"></param>
        public SweepStatusUpdateEventArgs(IPAddress current, bool finished, bool found)
        {
            CurrentNetworkAddress = current;
            Finished = finished;
            Found = found;
        }
    }

    #endregion

}
