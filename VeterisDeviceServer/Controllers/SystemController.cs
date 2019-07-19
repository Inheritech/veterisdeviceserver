using System.Threading.Tasks;
using VeterisDeviceServer.Configuration;
using VeterisDeviceServer.Extensions;
using VeterisDeviceServer.Models.Users;
using Unosquare.Labs.EmbedIO;
using Unosquare.Labs.EmbedIO.Constants;
using Unosquare.Labs.EmbedIO.Modules;

namespace VeterisDeviceServer.Controllers
{
    /// <summary>
    /// Controlador de ping y configuraciones del sistema
    /// </summary>
    public class SystemController : WebApiController
    {
        /// <summary>
        /// Inicializar controlador de API de sistema
        /// </summary>
        /// <param name="context">Contexto HTTP del controlador</param>
        public SystemController(IHttpContext context) 
            : base(context)
        {
        }

        /// <summary>
        /// Solicitud de Ping al servidor
        /// </summary>
        [WebApiHandler(HttpVerbs.Get, "/api/ping")]
        public async Task<bool> Ping()
        {
            var credential = ConfigurationManager.Get<IServerCredential>();
            return await this.JsonResponseAsync(
                new PingResponse(credential != null ? credential.ServerId : "")
            );
        }
    }
}
