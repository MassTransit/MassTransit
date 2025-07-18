namespace MassTransit.Persistence.PostgreSql.Configuration
{
    using System.Data;


    /// <summary>
    /// Configures a MessageData repository with Postgres.
    /// </summary>
    public interface IPostgresMessageDataConfigurator
    {
        /// <summary>
        /// Gets/sets the connection string.
        /// </summary>
        string? ConnectionString { get; set; }

        /// <summary>
        /// Gets/sets the table name.  Defaults to MessageData.
        /// </summary>
        string TableName { get; set; }

        /// <summary>
        /// Gets/sets the identity column name.  Defaults to Id.
        /// </summary>
        string IdColumnName { get; set; }

        /// <summary>
        /// Gets/sets the isolation level used during requests.
        /// </summary>
        IsolationLevel IsolationLevel { get; set; }

        /// <summary>
        /// Sets the connection string.
        /// </summary>
        IPostgresMessageDataConfigurator SetConnectionString(string connectionString);

        /// <summary>
        /// Sets the table name.
        /// </summary>
        IPostgresMessageDataConfigurator SetTableName(string tableName);

        /// <summary>
        /// Sets the identity column name.
        /// </summary>
        IPostgresMessageDataConfigurator SetIdColumnName(string idColumnName);

        /// <summary>
        /// Sets the isolation level.
        /// </summary>
        IPostgresMessageDataConfigurator SetIsolationLevel(IsolationLevel isolationLevel);
    }
}
