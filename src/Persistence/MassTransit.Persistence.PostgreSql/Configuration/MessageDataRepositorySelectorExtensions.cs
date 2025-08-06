namespace MassTransit.Persistence.PostgreSql.Configuration
{
    using MassTransit.Configuration;
    using MessageData;
    using Npgsql;

    public static class MessageDataRepositorySelectorExtensions
    {
        /// <summary>
        /// Configures a MessageData repository using Postgres.  Requires
        /// setting the connection string via <paramref name="configure" />.
        /// </summary>
        public static IMessageDataRepository UsingPostgres(this IMessageDataRepositorySelector selector,
            Action<IPostgresMessageDataConfigurator> configure)
        {
            return UsingPostgres(selector, string.Empty, configure);
        }

        /// <summary>
        /// Configures a MessageData repository using Postgres.
        /// Builds the connection string from the parameters.
        /// </summary>
        public static IMessageDataRepository UsingPostgres(this IMessageDataRepositorySelector selector,
            string hostname, string catalog, string username, string password,
            Action<IPostgresMessageDataConfigurator>? configure = null)
        {
            var connectionStringBuilder = new NpgsqlConnectionStringBuilder
            {
                Host = hostname,
                Database = catalog,
                Username = username,
                Password = password
            };

            return UsingPostgres(selector, connectionStringBuilder.ToString());
        }

        /// <summary>
        /// Configures a MessageData repository using Postgres.  Requires
        /// passing the connection string.
        /// </summary>
        public static IMessageDataRepository UsingPostgres(this IMessageDataRepositorySelector selector,
            string connectionString,
            Action<IPostgresMessageDataConfigurator>? configure = null)
        {
            var configurator = new PostgresMessageDataConfigurator();

            configurator.SetConnectionString(connectionString);

            configure?.Invoke(configurator);

            configurator.Validate().ThrowIfContainsFailure("The Sql Server configuration is invalid:");

            return new PostgresMessageDataRepository(
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
