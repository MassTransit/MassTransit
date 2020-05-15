namespace MassTransit
{
    using System;
    using AmazonSqsTransport;


    public static class BusFactoryConfiguratorExtensions
    {
        /// <summary>
        /// Select AmazonSQS as the transport for the service bus
        /// </summary>
        public static IBusControl CreateUsingAmazonSqs(this IBusFactorySelector selector, Action<IAmazonSqsBusFactoryConfigurator> configure)
        {
            return AmazonSqsBusFactory.Create(configure);
        }
    }
}
