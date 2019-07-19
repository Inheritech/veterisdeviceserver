using Newtonsoft.Json;
using System.Collections.Generic;

namespace VeterisDeviceServer.Models.Accessibility
{
    /// <summary>
    /// Estructura de traducción
    /// </summary>
    public sealed class DeviceTranslation
    {
        /// <summary>
        /// Numero de serie del dispositivo relacionado
        /// </summary>
        [JsonProperty("serial", Required = Required.Always)]
        public string SerialNumber { get; set; }

        /// <summary>
        /// Traducción del nombre
        /// </summary>
        [JsonProperty("name_translation", Required = Required.Always)]
        public string NameTranslation { get; set; }

        /// <summary>
        /// Traducciones de las propiedades
        /// </summary>
        [JsonProperty("props_translation", Required = Required.Always)]
        public Dictionary<string, string> PropertiesTranslation { get; set; }
    }
}
