namespace MassTransit
{
    using System;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Options;
    using SqlTransport.SqlServer;


    public static class SqlServerHostConfigurationExtensions
    {
        /// <summary>
        /// Configures the database transport to use SQL Server as the storage engine
        /// </summary>
        /// <param name="configurator"></param>
        /// <param name="hostAddress">The MassTransit host address of the database</param>
        /// <param name="configure"></param>
        public static void UseSqlServer(this ISqlBusFactoryConfigurator configurator, Uri hostAddress, Action<ISqlHostConfigurator>? configure = null)
        {
            var hostConfigurator = new SqlServerSqlHostConfigurator(hostAddress);

            configure?.Invoke(hostConfigurator);

            configurator.Host(hostConfigurator.Settings);
        }

        /// <summary>
        /// Configures the database transport to use SQL Server as the storage engine
        /// </summary>
        /// <param name="configurator"></param>
        /// <param name="context">The bus registration context, used to retrieve the DbTransportOptions</param>
        /// <param name="configure"></param>
        public static void UseSqlServer(this ISqlBusFactoryConfigurator configurator, IBusRegistrationContext context,
            Action<ISqlHostConfigurator>? configure = null)
        {
            var hostConfigurator = new SqlServerSqlHostConfigurator(context.GetRequiredService<IOptions<SqlTransportOptions>>().Value);

            configure?.Invoke(hostConfigurator);

            configurator.Host(hostConfigurator.Settings);
        }

        /// <summary>
        /// Configures the database transport to use SQL Server as the storage engine
        /// </summary>
        /// <param name="configurator"></param>
        /// <param name="connectionString">A valid SQL Server connection string</param>
        /// <param name="configure"></param>
        public static void UseSqlServer(this ISqlBusFactoryConfigurator configurator, string connectionString, Action<ISqlHostConfigurator>? configure = null)
        {
            var hostConfigurator = new SqlServerSqlHostConfigurator(connectionString);

            configure?.Invoke(hostConfigurator);

            configurator.Host(hostConfigurator.Settings);
        }
    }
}
