namespace MassTransit.Persistence.PostgreSql.Configuration
{
    using Npgsql;
    using Persistence.Configuration;


    public static class CustomJobSagaRepositoryConfiguratorExtensions
    {
        /// <summary>
        /// Configures the JobConsumer sagas to use Postgres.  Requires
        /// setting the connection string from the <paramref name="configure" />
        /// callback.
        /// </summary>
        public static ICustomJobSagaRepositoryConfigurator UsingPostgres(this ICustomJobSagaRepositoryConfigurator jobSagaConfigurator,
            Action<IPostgresJobSagaRepositoryConfigurator> configure)
        {
            return UsingPostgres(jobSagaConfigurator, string.Empty, configure);
        }

        /// <summary>
        /// Configures the JobConsumer sagas to use Postgres.
        /// Builds the connection string from the parameters.
        /// </summary>
        public static ICustomJobSagaRepositoryConfigurator UsingPostgres(this ICustomJobSagaRepositoryConfigurator jobSagaConfigurator,
            string hostname, string catalog, string username, string password,
            Action<IPostgresJobSagaRepositoryConfigurator>? configure = null)
        {
            var connectionStringBuilder = new NpgsqlConnectionStringBuilder
            {
                Host = hostname,
                Database = catalog,
                Username = username,
                Password = password
            };

            return UsingPostgres(jobSagaConfigurator, connectionStringBuilder.ToString(), configure);
        }

        /// <summary>
        /// Configures the JobConsumer sagas to use Postgres.
        /// Takes the connection string directly.
        /// </summary>
        public static ICustomJobSagaRepositoryConfigurator UsingPostgres(this ICustomJobSagaRepositoryConfigurator jobSagaConfigurator,
            string connectionString,
            Action<IPostgresJobSagaRepositoryConfigurator>? configure = null)
        {
            var repositoryConfigurator = new PostgresJobSagaRepositoryConfigurator();

            repositoryConfigurator.SetConnectionString(connectionString);

            configure?.Invoke(repositoryConfigurator);

            repositoryConfigurator.Validate().ThrowIfContainsFailure("The PostgreSql configuration is invalid:");
            repositoryConfigurator.Configure(jobSagaConfigurator);

            return jobSagaConfigurator;
        }
    }
}
