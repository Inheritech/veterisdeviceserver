using System;
using System.Security.Cryptography;

namespace VeterisDeviceServer.Security
{
    /// <summary>
    /// Clase de apoyo para generar tokens aleatorios
    /// </summary>
    public static class SecureRandom
    {
        /// <summary>
        /// Generar token aleatorio
        /// </summary>
        /// <param name="length">Longitud del token</param>
        public static string RandomToken(int length = 32)
        {
            string token;
            using (RandomNumberGenerator rng = new RNGCryptoServiceProvider()) {
                byte[] tokenData = new byte[length];
                rng.GetBytes(tokenData);

                token = Convert.ToBase64String(tokenData);
            }
            return token;
        }
    }
}
