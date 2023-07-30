#nullable enable
namespace MassTransit
{
    using System;
    using Configuration;
    using DependencyInjection;
    using MongoDbIntegration;


    public static class MongoDbOutboxConfigurationExtensions
    {
        /// <summary>
        /// Configures the Mongo DB outbox on the bus, which can subsequently be used to configure
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
        /// Configure the Mongo DB outbox on the receive endpoint
        /// </summary>
        /// <param name="configurator"></param>
        /// <param name="context">Configuration service provider</param>
        /// <param name="configure"></param>
        public static void UseMongoDbOutbox(this IReceiveEndpointConfigurator configurator, IRegistrationContext context,
            Action<IOutboxOptionsConfigurator>? configure = null)
        {
            if (configurator == null)
                throw new ArgumentNullException(nameof(configurator));
            if (context == null)
                throw new ArgumentNullException(nameof(context));

            var observer = new OutboxConsumePipeSpecificationObserver<MongoDbContext>(configurator, context);

            configure?.Invoke(observer);

            configurator.ConnectConsumerConfigurationObserver(observer);
            configurator.ConnectSagaConfigurationObserver(observer);
        }

        /// <summary>
        /// Configure the Entity Framework outbox on the receive endpoint
        /// </summary>
        /// <param name="configurator"></param>
        /// <param name="provider">Configuration service provider</param>
        /// <param name="configure"></param>
        [Obsolete("Use the IRegistrationContext overload instead. Visit https://masstransit.io/obsolete for details.")]
        public static void UseMongoDbOutbox(this IReceiveEndpointConfigurator configurator, IServiceProvider provider,
            Action<IOutboxOptionsConfigurator>? configure = null)
        {
            if (configurator == null)
                throw new ArgumentNullException(nameof(configurator));
            if (provider == null)
                throw new ArgumentNullException(nameof(provider));

            var observer = new OutboxConsumePipeSpecificationObserver<MongoDbContext>(configurator, provider, LegacySetScopedConsumeContext.Instance);

            configure?.Invoke(observer);

            configurator.ConnectConsumerConfigurationObserver(observer);
            configurator.ConnectSagaConfigurationObserver(observer);
        }
    }
}
