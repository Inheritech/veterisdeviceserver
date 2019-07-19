using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;

namespace Veteris.Domotic.Interaction.Helper
{

    /// <summary>
    /// Utilidades de red
    /// </summary>
    class NetworkUtilities
    {
        /// <summary>
        /// Bytes de la dirección IP loopback
        /// </summary>
        public static readonly byte[] LoopbackIPBytes = new byte[] { 127, 0, 0, 1 };

        /// <summary>
        /// Generar un rango de direcciones IP de subred en base a la IP local
        /// </summary>
        /// <param name="upperBound">Valor máximo de la IP</param>
        /// <param name="fastStart">Determina si se debe calcular la IP de inicio en base a la IP local</param>
        /// <returns>Lista de direcciones IP en base a la subred</returns>
        public static List<IPAddress> GetSubNet(int upperBound = 255, bool fastStart = false)
        {
            List<IPAddress> resultList = new List<IPAddress>();
            List<IPAddress> localIPs = GetLocalIPAddresses();
            foreach (IPAddress address in localIPs) {
                byte[] ipBytes = address.GetAddressBytes();
                int startAddress = 1;
                if (fastStart) {
                    float cent = Convert.ToInt32(ipBytes[3]);
                    cent /= 100f;
                    cent = (int)Math.Floor(cent);
                    startAddress = (int)cent * 100;
                }
                for (int i = startAddress; i < upperBound; i++) {
                    ipBytes[3] = Convert.ToByte(i);
                    resultList.Add(new IPAddress(ipBytes));
                }
            }

            return resultList;
        }

        /// <summary>
        /// Determinar si una dirección IP está en la red local
        /// </summary>
        /// <param name="address">Dirección a determinar</param>
        /// <returns></returns>
        public static bool AddressIsInLocalNet(IPAddress address)
        {
            byte[] addressBytes = address.GetAddressBytes();

            if (addressBytes.SequenceEqual(LoopbackIPBytes))
                return true;

            List<IPAddress> localIPAddresses = GetLocalIPAddresses();
            foreach (IPAddress localAddress in localIPAddresses) {
                byte[] localAddressBytes = localAddress.GetAddressBytes();
                if (AddressBytesAreInNet(addressBytes, localAddressBytes)) {
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// <para>Comparar los bytes de dos direcciones IP para saber si estan en la misma red</para>
        /// <para>Adicionalmente se puede utilizar el parametro full para saber si son identicas</para>
        /// </summary>
        /// <param name="lho">Dirección izquierda</param>
        /// <param name="rho">Dirección derecha</param>
        /// <param name="full">Determina si se debe revisar la dirección completa</param>
        /// <returns></returns>
        public static bool AddressBytesAreInNet(byte[] lho, byte[] rho, bool full = false)
        {
            if (lho.Length != 4 || rho.Length != 4)
                throw new ArgumentOutOfRangeException();

            for (int i = 0; i < 3; i++) {
                if (lho[i] != rho[i]) {
                    return false;
                }
            }
            return true;
        }

        /// <summary>
        /// Obtener la dirección IP local o retornar null
        /// </summary>
        /// <returns>Objeto de dirección IP</returns>
        public static List<IPAddress> GetLocalIPAddresses()
        {
            List<IPAddress> addresses = new List<IPAddress>();

            NetworkInterface[] networkInterfaces = NetworkInterface.GetAllNetworkInterfaces();
            foreach (NetworkInterface network in networkInterfaces) {
                IPInterfaceProperties properties = network.GetIPProperties();

                foreach (IPAddressInformation address in properties.UnicastAddresses) {

                    if (address.Address.AddressFamily != AddressFamily.InterNetwork)
                        continue;

                    if (IPAddress.IsLoopback(address.Address))
                        continue;

                    addresses.Add(address.Address.MapToIPv4());
                }
            }

            return addresses;
        }

        /// <summary>
        /// Tratar de obtener la dirección IP principal si alguna dirección empieza con "192", sino,
        /// retornar la primer dirección registrada
        /// </summary>
        /// <returns>null en caso que no haya direcciones que obtener</returns>
        public static IPAddress GetMainLocalIPAddress()
        {
            List<IPAddress> addresses = GetLocalIPAddresses();
            List<string> addressesStr = addresses.Select(a => a.ToString()).ToList();

            if (addresses.Count == 0) return null;

            // Revisar por direcciones que inicien con 192.168.0 y 192.168.1
            string mainAddrStr = addressesStr.FirstOrDefault(astr => astr.StartsWith("192.168.0") || astr.StartsWith("192.168.1"));

            if (mainAddrStr != null) {
                return IPAddress.Parse(mainAddrStr);
            }
            
            // Revisar por direcciones que inicien con 192.168
            mainAddrStr = addressesStr.FirstOrDefault(astr => astr.StartsWith("192.168"));

            if (mainAddrStr != null) {
                return IPAddress.Parse(mainAddrStr);
            }

            // Revisar por direcciones que inicien con 192
            mainAddrStr = addressesStr.FirstOrDefault(astr => astr.StartsWith("192"));

            if (mainAddrStr != null) {
                return IPAddress.Parse(mainAddrStr);
            }

            return addresses[0];
        }
    }
}
