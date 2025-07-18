namespace MassTransit.Persistence.MySql.Configuration
{
    using MassTransit.Configuration;
    using MessageData;
    using MySqlConnector;

    public static class MessageDataRepositorySelectorExtensions
    {
        /// <summary>
        /// Configures a MessageData repository using MySql.  Requires
        /// setting the connection string via <paramref name="configure" />.
        /// </summary>
        public static IMessageDataRepository UsingMySql(this IMessageDataRepositorySelector selector,
            Action<IMySqlMessageDataConfigurator> configure)
        {
            return UsingMySql(selector, string.Empty, configure);
        }

        /// <summary>
        /// Configures a MessageData repository using MySql.
        /// Builds the connection string from the parameters.
        /// </summary>
        public static IMessageDataRepository UsingMySql(this IMessageDataRepositorySelector selector,
            string hostname, string catalog, string username, string password,
            Action<IMySqlMessageDataConfigurator>? configure = null)
        {
            var connectionStringBuilder = new MySqlConnectionStringBuilder
            {
                Server = hostname,
                Database = catalog,
                UserID = username,
                Password = password
            };

            return UsingMySql(selector, connectionStringBuilder.ToString());
        }

        /// <summary>
        /// Configures a MessageData repository using MySql.  Requires
        /// passing the connection string.
        /// </summary>
        public static IMessageDataRepository UsingMySql(this IMessageDataRepositorySelector selector,
            string connectionString,
            Action<IMySqlMessageDataConfigurator>? configure = null)
        {
            var configurator = new MySqlMessageDataConfigurator();

            configurator.SetConnectionString(connectionString);

            configure?.Invoke(configurator);

            configurator.Validate().ThrowIfContainsFailure("The Sql Server configuration is invalid:");

            return new MySqlMessageDataRepository(
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
