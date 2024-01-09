namespace MassTransit
{
    using System;
    using SqlTransport.Configuration;


    public static class SqlServerBusFactoryConfiguratorExtensions
    {
        /// <summary>
        /// Configure the bus to use the SQL Server transport
        /// </summary>
        /// <param name="configurator">The registration configurator (configured via AddMassTransit)</param>
        /// <param name="configure">The configuration callback for the bus factory</param>
        public static void UsingSqlServer(this IBusRegistrationConfigurator configurator,
            Action<IBusRegistrationContext, ISqlBusFactoryConfigurator>? configure = null)
        {
            configurator.SetBusFactory(new SqlRegistrationBusFactory((context, cfg) =>
            {
                cfg.UseSqlServer(context);

                configure?.Invoke(context, cfg);
            }));
        }
    }
}
