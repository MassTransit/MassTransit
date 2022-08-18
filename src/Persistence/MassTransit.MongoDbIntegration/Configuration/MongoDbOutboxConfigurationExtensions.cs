#nullable enable
namespace MassTransit
{
    using System;
    using Configuration;
    using MongoDbIntegration;


    public static class MongoDbOutboxConfigurationExtensions
    {
        /// <summary>
        /// Configures the MongoDb Outbox on the bus, which can subsequently be used to configure
        /// the transactional outbox on a receive endpoint.
        /// </summary>
        /// <param name="configurator"></param>
        /// <param name="configure"></param>
        /// <returns></returns>
        public static void AddMongoDbOutbox(this IBusRegistrationConfigurator configurator,
            Action<IMongoDbOutboxConfigurator>? configure = null)
        {
            var outboxConfigurator = new MongoDbOutboxConfigurator(configurator);

            outboxConfigurator.Configure(configure);
        }

        /// <summary>
        /// Configure the MongoDb outbox on the receive endpoint
        /// </summary>
        /// <param name="configurator"></param>
        /// <param name="provider">Configuration service provider</param>
        public static void UseMongoDbOutbox(this IReceiveEndpointConfigurator configurator, IServiceProvider provider)
        {
            if (configurator == null)
                throw new ArgumentNullException(nameof(configurator));
            if (provider == null)
                throw new ArgumentNullException(nameof(provider));

            var observer = new OutboxConsumePipeSpecificationObserver<MongoDbContext>(configurator, provider);

            configurator.ConnectConsumerConfigurationObserver(observer);
            configurator.ConnectSagaConfigurationObserver(observer);
        }
    }
}
