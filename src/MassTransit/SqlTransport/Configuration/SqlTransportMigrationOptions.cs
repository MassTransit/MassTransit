namespace MassTransit
{
    public class SqlTransportMigrationOptions
    {
        /// <summary>
        /// If true, the database and all transport components will be created/updated on startup
        /// </summary>
        public bool CreateDatabase { get; set; }

        /// <summary>
        /// If true, the schema for transport components will be created/updated on startup
        ///
        /// Use this, without CreateDatabase, if you do not have the required permissions to create the schema and grant access
        /// </summary>
        public bool CreateSchema { get; set; }

        /// <summary>
        /// If true, the infrastructure components for the transport will be created/updated on startup
        ///
        /// Use this, without CreateDatabase, if you do not have the required permissions to create databases and logins
        /// </summary>
        public bool CreateInfrastructure { get; set; }

        /// <summary>
        /// If true, the database and all transport components will be deleted on shutdown
        /// </summary>
        public bool DeleteDatabase { get; set; }
    }
}
