namespace MassTransit
{
    using System;
    using Npgsql;
    using SqlTransport.Configuration;


    public static class PostgresBusFactoryConfiguratorExtensions
    {
        /// <summary>
        /// Configure the bus to use the PostgreSQL database transport
        /// </summary>
        /// <param name="configurator">The registration configurator (configured via AddMassTransit)</param>
        /// <param name="configure">The configuration callback for the bus factory</param>
        public static void UsingPostgres(this IBusRegistrationConfigurator configurator,
            Action<IBusRegistrationContext, ISqlBusFactoryConfigurator>? configure = null)
        {
            configurator.SetBusFactory(new SqlRegistrationBusFactory((context, cfg) =>
            {
                cfg.UsePostgres(context);

                configure?.Invoke(context, cfg);
            }));
        }

        /// <summary>
        /// Configure the bus to use the PostgreSQL database transport
        /// </summary>
        /// <param name="configurator">The registration configurator (configured via AddMassTransit)</param>
        /// <param name="connectionString">
        /// Connection string to be used/parsed by the transport. <see cref="SqlTransportOptions" /> are not
        /// used with this overload
        /// </param>
        /// <param name="configure">The configuration callback for the bus factory</param>
        public static void UsingPostgres(this IBusRegistrationConfigurator configurator, string connectionString,
            Action<IBusRegistrationContext, ISqlBusFactoryConfigurator>? configure = null)
        {
            configurator.SetBusFactory(new SqlRegistrationBusFactory((context, cfg) =>
            {
                cfg.UsePostgres(connectionString);

                configure?.Invoke(context, cfg);
            }));
        }

        /// <summary>
        /// Configure the bus to use the PostgreSQL database transport
        /// </summary>
        /// <param name="configurator">The registration configurator (configured via AddMassTransit)</param>
        /// <param name="dataSource">
        /// The dataSource used by the transport <see cref="SqlTransportOptions" /> are not
        /// used with this overload
        /// </param>
        /// <param name="configure">The configuration callback for the bus factory</param>
        /// <remarks>The dataSource is also only used for application level credentials when running
        /// sqlTransport with MassTransit and not quite used to run migrations which require admin
        /// level access on the operating database </remarks>
        public static void UsingPostgres(this IBusRegistrationConfigurator configurator, NpgsqlDataSource dataSource,
            Action<IBusRegistrationContext, ISqlBusFactoryConfigurator>? configure = null)
        {
            configurator.SetBusFactory(new SqlRegistrationBusFactory((context, cfg) =>
            {
                cfg.UsePostgres(dataSource);

                configure?.Invoke(context, cfg);
            }));
        }
    }
}
