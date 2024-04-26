namespace MassTransit
{
    using System;
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
    }
}
