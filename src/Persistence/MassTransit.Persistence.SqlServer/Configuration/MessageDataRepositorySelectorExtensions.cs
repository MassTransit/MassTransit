namespace MassTransit.Persistence.SqlServer.Configuration
{
    using MassTransit.Configuration;
    using MessageData;
    using Microsoft.Data.SqlClient;
    
    public static class MessageDataRepositorySelectorExtensions
    {
        /// <summary>
        /// Configures a MessageData repository using SqlServer.  Requires
        /// setting the connection string via <paramref name="configure" />.
        /// </summary>
        public static IMessageDataRepository UsingSqlServer(this IMessageDataRepositorySelector selector,
            Action<ISqlServerMessageDataConfigurator> configure)
        {
            return UsingSqlServer(selector, string.Empty, configure);
        }

        /// <summary>
        /// Configures a MessageData repository using SqlServer.
        /// Builds the connection string from the parameters.
        /// </summary>
        public static IMessageDataRepository UsingSqlServer(this IMessageDataRepositorySelector selector,
            string hostname, string catalog, string username, string password,
            Action<ISqlServerMessageDataConfigurator>? configure = null)
        {
            var connectionStringBuilder = new SqlConnectionStringBuilder
            {
                DataSource = hostname,
                InitialCatalog = catalog,
                UserID = username,
                Password = password
            };

            return UsingSqlServer(selector, connectionStringBuilder.ToString());
        }

        /// <summary>
        /// Configures a MessageData repository using SqlServer.  Requires
        /// passing the connection string.
        /// </summary>
        public static IMessageDataRepository UsingSqlServer(this IMessageDataRepositorySelector selector,
            string connectionString,
            Action<ISqlServerMessageDataConfigurator>? configure = null)
        {
            var configurator = new SqlServerMessageDataConfigurator();

            configurator.SetConnectionString(connectionString);

            configure?.Invoke(configurator);

            configurator.Validate().ThrowIfContainsFailure("The Sql Server configuration is invalid:");

            return new SqlServerMessageDataRepository(
                configurator.ConnectionString!,
                configurator.TableName,
                configurator.IdColumnName,
                configurator.IsolationLevel,
#if NET8_0_OR_GREATER
                () => TimeProvider.System.GetUtcNow()
#else
                () => DateTimeOffset.UtcNow
#endif
            );
        }
    }
}
