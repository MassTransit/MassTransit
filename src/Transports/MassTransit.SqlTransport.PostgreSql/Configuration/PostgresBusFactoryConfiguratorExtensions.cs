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
    }
}
