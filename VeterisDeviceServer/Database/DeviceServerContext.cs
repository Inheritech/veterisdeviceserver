using Microsoft.EntityFrameworkCore;
using VeterisDeviceServer.Configuration;
using VeterisDeviceServer.Database.Models;

namespace VeterisDeviceServer.Database
{
    /// <summary>
    /// Contexto de base de datos
    /// </summary>
    public class DeviceServerContext : DbContext
    {
        /// <summary>
        /// Configuraciones serializadas de dispositivo
        /// </summary>
        public DbSet<SerializedDeviceConfiguration> SerializedDeviceConfigurations { get; set; }

        /// <summary>
        /// Traducciones serializadas de dispositivo
        /// </summary>
        public DbSet<SerializedDeviceTranslation> SerializedDeviceTranslations { get; set; }

        /// <summary>
        /// Accesos al servidor
        /// </summary>
        public DbSet<ServerUserAccess> ServerUserAccesses { get; set; }

        /// <summary>
        /// Al configurarse la base de datos
        /// </summary>
        /// <param name="optionsBuilder"></param>
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            base.OnConfiguring(optionsBuilder);

            IDatabaseConfiguration databaseConfiguration =  ConfigurationManager.Get<IDatabaseConfiguration>();
            optionsBuilder.UseSqlite($"Data Source={databaseConfiguration.DBFilePath}");
        }

    }
}
