using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System;
using System.Reflection;

namespace VeterisDeviceServer.Serialization
{
    /// <summary>
    /// Resolver personalizado para tomar en cuenta JsonTypeName al convertir
    /// </summary>
    public class TypeNameContractResolver : DefaultContractResolver
    {
        /// <summary>
        /// Aplicar JsonTypeName en las propiedades serializadas
        /// </summary>
        protected override JsonProperty CreateProperty(MemberInfo member, MemberSerialization memberSerialization)
        {
            JsonProperty property = base.CreateProperty(member, memberSerialization);

            JsonTypeNameAttribute attr = property.PropertyType.GetCustomAttribute<JsonTypeNameAttribute>();

            if (attr != null) {
                property.PropertyName = attr.TypeName;
            }

            return property;
        }
    }

    /// <summary>
    /// Atributo de nombre de tipo para JSON
    /// </summary>
    public class JsonTypeNameAttribute : Attribute
    {
        /// <summary>
        /// Nombre de tipo
        /// </summary>
        public string TypeName { get; }

        /// <summary>
        /// Decorar con un nombre de tipo
        /// </summary>
        /// <param name="typeName">Nombre de tipo</param>
        public JsonTypeNameAttribute(string typeName)
        {
            TypeName = typeName;
        }
    }
}
