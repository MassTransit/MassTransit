namespace MassTransit
{
    public class SqlTransportMigrationOptions
    {
        /// <summary>
        /// If true, the database and all transport components will be created/updated on startup
        /// </summary>
        public bool CreateDatabase { get; set; }

        /// <summary>
        /// If true, the database and all transport components will be deleted on shutdown
        /// </summary>
        public bool DeleteDatabase { get; set; }
    }
}
