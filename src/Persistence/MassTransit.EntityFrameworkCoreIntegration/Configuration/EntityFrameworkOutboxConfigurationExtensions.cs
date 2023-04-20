#nullable enable
namespace MassTransit
{
    using System;
    using Configuration;
    using EntityFrameworkCoreIntegration;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore.Metadata.Builders;


    public static class EntityFrameworkOutboxConfigurationExtensions
    {
        /// <summary>
        /// Configures the Entity Framework Outbox on the bus, which can subsequently be used to configure
        /// the transactional outbox on a receive endpoint.
        /// </summary>
        /// <param name="configurator"></param>
        /// <param name="configure"></param>
        /// <typeparam name="TDbContext"></typeparam>
        /// <returns></returns>
        public static void AddEntityFrameworkOutbox<TDbContext>(this IBusRegistrationConfigurator configurator,
            Action<IEntityFrameworkOutboxConfigurator>? configure = null)
            where TDbContext : DbContext
        {
            var outboxConfigurator = new EntityFrameworkOutboxConfigurator<TDbContext>(configurator);

            outboxConfigurator.Configure(configure);
        }

        /// <summary>
        /// Configure the Entity Framework outbox on the receive endpoint
        /// </summary>
        /// <param name="configurator"></param>
        /// <param name="context">Configuration service provider</param>
        /// <param name="configure"></param>
        public static void UseEntityFrameworkOutbox<TDbContext>(this IReceiveEndpointConfigurator configurator, IRegistrationContext context,
            Action<IOutboxOptionsConfigurator>? configure = null)
            where TDbContext : DbContext
        {
            if (configurator == null)
                throw new ArgumentNullException(nameof(configurator));
            if (context == null)
                throw new ArgumentNullException(nameof(context));

            var observer = new OutboxConsumePipeSpecificationObserver<TDbContext>(configurator, context);

            configure?.Invoke(observer);

            configurator.ConnectConsumerConfigurationObserver(observer);
            configurator.ConnectSagaConfigurationObserver(observer);
        }

        /// <summary>
        /// Configure the outbox for use with SQL Server
        /// </summary>
        /// <param name="configurator"></param>
        /// <returns></returns>
        public static IEntityFrameworkOutboxConfigurator UseSqlServer(this IEntityFrameworkOutboxConfigurator configurator)
        {
            configurator.LockStatementProvider = new SqlServerLockStatementProvider();

            return configurator;
        }

        /// <summary>
        /// Configure the outbox for use with Postgres
        /// </summary>
        /// <param name="configurator"></param>
        /// <returns></returns>
        public static IEntityFrameworkOutboxConfigurator UsePostgres(this IEntityFrameworkOutboxConfigurator configurator)
        {
            configurator.LockStatementProvider = new PostgresLockStatementProvider();

            return configurator;
        }

        /// <summary>
        /// Configure the outbox for use with MySQL
        /// </summary>
        /// <param name="configurator"></param>
        /// <returns></returns>
        public static IEntityFrameworkOutboxConfigurator UseMySql(this IEntityFrameworkOutboxConfigurator configurator)
        {
            configurator.LockStatementProvider = new MySqlLockStatementProvider();

            return configurator;
        }

        /// <summary>
        /// Configure the outbox for use with SQLite
        /// </summary>
        /// <param name="configurator"></param>
        /// <returns></returns>
        public static IEntityFrameworkOutboxConfigurator UseSqlite(this IEntityFrameworkOutboxConfigurator configurator)
        {
            configurator.LockStatementProvider = new SqliteLockStatementProvider();

            return configurator;
        }

        public static void AddInboxStateEntity(this ModelBuilder modelBuilder, Action<EntityTypeBuilder<InboxState>>? callback = null)
        {
            EntityTypeBuilder<InboxState> inbox = modelBuilder.Entity<InboxState>();

            inbox.Property(p => p.Id);
            inbox.HasKey(p => p.Id);

            inbox.Property(p => p.MessageId);
            inbox.Property(p => p.ConsumerId);

            inbox.HasAlternateKey(p => new
            {
                p.MessageId,
                p.ConsumerId
            });

            inbox.Property(p => p.LockId);

            inbox.Property(p => p.RowVersion).IsRowVersion();

            inbox.Property(p => p.Received);
            inbox.Property(p => p.ReceiveCount);
            inbox.Property(p => p.ExpirationTime);
            inbox.Property(p => p.Consumed);

            inbox.Property(p => p.Delivered);
            inbox.HasIndex(p => p.Delivered);

            inbox.Property(p => p.LastSequenceNumber);

            callback?.Invoke(inbox);
        }

        public static void AddOutboxStateEntity(this ModelBuilder modelBuilder, Action<EntityTypeBuilder<OutboxState>>? callback = null)
        {
            EntityTypeBuilder<OutboxState> outbox = modelBuilder.Entity<OutboxState>();

            outbox.Property(p => p.OutboxId);
            outbox.HasKey(p => p.OutboxId);

            outbox.Property(p => p.LockId);

            outbox.Property(p => p.RowVersion).IsRowVersion();

            outbox.Property(p => p.Created);
            outbox.HasIndex(p => p.Created);

            outbox.Property(p => p.Delivered);
            outbox.Property(p => p.LastSequenceNumber);

            callback?.Invoke(outbox);
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
                p.OutboxId,
                p.SequenceNumber,
            }).IsUnique();

            outbox.Property(p => p.Headers);

            outbox.Property(p => p.Properties);

            outbox.Property(p => p.ContentType)
                .HasMaxLength(256);

            outbox.Property(p => p.Body);

            callback?.Invoke(outbox);
        }
    }
}
