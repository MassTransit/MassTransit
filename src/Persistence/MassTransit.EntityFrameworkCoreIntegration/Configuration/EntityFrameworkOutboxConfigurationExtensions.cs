#nullable enable
namespace MassTransit
{
    using System;
    using System.Linq;
    using Configuration;
    using DependencyInjection;
    using EntityFrameworkCoreIntegration;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore.Metadata.Builders;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Options;
    using Middleware;


    public static class EntityFrameworkOutboxConfigurationExtensions
    {
        /// <summary>
        /// Adds the DbContext Outbox to the container, using the specified <typeparamref name="TDbContext" /> type.
        /// The DbContext should use AddInboxMessageEntity and AddOutboxMessageEntity to register the outbox entities.
        /// </summary>
        /// <param name="collection"></param>
        /// <typeparam name="TDbContext"></typeparam>
        /// <returns></returns>
        public static IServiceCollection AddEntityFrameworkOutbox<TDbContext>(this IServiceCollection collection)
            where TDbContext : DbContext
        {
            collection.AddScoped<IOutboxContextFactory<TDbContext>, EntityFrameworkOutboxContextFactory<TDbContext>>();
            collection.AddOptions<EntityFrameworkOutboxOptions>();

            return collection;
        }

        /// <summary>
        /// Adds an Entity Framework on ramp (Outbox) to the bus, which is stores all messages that are published/sent using the bus
        /// (including any ISendEndpointProvider and IPublishEndpoint references that resolve to the bus) in the database
        /// associated with the specified DbContext. Those messages are later sent to the message broker asynchronously using
        /// a separate message delivery hosted service.
        /// </summary>
        /// <param name="busConfigurator"></param>
        /// <typeparam name="TDbContext"></typeparam>
        /// <returns></returns>
        public static IBusRegistrationConfigurator AddEntityFrameworkOnRamp<TDbContext>(this IBusRegistrationConfigurator busConfigurator)
            where TDbContext : DbContext
        {
            ReplaceScopedBusContextProvider<IBus, TDbContext>(busConfigurator);

            return busConfigurator;
        }

        /// <summary>
        /// Adds an Entity Framework on ramp (Outbox) to the bus, which is stores all messages that are published/sent using the bus
        /// (including any ISendEndpointProvider and IPublishEndpoint references that resolve to the bus) in the database
        /// associated with the specified DbContext. Those messages are later sent to the message broker asynchronously using
        /// a separate message delivery hosted service.
        /// </summary>
        /// <param name="busConfigurator"></param>
        /// <typeparam name="TDbContext"></typeparam>
        /// <typeparam name="TBus"></typeparam>
        /// <returns></returns>
        public static IBusRegistrationConfigurator AddEntityFrameworkOnRamp<TBus, TDbContext>(this IBusRegistrationConfigurator<TBus> busConfigurator)
            where TDbContext : DbContext
            where TBus : class, IBus
        {
            ReplaceScopedBusContextProvider<TBus, TDbContext>(busConfigurator);

            return busConfigurator;
        }

        /// <summary>
        /// Adds the DbContext Outbox to the container, using the specified <typeparamref name="TDbContext" /> type.
        /// The DbContext should use AddInboxMessageEntity and AddOutboxMessageEntity to register the outbox entities.
        /// </summary>
        /// <param name="collection"></param>
        /// <param name="configure">Configure the delivery options</param>
        /// <typeparam name="TDbContext"></typeparam>
        /// <returns></returns>
        public static IServiceCollection AddEntityFrameworkOnRampDeliveryService<TDbContext>(this IServiceCollection collection,
            Action<OnRampDeliveryOptions>? configure = null)
            where TDbContext : DbContext
        {
            collection.AddHostedService<OnRampDeliveryHostedService<TDbContext>>();
            collection.AddOptions<EntityFrameworkOutboxOptions>();

            OptionsBuilder<OnRampDeliveryOptions>? options = collection.AddOptions<OnRampDeliveryOptions>();

            if (configure != null)
                options.Configure(configure);

            return collection;
        }

        static void ReplaceScopedBusContextProvider<TBus, TDbContext>(IServiceCollection busConfigurator)
            where TBus : class, IBus
            where TDbContext : DbContext
        {
            var descriptor = busConfigurator.FirstOrDefault(x => x.ServiceType == typeof(IScopedBusContextProvider<TBus>));
            if (descriptor != null)
                busConfigurator.Remove(descriptor);

            busConfigurator.AddScoped<IScopedBusContextProvider<TBus>, EntityFrameworkScopedBusContextProvider<TBus, TDbContext>>();
        }

        /// <summary>
        /// Configure the Entity Framework outbox on the receive endpoint
        /// </summary>
        /// <param name="configurator"></param>
        /// <param name="provider">Configuration service provider</param>
        public static void UseEntityFrameworkOutbox<TDbContext>(this IReceiveEndpointConfigurator configurator, IServiceProvider provider)
            where TDbContext : DbContext
        {
            if (configurator == null)
                throw new ArgumentNullException(nameof(configurator));
            if (provider == null)
                throw new ArgumentNullException(nameof(provider));

            var observer = new OutboxConsumePipeSpecificationObserver<TDbContext>(configurator, provider);

            configurator.ConnectConsumerConfigurationObserver(observer);
            configurator.ConnectSagaConfigurationObserver(observer);
        }

        public static void AddInboxStateEntity(this ModelBuilder modelBuilder, Action<EntityTypeBuilder<InboxState>>? callback = null)
        {
            EntityTypeBuilder<InboxState> inbox = modelBuilder.Entity<InboxState>();

            inbox.Property(p => p.Id);
            inbox.HasKey(p => p.Id);

            inbox.Property(p => p.MessageId);
            inbox.Property(p => p.ConsumerId);
            inbox.HasIndex(p => new
            {
                p.MessageId,
                p.ConsumerId
            }).IsUnique();

            inbox.Property(p => p.Received);
            inbox.Property(p => p.ReceiveCount);
            inbox.Property(p => p.ExpirationTime);
            inbox.Property(p => p.Consumed);
            inbox.Property(p => p.Delivered);
            inbox.Property(p => p.LastSequenceNumber);

            callback?.Invoke(inbox);
        }

        public static void SetOptimistic(this EntityTypeBuilder<InboxState> builder)
        {
            builder.Property(x => x.RowVersion)
                .IsRowVersion();
        }

        public static void AddOutboxStateEntity(this ModelBuilder modelBuilder, Action<EntityTypeBuilder<OutboxState>>? callback = null)
        {
            EntityTypeBuilder<OutboxState> outbox = modelBuilder.Entity<OutboxState>();

            outbox.Property(p => p.OutboxId);
            outbox.HasKey(p => p.OutboxId);

            outbox.Property(p => p.Delivered);
            outbox.Property(p => p.LastSequenceNumber);

            callback?.Invoke(outbox);
        }

        public static void SetOptimistic(this EntityTypeBuilder<OutboxState> builder)
        {
            builder.Property(x => x.RowVersion)
                .IsRowVersion();
        }

        public static void AddOutboxMessageEntity(this ModelBuilder modelBuilder, Action<EntityTypeBuilder<OutboxMessage>>? callback = null)
        {
            EntityTypeBuilder<OutboxMessage> outbox = modelBuilder.Entity<OutboxMessage>();

            outbox.Property(p => p.SequenceNumber);
            outbox.HasKey(p => p.SequenceNumber);

            outbox.Property(p => p.MessageId);

            outbox.Property(p => p.ConversationId);
            outbox.Property(p => p.CorrelationId);
            outbox.Property(p => p.InitiatorId);
            outbox.Property(p => p.RequestId);

            outbox.Property(p => p.SourceAddress).HasMaxLength(256);
            outbox.Property(p => p.DestinationAddress).HasMaxLength(256);
            outbox.Property(p => p.ResponseAddress).HasMaxLength(256);
            outbox.Property(p => p.FaultAddress).HasMaxLength(256);

            outbox.Property(p => p.ExpirationTime);

            outbox.HasIndex(p => p.ExpirationTime);

            outbox.Property(p => p.EnqueueTime);

            outbox.HasIndex(p => p.EnqueueTime);

            outbox.Property(p => p.SentTime);

            outbox.Property(p => p.InboxMessageId);
            outbox.Property(p => p.InboxConsumerId);
            outbox.HasIndex(p => new
            {
                p.InboxMessageId,
                p.InboxConsumerId,
                p.SequenceNumber
            }).IsUnique();

            outbox.Property(p => p.OutboxId);
            outbox.HasIndex(p => new
            {
                p.SequenceNumber,
                p.OutboxId,
            }).IsUnique();

            outbox.Property(p => p.Headers);

            outbox.Property(p => p.Properties);

            outbox.Property(p => p.ContentType)
                .HasMaxLength(256);

            outbox.Property(p => p.Body);

            callback?.Invoke(outbox);
        }

        public static void SetOptimistic(this EntityTypeBuilder<OutboxMessage> builder)
        {
            builder.Property(x => x.RowVersion)
                .IsRowVersion();
        }
    }
}
