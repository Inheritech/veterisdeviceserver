using System;
using VeterisDeviceServer.Channels;
using VeterisDeviceServer.Diagnostics;
using VeterisDeviceServer.Helper;
using VeterisDeviceServer.Models.IoT.Inbound;
using VeterisDeviceServer.Models.IoT.Outbound;

namespace VeterisDeviceServer.Devices
{
    /// <summary>
    /// Dispositivo conectado al sistema
    /// </summary>
    public sealed class Device
    {
        /// <summary>
        /// Al establecerse la identidad del dispositivo
        /// </summary>
        public event EventHandler<DeviceIdentity> OnIdentity;

        /// <summary>
        /// Al establecerse el estado del dispositivo
        /// </summary>
        public event EventHandler<DeviceStatus> OnStatus;

        /// <summary>
        /// En caso que el dispositivo emita un evento
        /// </summary>
        public event EventHandler<DeviceEvent> OnEvent;

        /// <summary>
        /// Identidad del dispositivo
        /// </summary>
        public DeviceIdentity Identity
        {
            get
            {
                return m_identity;
            }
            private set
            {
                if (value == null)
                    return;

                if (m_identity != null)
                    return;

                m_identity = value;
                OnIdentity?.Invoke(this, m_identity);
                Debug.WriteLine(this, "Establecida identidad del dispositivo " + m_identity.SerialNumber, VerbosityLevel.Debug);

                DeviceRequest request = DeviceRequest.Status;
                request.SerialNumber = m_identity.SerialNumber;
                Channel.Write(request);
                Debug.WriteLine(this, "Enviada solicitud de estado al dispositivo " + m_identity.SerialNumber, VerbosityLevel.Debug);
            }
        }

        /// <summary>
        /// Estado del dispositivo
        /// </summary>
        public DeviceStatus Status
        {
            get
            {
                return m_status;
            }
            private set
            {
                if (value == null)
                    return;

                if (m_identity == null) {
                    Debug.WriteLine(this, "No se puede establecer el estado de un dispositivo sin identidad", VerbosityLevel.Warning);
                    return;
                }

                m_status = value;
                OnStatus?.Invoke(this, m_status);
                Debug.WriteLine(this, "Establecido estado del dispositivo" + m_status.SerialNumber, VerbosityLevel.Debug);
            }
        }

        /// <summary>
        /// Canal del dispositivo
        /// </summary>
        public IChannel Channel { get; }

        /// <summary>
        /// Private: Identidad del dispositivo
        /// </summary>
        private DeviceIdentity m_identity;

        /// <summary>
        /// Private: Estado del dispositivo
        /// </summary>
        private DeviceStatus m_status;

        /// <summary>
        /// Construir nuevo dispositivo
        /// </summary>
        /// <param name="channel">Canal de comunicaciones a utilizar</param>
        public Device(IChannel channel)
        {
            Channel = channel;
            Channel.OnMessage += Channel_OnMessage;
        }

        /// <summary>
        /// Revisar la integridad de la conexión al dispositivo
        /// </summary>
        public void CheckIntegrity()
        {
            Debug.WriteLine(this, "Revisando la integridad de dispositivo", VerbosityLevel.Debug);
            if (m_identity == null) {
                Channel.Write(DeviceRequest.Identity);
                Debug.WriteLine(this, "Solicitud de identidad enviada como revisión de integridad", VerbosityLevel.Debug);
                return;
            }
            if (m_status == null) {
                DeviceRequest request = DeviceRequest.Status;
                request.SerialNumber = m_identity.SerialNumber;
                Channel.Write(request);
                Debug.WriteLine(this, "Solicitud de estado enviada como revisión de integridad", VerbosityLevel.Debug);
                return;
            }
        }

        /// <summary>
        /// Al recibir un mensaje por el canal
        /// </summary>
        /// <param name="sender">Emisor</param>
        /// <param name="msg">Mensaje</param>
        private void Channel_OnMessage(object sender, string msg)
        {
            Json.Parse<DeviceIdentity>(msg, (i) =>
            {
                Identity = i;
            });
            Json.Parse<DeviceStatus>(msg, (s) =>
            {
                Status = s;
            });
            Json.Parse<DeviceEvent>(msg, (e) =>
            {
                OnEvent?.Invoke(this, e);
            });
            Json.Parse<DeviceIntegrityRequest>(msg, (e) =>
            {
                if (e.IntegrityEvent == IntegrityEvent.Check) {
                    CheckIntegrity();
                }
            });
        }

        /// <summary>
        /// Disponer de los recursos del dispositivo y canal
        /// </summary>
        ~Device()
        {
            if (Channel != null)
                Channel.Dispose();
        }
    }
}
