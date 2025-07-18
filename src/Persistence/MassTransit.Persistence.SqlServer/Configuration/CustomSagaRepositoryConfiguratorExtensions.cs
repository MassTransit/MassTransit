namespace MassTransit.Persistence.SqlServer.Configuration
{
    using Microsoft.Data.SqlClient;
    using Persistence.Configuration;


    public static class CustomSagaRepositoryConfiguratorExtensions
    {
        /// <summary>
        /// Configures a Sql Server-based saga repository for this <typeparamref name="TSaga" />.
        /// Requires setting the connection string as part of the <paramref name="configure" />
        /// callback.
        /// </summary>
        public static ICustomRepositoryConfigurator<TSaga> UsingSqlServer<TSaga>(this ICustomRepositoryConfigurator<TSaga> sagaConfigurator,
            Action<ISqlServerRepositoryConfigurator<TSaga>> configure)
            where TSaga : class, ISaga
        {
            return UsingSqlServer(sagaConfigurator, string.Empty, configure);
        }

        /// <summary>
        /// Configures a Sql Server-based saga repository for this <typeparamref name="TSaga" />.
        /// Builds the connection string from provided parameters.
        /// </summary>
        public static ICustomRepositoryConfigurator<TSaga> UsingSqlServer<TSaga>(this ICustomRepositoryConfigurator<TSaga> sagaConfigurator,
            string hostname, string catalog, string username, string password,
            Action<ISqlServerRepositoryConfigurator<TSaga>>? configure = null)
            where TSaga : class, ISaga
        {
            var connectionStringBuilder = new SqlConnectionStringBuilder
            {
                DataSource = hostname,
                InitialCatalog = catalog,
                UserID = username,
                Password = password
            };

            return UsingSqlServer(sagaConfigurator, connectionStringBuilder.ToString(), configure);
        }

        /// <summary>
        /// Configures a Sql Server-based saga repository for this <typeparamref name="TSaga" />.
        /// Takes the connection string directly.
        /// </summary>
        public static ICustomRepositoryConfigurator<TSaga> UsingSqlServer<TSaga>(this ICustomRepositoryConfigurator<TSaga> sagaConfigurator,
            string connectionString,
            Action<ISqlServerRepositoryConfigurator<TSaga>>? configure = null)
            where TSaga : class, ISaga
        {
            var repositoryConfigurator = new SqlServerRepositoryConfigurator<TSaga>();

            repositoryConfigurator.SetConnectionString(connectionString);

            configure?.Invoke(repositoryConfigurator);

            repositoryConfigurator.Validate().ThrowIfContainsFailure("The Sql Server configuration is invalid:");
            repositoryConfigurator.Configure(sagaConfigurator);

            return sagaConfigurator;
        }
    }
}
