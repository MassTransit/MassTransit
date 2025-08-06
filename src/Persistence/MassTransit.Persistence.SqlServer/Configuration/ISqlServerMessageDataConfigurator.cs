namespace MassTransit.Persistence.SqlServer.Configuration
{
    using System.Data;


    /// <summary>
    /// Configures a MessageData repository with SqlServer.
    /// </summary>
    public interface ISqlServerMessageDataConfigurator
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
        ISqlServerMessageDataConfigurator SetConnectionString(string connectionString);

        /// <summary>
        /// Sets the table name.
        /// </summary>
        ISqlServerMessageDataConfigurator SetTableName(string tableName);

        /// <summary>
        /// Sets the identity column name.
        /// </summary>
        ISqlServerMessageDataConfigurator SetIdColumnName(string idColumnName);

        /// <summary>
        /// Sets the isolation level.
        /// </summary>
        ISqlServerMessageDataConfigurator SetIsolationLevel(IsolationLevel isolationLevel);
    }
}
