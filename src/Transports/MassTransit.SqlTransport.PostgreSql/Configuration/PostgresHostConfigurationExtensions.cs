namespace MassTransit
{
    using System;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Options;
    using SqlTransport.PostgreSql;


    public static class PostgresHostConfigurationExtensions
    {
        /// <summary>
        /// Configures the database transport to use PostgreSQL as the storage engine
        /// </summary>
        /// <param name="configurator"></param>
        /// <param name="hostAddress">The MassTransit host address of the database</param>
        /// <param name="configure"></param>
        public static void UsePostgres(this ISqlBusFactoryConfigurator configurator, Uri hostAddress, Action<ISqlHostConfigurator>? configure = null)
        {
            var hostConfigurator = new PostgresSqlHostConfigurator(hostAddress);

            configure?.Invoke(hostConfigurator);

            configurator.Host(hostConfigurator.Settings);
        }

        /// <summary>
        /// Configures the database transport to use PostgreSQL as the storage engine
        /// </summary>
        /// <param name="configurator"></param>
        /// <param name="context">The bus registration context, used to retrieve the DbTransportOptions</param>
        /// <param name="configure"></param>
        public static void UsePostgres(this ISqlBusFactoryConfigurator configurator, IBusRegistrationContext context, Action<ISqlHostConfigurator>? configure = null)
        {
            var hostConfigurator = new PostgresSqlHostConfigurator(context.GetRequiredService<IOptions<SqlTransportOptions>>().Value);

            configure?.Invoke(hostConfigurator);

            configurator.Host(hostConfigurator.Settings);
        }
    }
}