using Newtonsoft.Json;
using PusherClient;
using System;
using System.Net;
using System.Text;
using Veteris.DomoticServer;

namespace VeterisDeviceServer.Channels.Cloud
{
    /// <summary>
    /// Autorizador de Pusher con autorización
    /// </summary>
    public class PusherServerAuthorizer : IAuthorizer
    {
        private Uri _authEndpoint;

        private string _accessPayload;

        public PusherServerAuthorizer(string authEndpoint)
        {
            _authEndpoint = new Uri(authEndpoint);
            string credentialJson = JsonConvert.SerializeObject(Domotic.Cloud.Credentials);
            byte[] credentialBytes = Encoding.ASCII.GetBytes(credentialJson);
            _accessPayload = Convert.ToBase64String(credentialBytes);
        }

        public string Authorize(string channelName, string socketId)
        {
            string authToken = null;

            using (var webClient = new WebClient()) {
                string data = string.Format("channel_name={0}&socket_id={1}", channelName, socketId);
                webClient.Headers[HttpRequestHeader.ContentType] = "application/x-www-form-urlencoded";
                webClient.Headers[HttpRequestHeader.Authorization] = _accessPayload;
                try {
                    authToken = webClient.UploadString(_authEndpoint, "POST", data);
                } catch (Exception) {
                    return null;
                }
            }

            return authToken;
        }
    }
}
