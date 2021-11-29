namespace MassTransit
{
    using System;
    using GrpcTransport.Configuration;


    public static class GrpcConfigurationExtensions
    {
        /// <summary>
        /// Configure and create a gRPC bus
        /// </summary>
        /// <param name="selector">Hang off the selector interface for visibility</param>
        /// <param name="configure">The configuration callback to configure the bus</param>
        /// <returns></returns>
        public static IBusControl CreateUsingGrpc(this IBusFactorySelector selector, Action<IGrpcBusFactoryConfigurator> configure)
        {
            return GrpcBus.Create(configure);
        }

        /// <summary>
        /// Configure and create a gRPC bus
        /// </summary>
        /// <param name="selector">Hang off the selector interface for visibility</param>
        /// <param name="baseAddress">Override the default base address</param>
        /// <param name="configure">The configuration callback to configure the bus</param>
        /// <returns></returns>
        public static IBusControl CreateUsingGrpc(this IBusFactorySelector selector, Uri baseAddress, Action<IGrpcBusFactoryConfigurator> configure)
        {
            return GrpcBus.Create(baseAddress, configure);
        }

        /// <summary>
        /// Configure MassTransit to use the gRPC transport
        /// </summary>
        /// <param name="configurator">The registration configurator (configured via AddMassTransit)</param>
        /// <param name="configure">The configuration callback for the bus factory</param>
        public static void UsingGrpc(this IBusRegistrationConfigurator configurator,
            Action<IBusRegistrationContext, IGrpcBusFactoryConfigurator> configure = null)
        {
            UsingGrpc(configurator, null, configure);
        }

        /// <summary>
        /// Configure MassTransit to use the gRPC transport
        /// </summary>
        /// <param name="configurator">The registration configurator (configured via AddMassTransit)</param>
        /// <param name="baseAddress">The base Address of the transport</param>
        /// <param name="configure">The configuration callback for the bus factory</param>
        public static void UsingGrpc(this IBusRegistrationConfigurator configurator, Uri baseAddress,
            Action<IBusRegistrationContext, IGrpcBusFactoryConfigurator> configure = null)
        {
            configurator.SetBusFactory(new GrpcRegistrationBusFactory(baseAddress, configure));
        }
    }
}
