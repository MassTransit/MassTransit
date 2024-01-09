#nullable enable
namespace MassTransit
{
    using System;
    using SqlTransport.Configuration;


    public static class SqlBusFactoryConfiguratorExtensions
    {
        /// <summary>
        /// Create a bus using the database transport
        /// </summary>
        public static IBusControl CreateUsingDb(this IBusFactorySelector selector, Action<ISqlBusFactoryConfigurator> configure)
        {
            return SqlBusFactory.Create(configure);
        }

        /// <summary>
        /// Configure the bus to use the database transport
        /// </summary>
        /// <param name="configurator">The registration configurator (configured via AddMassTransit)</param>
        /// <param name="configure">The configuration callback for the bus factory</param>
        public static void UsingDb(this IBusRegistrationConfigurator configurator,
            Action<IBusRegistrationContext, ISqlBusFactoryConfigurator>? configure = null)
        {
            configurator.SetBusFactory(new SqlRegistrationBusFactory(configure));
        }
    }
}
