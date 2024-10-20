namespace MassTransit.EntityFrameworkCoreIntegration.Tests.ReliableMessaging;

using System;
using System.Reflection;
using System.Threading.Tasks;
using Logging;
using MassTransit.Tests;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using NUnit.Framework;
using Testing;


public class OutboxTransactionFault_Specs
{
    [Test]
    public async Task Should_throw_typed_exception()
    {
        var services = new ServiceCollection();

        services
            .AddDbContext<TestDbContext>(builder =>
            {
                builder.UseSqlServer(LocalDbConnectionStringProvider.GetLocalDbConnectionString(), options =>
                {
                    options.MigrationsAssembly(Assembly.GetExecutingAssembly().GetName().Name);
                    options.MigrationsHistoryTable($"__{nameof(TestDbContext)}");

                    options.MinBatchSize(1);
                });

                builder.EnableSensitiveDataLogging();
            });

        services
            .AddHostedService<MigrationHostedService<TestDbContext>>()
            .AddMassTransitTestHarness(x =>
            {
                x.AddEntityFrameworkOutbox<TestDbContext>();

                x.SetTestTimeouts(testInactivityTimeout: TimeSpan.FromSeconds(10));

                x.AddConsumer<TestMessageConsumer>();
                x.AddConsumer<TestFaultTypedMessageConsumer>();
                x.AddConsumer<TestFaultNotTypedMessageConsumer>();


                x.AddConfigureEndpointsCallback((context, name, cfg) =>
                {
                    cfg.UseEntityFrameworkOutbox<TestDbContext>(context);
                });

                x.UsingInMemory((context, cfg) =>
                {
                    cfg.UseDelayedMessageScheduler();

                    cfg.ConfigureEndpoints(context);
                });
            });

        services.AddOptions<TextWriterLoggerOptions>()
            .Configure(options => options.Disable("Microsoft"));

        await using var provider = services.BuildServiceProvider(true);

        var harness = await provider.StartTestHarness();

        var testEntityId = Guid.NewGuid();

        await harness.Bus.Publish(new TestMessage
        {
            TestId = testEntityId,
            Key = "A",
            ThrowInConsumer = false
        });

        Assert.That(await harness.Consumed.Any<TestMessage>(x => x.Context.Message.Key == "A"));

        await harness.Bus.Publish(new TestMessage
        {
            TestId = testEntityId,
            Key = "C",
            ThrowInConsumer = false
        });

        Assert.That(await harness.Consumed.Any<TestMessage>(x => x.Context.Message.Key == "C"));

        Assert.That(await harness.Published.Any<Fault<TestMessage>>(x => x.Context.Message.Message.Key == "C"));

        await harness.Stop();
    }


    public class TestMessage
    {
        public Guid TestId { get; set; }
        public string Key { get; set; }

        public bool ThrowInConsumer { get; set; }
    }


    class TestDbContext(DbContextOptions<TestDbContext> options) :
        DbContext(options)
    {
        public DbSet<TestEntity> TestEntities { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.AddTransactionalOutboxEntities();
        }
    }


    class TestEntity
    {
        public Guid Id { get; set; }
    }


    class TestMessageConsumer(TestDbContext testDbContext) :
        IConsumer<TestMessage>
    {
        public async Task Consume(ConsumeContext<TestMessage> context)
        {
            if (context.Message.ThrowInConsumer)
                throw new InvalidOperationException("Throw requested by messaged inside consumer");

            await testDbContext.AddAsync(new TestEntity { Id = context.Message.TestId });
        }
    }


    class TestFaultTypedMessageConsumer(ILogger<TestFaultTypedMessageConsumer> logger) : IConsumer<Fault<TestMessage>>
    {
        public Task Consume(ConsumeContext<Fault<TestMessage>> context)
        {
            logger.LogInformation("Consumed typed FAULT for Key {testId}", context.Message.Message.Key);
            return Task.CompletedTask;
        }
    }


    class TestFaultNotTypedMessageConsumer(ILogger<TestFaultNotTypedMessageConsumer> logger) : IConsumer<Fault>
    {
        public Task Consume(ConsumeContext<Fault> context)
        {
            logger.LogInformation("Consumed NOTTYPED FAULT");
            return Task.CompletedTask;
        }
    }
}
