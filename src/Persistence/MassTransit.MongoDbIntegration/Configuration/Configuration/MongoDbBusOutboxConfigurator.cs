#nullable enable
namespace MassTransit.Configuration
{
    using System;
    using DependencyInjection;
    using Microsoft.Extensions.DependencyInjection;
    using Middleware.Outbox;
    using MongoDbIntegration;


    public class MongoDbBusOutboxConfigurator :
        IMongoDbBusOutboxConfigurator
    {
        readonly IBusRegistrationConfigurator _configurator;
        readonly MongoDbOutboxConfigurator _outboxConfigurator;

        public MongoDbBusOutboxConfigurator(IBusRegistrationConfigurator configurator, MongoDbOutboxConfigurator outboxConfigurator)
        {
            _outboxConfigurator = outboxConfigurator;
            _configurator = configurator;
        }

        /// <summary>
        /// The number of message to deliver at a time from the outbox
        /// </summary>
        public int MessageDeliveryLimit { get; set; } = 100;

        /// <summary>
        /// Transport Send timeout when delivering messages to the transport
        /// </summary>
        public TimeSpan MessageDeliveryTimeout { get; set; } = TimeSpan.FromSeconds(10);

        public void DisableDeliveryService()
        {
            _configurator.RemoveHostedService<BusOutboxDeliveryService>();
        }

        public virtual void Configure(Action<IMongoDbBusOutboxConfigurator>? configure)
        {
            _configurator.AddHostedService<BusOutboxDeliveryService>();
            _configurator.ReplaceScoped<IScopedBusContextProvider<IBus>, MongoDbScopedBusContextProvider<IBus>>();
            _configurator.AddSingleton<IBusOutboxNotification, BusOutboxNotification>();

            _configurator.AddOptions<OutboxDeliveryServiceOptions>()
                .Configure(options =>
                {
                    options.QueryDelay = _outboxConfigurator.QueryDelay;
                    options.QueryMessageLimit = _outboxConfigurator.QueryMessageLimit;
                    options.QueryTimeout = _outboxConfigurator.QueryTimeout;
                    options.MessageDeliveryLimit = MessageDeliveryLimit;
                    options.MessageDeliveryTimeout = MessageDeliveryTimeout;
                });

            configure?.Invoke(this);
        }
    }
}
