namespace MassTransit.Persistence.PostgreSql.Configuration
{
    using Npgsql;
    using Persistence.Configuration;


    public static class CustomSagaRepositoryConfiguratorExtensions
    {
        /// <summary>
        /// Configures a Postgres-based saga repository for this <typeparamref name="TSaga" />.
        /// Requires setting the connection string as part of the <paramref name="configure" />
        /// callback.
        /// </summary>
        public static ICustomRepositoryConfigurator<TSaga> UsingPostgres<TSaga>(this ICustomRepositoryConfigurator<TSaga> sagaConfigurator,
            Action<IPostgresRepositoryConfigurator<TSaga>> configure)
            where TSaga : class, ISaga
        {
            return UsingPostgres(sagaConfigurator, string.Empty, configure);
        }

        /// <summary>
        /// Configures a Postgres-based saga repository for this <typeparamref name="TSaga" />.
        /// Builds the connection string from the parameters.
        /// </summary>
        public static ICustomRepositoryConfigurator<TSaga> UsingPostgres<TSaga>(this ICustomRepositoryConfigurator<TSaga> sagaConfigurator,
            string hostname, string catalog, string username, string password,
            Action<IPostgresRepositoryConfigurator<TSaga>>? configure = null)
            where TSaga : class, ISaga
        {
            var connectionStringBuilder = new NpgsqlConnectionStringBuilder
            {
                Host = hostname,
                Database = catalog,
                Username = username,
                Password = password
            };

            return UsingPostgres(sagaConfigurator, connectionStringBuilder.ToString(), configure);
        }

        /// <summary>
        /// Configures a Postgres-based saga repository for this <typeparamref name="TSaga" />.
        /// Takes the connection string directly.
        /// </summary>
        public static ICustomRepositoryConfigurator<TSaga> UsingPostgres<TSaga>(this ICustomRepositoryConfigurator<TSaga> sagaConfigurator,
            string connectionString,
            Action<IPostgresRepositoryConfigurator<TSaga>>? configure = null)
            where TSaga : class, ISaga
        {
            var repositoryConfigurator = new PostgresRepositoryConfigurator<TSaga>();

            repositoryConfigurator.SetConnectionString(connectionString);

            configure?.Invoke(repositoryConfigurator);

            repositoryConfigurator.Validate().ThrowIfContainsFailure("The PostgreSql configuration is invalid:");
            repositoryConfigurator.Configure(sagaConfigurator);

            return sagaConfigurator;
        }
    }
}
