using Newtonsoft.Json;
using System;
using System.Net;
using System.Threading.Tasks;
using Unosquare.Labs.EmbedIO;
using Unosquare.Labs.EmbedIO.Modules;

namespace VeterisDeviceServer.Extensions
{
    /// <summary>
    /// Extensiones de modulos de API Web
    /// </summary>
    public static class WebApiControllerExtensions
    {
        /// <summary>
        /// Convertir el cuerpo JSON de la solicitud en un modelo de datos
        /// </summary>
        /// <typeparam name="T">Tipo de modelo</typeparam>
        /// <param name="controller">Controlador</param>
        /// <param name="body">Referencia donde almacenar el contenido del cuerpo</param>
        /// <returns>null en caso de que la solicitud no sea JSON o no tenga la estructura correcta</returns>
        public static T RequestBodyToModel<T>(this WebApiController controller, out string body)
            where T : class
        {
            body = controller.RequestBody();
            try {
                if (controller.Request.ContentType != "application/json")
                    return null;

                return JsonConvert.DeserializeObject<T>(body);
            } catch (Exception) {
                return null;
            }
        }

        /// <summary>
        /// Enviar respuesta Json vacía con un código de estado
        /// </summary>
        /// <param name="controller"></param>
        /// <param name="code">Código de estado</param>
        /// <returns></returns>
        public static bool JsonResponse(this WebApiController controller, HttpStatusCode code)
        {
            controller.Response.StatusCode = (int)code;
            return controller.JsonResponse("");
        }

        /// <summary>
        /// Enviar respuesta Json vacía con un código de estado
        /// </summary>
        /// <param name="controller"></param>
        /// <param name="code">Código de estado</param>
        /// <returns></returns>
        public static async Task<bool> JsonResponseAsync(this WebApiController controller, HttpStatusCode code)
        {
            controller.Response.StatusCode = (int)code;
            return await controller.JsonResponseAsync("");
        }
    }
}
