namespace MassTransit.Persistence.SqlServer.Configuration
{
    using Microsoft.Data.SqlClient;
    using Persistence.Configuration;


    public static class CustomJobSagaRepositoryConfiguratorExtensions
    {
        /// <summary>
        /// Configures the JobConsumer sagas to use Sql Server.  Requires
        /// setting the connection string as part of the <paramref name="configure" />
        /// callback.
        /// </summary>
        public static ICustomJobSagaRepositoryConfigurator UsingSqlServer(this ICustomJobSagaRepositoryConfigurator jobSagaConfigurator,
            Action<ISqlServerJobSagaRepositoryConfigurator> configure)
        {
            return UsingSqlServer(jobSagaConfigurator, string.Empty, configure);
        }

        /// <summary>
        /// Configures the JobConsumer sagas to use Sql Server.  Builds
        /// the connection string from the provided parameters.
        /// </summary>
        public static ICustomJobSagaRepositoryConfigurator UsingSqlServer(this ICustomJobSagaRepositoryConfigurator jobSagaConfigurator,
            string hostname, string catalog, string username, string password,
            Action<ISqlServerJobSagaRepositoryConfigurator>? configure = null)
        {
            var connectionStringBuilder = new SqlConnectionStringBuilder
            {
                DataSource = hostname,
                InitialCatalog = catalog,
                UserID = username,
                Password = password
            };

            return UsingSqlServer(jobSagaConfigurator, connectionStringBuilder.ToString(), configure);
        }

        /// <summary>
        /// Configures the JobConsumer sagas to use Sql Server.  Takes the
        /// connection string directly.
        /// </summary>
        public static ICustomJobSagaRepositoryConfigurator UsingSqlServer(this ICustomJobSagaRepositoryConfigurator jobSagaConfigurator,
            string connectionString,
            Action<ISqlServerJobSagaRepositoryConfigurator>? configure = null)
        {
            var repositoryConfigurator = new SqlServerJobSagaRepositoryConfigurator();

            repositoryConfigurator.SetConnectionString(connectionString);

            configure?.Invoke(repositoryConfigurator);

            repositoryConfigurator.Validate().ThrowIfContainsFailure("The Sql Server configuration is invalid:");
            repositoryConfigurator.Configure(jobSagaConfigurator);

            return jobSagaConfigurator;
        }
    }
}
