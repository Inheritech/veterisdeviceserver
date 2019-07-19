using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using VeterisDeviceServer.Database;
using VeterisDeviceServer.Database.Models;

using ApiServerUserAccess = Veteris.DomoticServer.Models.ServerUserAccess;

namespace VeterisDeviceServer.Repositories
{
    /// <summary>
    /// Repositorio de accesos para usuarios de servidor
    /// </summary>
    public class ServerUserAccessRepository : IDisposable
    {
        /// <summary>
        /// Contexto de base de datos de servidor de dispositivos
        /// </summary>
        private readonly DeviceServerContext m_context;

        /// <summary>
        /// Inicializar repositorio
        /// </summary>
        public ServerUserAccessRepository()
        {
            m_context = new DeviceServerContext();
        }

        /// <summary>
        /// Guardar acceso en base de datos
        /// </summary>
        /// <param name="access"></param>
        public void SaveAccess(ServerUserAccess access)
        {
            m_context.Add(access);
            m_context.SaveChanges();
        }

        /// <summary>
        /// Guardar accesos nuevos en formato de API
        /// </summary>
        /// <param name="accesses">Accesos</param>
        public void SaveAccesses(IEnumerable<ApiServerUserAccess> accesses)
        {
            var dbAccesses = accesses.Select(a => ServerUserAccess.FromApiModel(a));
            SaveAccesses(dbAccesses);
        }

        /// <summary>
        /// Obtener todos los accesos disponibles
        /// </summary>
        public List<ServerUserAccess> GetAccesses()
        {
            return m_context
                .ServerUserAccesses
                .ToList();
        }

        /// <summary>
        /// Guardar accesos nuevos
        /// </summary>
        /// <param name="accesses">Accesos</param>
        public void SaveAccesses(IEnumerable<ServerUserAccess> accesses)
        {
            var entityData = m_context.Model.FindEntityType(typeof(ServerUserAccess)).Relational();
            string tableName = entityData.TableName;

            string truncateStmt = "DELETE FROM " + tableName + ";";

#pragma warning disable EF1000 // Possible SQL injection vulnerability.
            m_context
                .Database
                .ExecuteSqlCommand(truncateStmt);
#pragma warning restore EF1000 // Possible SQL injection vulnerability.

            m_context
                .ServerUserAccesses
                .AddRange(accesses);

            m_context.SaveChanges();
        }

        /// <summary>
        /// Determinar si un acceso para un usuario es valido
        /// </summary>
        /// <param name="userId">Identificador del usuario</param>
        /// <param name="accessId">Identificador del acceso</param>
        public bool AccessIsValid(string userId, string accessId)
        {
            var access = m_context
                .ServerUserAccesses
                .SingleOrDefault(a => a.Identifier == accessId);

            return access != null && access.UserIdentifier == userId;
        }

        /// <summary>
        /// Obtener acceso en caso que exista y sea valido
        /// </summary>
        /// <param name="userId">Identificador de usuario</param>
        /// <param name="accessId">Identificador de acceso</param>
        public ServerUserAccess GetAccess(string userId, string accessId)
        {
            return m_context
                .ServerUserAccesses
                .SingleOrDefault(a => a.Identifier == accessId && a.UserIdentifier == userId);
        }

        /// <summary>
        /// Disponer de recursos
        /// </summary>
        ~ServerUserAccessRepository()
        {
            Dispose();
        }

        /// <summary>
        /// Disponer de los recursos
        /// </summary>
        public void Dispose()
        {
            m_context.Dispose();
        }
    }
}
