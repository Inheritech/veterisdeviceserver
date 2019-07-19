using System;
using VeterisDeviceServer.Channels.Mesh;
using VeterisDeviceServer.Channels.Socket;
using VeterisDeviceServer.Channels.Unity;
using VeterisDeviceServer.Routing;
using System.Threading;
using VeterisDeviceServer.Channels.TcpSocket;
using VeterisDeviceServer.Channels.Cloud;

namespace VeterisDeviceServer
{
    class Program
    {
        /// <summary>
        /// Nucleo del servidor de dispositivos
        /// </summary>
        public static DeviceServerCore Core { get; private set; }

        /// <summary>
        /// Iniciar servidor
        /// </summary>
        /// <param name="args"></param>
        static void Main(string[] args)
        {
            Core = new DeviceServerCore();

            Core.EnableApi()
                .EnableMulticastBroadcast()
                .SendBeacon();

            Core.DeviceManager.AddChannelManager<MeshChannelManager>();
            Core.DeviceManager.AddChannelManager<UnityChannelManager>();
            Core.UserManager.AddChannelManager<SocketChannelManager>();
            Core.UserManager.AddChannelManager<TcpSocketChannelManager>();
            Core.UserManager.AddChannelManager<CloudChannelManager>();

            Core.AddRouter<UserUpdateRouter>();
            Core.AddRouter<UserConfigRouter>();
            Core.AddRouter<UserTranslateRouter>();
            Core.AddRouter<UserRequestRouter>();
            Core.AddRouter<DeviceStatusRouter>();
            Core.AddRouter<DeviceEventRouter>();

            Console.WriteLine("Servidor de dispositivos iniciado!");

            Thread.Sleep(Timeout.Infinite);
        }
    }
}
