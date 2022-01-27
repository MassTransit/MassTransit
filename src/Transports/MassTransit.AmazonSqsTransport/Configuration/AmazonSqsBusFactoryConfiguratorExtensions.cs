namespace MassTransit
{
    using System;
    using Amazon.SimpleNotificationService;
    using Amazon.SQS;
    using AmazonSqsTransport.Configuration;


    public static class AmazonSqsBusFactoryConfiguratorExtensions
    {
        /// <summary>
        /// Select AmazonSQS as the transport for the service bus
        /// </summary>
        public static IBusControl CreateUsingAmazonSqs(this IBusFactorySelector selector, Action<IAmazonSqsBusFactoryConfigurator> configure)
        {
            return AmazonSqsBusFactory.Create(configure);
        }

        /// <summary>
        /// Configure MassTransit to use Amazon SQS for the transport.
        /// </summary>
        /// <param name="configurator">The registration configurator (configured via AddMassTransit)</param>
        /// <param name="configure">The configuration callback for the bus factory</param>
        public static void UsingAmazonSqs(this IBusRegistrationConfigurator configurator,
            Action<IBusRegistrationContext, IAmazonSqsBusFactoryConfigurator> configure = null)
        {
            configurator.SetBusFactory(new AmazonSqsRegistrationBusFactory(configure));
        }

        public static void LocalstackHost(this IAmazonSqsBusFactoryConfigurator configurator)
        {
            configurator.Host(new Uri("amazonsqs://localhost:4576"), h =>
            {
                h.AccessKey("admin");
                h.SecretKey("admin");

                h.Config(new AmazonSQSConfig { ServiceURL = "http://localhost:4566" });
                h.Config(new AmazonSimpleNotificationServiceConfig { ServiceURL = "http://localhost:4566" });
            });
        }
    }
}
