#nullable enable
namespace MassTransit.Configuration
{
    using System;
    using DependencyInjection;
    using EntityFrameworkCoreIntegration;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.Extensions.DependencyInjection;


    public class EntityFrameworkBusOutboxConfigurator<TDbContext> :
        IEntityFrameworkBusOutboxConfigurator
        where TDbContext : DbContext
    {
        readonly IBusRegistrationConfigurator _configurator;
        readonly EntityFrameworkOutboxConfigurator<TDbContext> _outboxConfigurator;

        public EntityFrameworkBusOutboxConfigurator(IBusRegistrationConfigurator configurator, EntityFrameworkOutboxConfigurator<TDbContext> outboxConfigurator)
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
            _configurator.RemoveHostedService<BusOutboxDeliveryService<TDbContext>>();
        }

        public virtual void Configure(Action<IEntityFrameworkBusOutboxConfigurator>? configure)
        {
            _configurator.AddHostedService<BusOutboxDeliveryService<TDbContext>>();
            _configurator.ReplaceScoped<IScopedBusContextProvider<IBus>, EntityFrameworkScopedBusContextProvider<IBus, TDbContext>>();

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
