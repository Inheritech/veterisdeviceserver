using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace VeterisDeviceServer.Extensions
{
    /// <summary>
    /// Extensiones de string
    /// </summary>
    public static class StringExtensions
    {
        /// <summary>
        /// Convertir string JSON en un modelo
        /// </summary>
        /// <typeparam name="T">Tipo de modelo</typeparam>
        /// <param name="str">String JSON</param>
        /// <returns></returns>
        public static T ToJson<T>(this string str)
            where T : class
        {
            try {
                return JsonConvert.DeserializeObject<T>(str);
            } catch (Exception) {
                return null;
            }
        }
    }
}
