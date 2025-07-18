namespace MassTransit.Persistence.MySql.Configuration
{
    using System.Data;


    /// <summary>
    /// Configures a MessageData repository using MySql.
    /// </summary>
    public interface IMySqlMessageDataConfigurator
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
        IMySqlMessageDataConfigurator SetConnectionString(string connectionString);

        /// <summary>
        /// Sets the table name.
        /// </summary>
        IMySqlMessageDataConfigurator SetTableName(string tableName);

        /// <summary>
        /// Sets the identity column name.
        /// </summary>
        IMySqlMessageDataConfigurator SetIdColumnName(string idColumnName);

        /// <summary>
        /// Sets the isolation level.
        /// </summary>
        IMySqlMessageDataConfigurator SetIsolationLevel(IsolationLevel isolationLevel);
    }
}
