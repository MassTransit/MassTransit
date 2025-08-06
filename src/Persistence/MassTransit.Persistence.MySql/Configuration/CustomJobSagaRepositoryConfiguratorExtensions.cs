namespace MassTransit.Persistence.MySql.Configuration
{
    using MySqlConnector;
    using Persistence.Configuration;


    public static class CustomJobSagaRepositoryConfiguratorExtensions
    {
        /// <summary>
        /// Configures the JobConsumer sagas to use MySql.  Requires
        /// setting the connection string from the <paramref name="configure" />
        /// callback.
        /// </summary>
        public static ICustomJobSagaRepositoryConfigurator UsingMySql(this ICustomJobSagaRepositoryConfigurator jobSagaConfigurator,
            Action<IMySqlJobSagaRepositoryConfigurator> configure)
        {
            return UsingMySql(jobSagaConfigurator, string.Empty, configure);
        }

        /// <summary>
        /// Configures the JobConsumer sagas to use MySql.
        /// Builds the connection string from the parameters.
        /// </summary>
        public static ICustomJobSagaRepositoryConfigurator UsingMySql(this ICustomJobSagaRepositoryConfigurator jobSagaConfigurator,
            string hostname, string catalog, string username, string password,
            Action<IMySqlJobSagaRepositoryConfigurator>? configure = null)
        {
            var connectionStringBuilder = new MySqlConnectionStringBuilder
            {
                Server = hostname,
                Database = catalog,
                UserID = username,
                Password = password
            };

            return UsingMySql(jobSagaConfigurator, connectionStringBuilder.ToString(), configure);
        }

        /// <summary>
        /// Configures the JobConsumer sagas to use MySql.
        /// Takes the connection string directly.
        /// </summary>
        public static ICustomJobSagaRepositoryConfigurator UsingMySql(this ICustomJobSagaRepositoryConfigurator jobSagaConfigurator,
            string connectionString,
            Action<IMySqlJobSagaRepositoryConfigurator>? configure = null)
        {
            var repositoryConfigurator = new MySqlJobSagaRepositoryConfigurator();

            repositoryConfigurator.SetConnectionString(connectionString);

            configure?.Invoke(repositoryConfigurator);

            repositoryConfigurator.Validate().ThrowIfContainsFailure("The MySql configuration is invalid:");
            repositoryConfigurator.Configure(jobSagaConfigurator);

            return jobSagaConfigurator;
        }
    }
}
