namespace MassTransit.QuartzService.Configuration
{
    /// <summary>
    /// A connection string source used to build out connection strings
    /// </summary>
    public interface IConnectionStringProvider
    {
        /// <summary>
        /// Returns the requested connection string, optionally replacing the server or catalog
        /// </summary>
        /// <param name="connectionName">The name of the connection string</param>
        /// <param name="serverName">Optional, the server name to use instead of the name in the setting source</param>
        /// <param name="databaseName">Optional, the catalog name to use instead of the name in the setting source</param>
        /// <returns>A properly formatted connection string</returns>
        string GetConnectionString(string connectionName, string serverName = null, string databaseName = null);
        
    }
}