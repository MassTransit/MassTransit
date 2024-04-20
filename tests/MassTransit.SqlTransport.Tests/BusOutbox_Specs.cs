namespace MassTransit.DbTransport.Tests
{
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Linq;
    using System.Reflection;
    using System.Text.RegularExpressions;
    using System.Threading;
    using System.Threading.Tasks;
    using EntityFrameworkCoreIntegration;
    using Logging;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore.Design;
    using Microsoft.EntityFrameworkCore.Metadata.Internal;
    using Microsoft.Extensions.DependencyInjection;
    using NUnit.Framework;
    using OutboxTypes;
    using Testing;


    public class Using_the_bus_outbox
    {
        [Test]
        public async Task Should_work_with_the_db_transport()
        {
            await using var provider = new ServiceCollection()
                .AddBusOutboxServices()
                .ConfigurePostgresTransport()
                .AddMassTransitTestHarness(x =>
                {
                    x.SetTestTimeouts(testInactivityTimeout: TimeSpan.FromSeconds(10));

                    x.AddEntityFrameworkOutbox<ReliableDbContext>(o =>
                    {
                        o.QueryDelay = TimeSpan.FromSeconds(1);
                        o.UsePostgres();

                        o.UseBusOutbox(bo =>
                        {
                            bo.MessageDeliveryLimit = 10;
                        });
                    });

                    x.AddConsumer<PingConsumer>();

                    x.UsingPostgres((context, cfg) =>
                    {
                        cfg.ConfigureEndpoints(context);
                    });
                })
                .BuildServiceProvider(true);

            var harness = provider.GetTestHarness();

            await harness.Start();

            IConsumerTestHarness<PingConsumer> consumerHarness = harness.GetConsumerHarness<PingConsumer>();

            try
            {
                {
                    await using var dbContext = harness.Scope.ServiceProvider.GetRequiredService<ReliableDbContext>();

                    var transaction = await dbContext.Database.BeginTransactionAsync(IsolationLevel.Serializable);

                    var publishEndpoint = harness.Scope.ServiceProvider.GetRequiredService<IPublishEndpoint>();

                    await publishEndpoint.Publish(new PingMessage());

                    using var cts = new CancellationTokenSource(TimeSpan.FromSeconds(2));

                    Assert.That(await consumerHarness.Consumed.Any<PingMessage>(cts.Token), Is.False);

                    await dbContext.SaveChangesAsync(harness.CancellationToken);

                    await transaction.CommitAsync();
                }

                Assert.That(await consumerHarness.Consumed.Any<PingMessage>(), Is.True);

                IReceivedMessage<PingMessage> context = harness.Consumed.Select<PingMessage>().First();

                Assert.Multiple(() =>
                {
                    Assert.That(context.Context.MessageId, Is.Not.Null);
                    Assert.That(context.Context.ConversationId, Is.Not.Null);
                    Assert.That(context.Context.DestinationAddress, Is.Not.Null);
                    Assert.That(context.Context.SourceAddress, Is.Not.Null);
                });
            }
            finally
            {
                await harness.Stop();
            }
        }


        class PingConsumer :
            IConsumer<PingMessage>
        {
            public Task Consume(ConsumeContext<PingMessage> context)
            {
                return Task.CompletedTask;
            }
        }
    }


    namespace OutboxTypes
    {
        public record PingMessage;
    }


    public class ReliableDbContext :
        SagaDbContext
    {
        public ReliableDbContext(DbContextOptions<ReliableDbContext> options)
            : base(options)
        {
        }

        protected override IEnumerable<ISagaClassMap> Configurations
        {
            get { yield break; }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.HasDefaultSchema("reliable");

            base.OnModelCreating(modelBuilder);

            modelBuilder.AddInboxStateEntity();
            modelBuilder.AddOutboxMessageEntity();
            modelBuilder.AddOutboxStateEntity();

            foreach (var entity in modelBuilder.Model.GetEntityTypes())
            {
                #pragma warning disable EF1001
                if (entity is EntityType { IsImplicitlyCreatedJoinEntityType: true })
                    continue;

                entity.SetTableName(entity.DisplayName());
            }

            ChangeEntityNames(modelBuilder);
        }

        static void ChangeEntityNames(ModelBuilder builder)
        {
            foreach (var entity in builder.Model.GetEntityTypes())
            {
                var tableName = entity.GetTableName();
                if (!string.IsNullOrWhiteSpace(tableName))
                    entity.SetTableName(tableName.ToSnakeCase());

                foreach (var property in entity.GetProperties())
                    property.SetColumnName(property.GetColumnName().ToSnakeCase());
            }
        }
    }


    public static partial class StringExtensions
    {
        public static string ToSnakeCase(this string input)
        {
            if (string.IsNullOrEmpty(input))
                return input;

            var startUnderscores = MyRegex().Match(input);
            return startUnderscores + MyRegex1().Replace(input, "$1_$2").ToLower();
        }

        [GeneratedRegex("^_+")]
        private static partial Regex MyRegex();

        [GeneratedRegex("([a-z0-9])([A-Z])")]
        private static partial Regex MyRegex1();
    }


    public class ReliableDbContextFactory :
        IDesignTimeDbContextFactory<ReliableDbContext>
    {
        public ReliableDbContext CreateDbContext(string[] args)
        {
            var builder = new DbContextOptionsBuilder<ReliableDbContext>();

            Apply(builder);

            return new ReliableDbContext(builder.Options);
        }

        public static void Apply(DbContextOptionsBuilder builder)
        {
            builder.UseNpgsql("host=localhost;user id=postgres;password=Password12!;database=masstransit_transport_tests;", options =>
            {
                options.MigrationsAssembly(Assembly.GetExecutingAssembly().GetName().Name);
                options.MigrationsHistoryTable("reliable_db_context_ef");
            });
        }

        public ReliableDbContext CreateDbContext(DbContextOptionsBuilder<ReliableDbContext> optionsBuilder)
        {
            return new ReliableDbContext(optionsBuilder.Options);
        }
    }


    public static class BusOutboxTestExtensions
    {
        public static IServiceCollection AddBusOutboxServices(this IServiceCollection services)
        {
            services.AddDbContext<ReliableDbContext>(builder =>
            {
                ReliableDbContextFactory.Apply(builder);
            });
            services.AddHostedService<MigrationHostedService<ReliableDbContext>>();

            services.AddOptions<TextWriterLoggerOptions>().Configure(options => options.Disable("Microsoft"));

            return services;
        }
    }
}
