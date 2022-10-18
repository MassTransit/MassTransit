namespace MassTransit.EntityFrameworkCoreIntegration.Tests.ReliableMessaging
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using Logging;
    using Microsoft.Extensions.DependencyInjection;
    using NUnit.Framework;
    using Responsible;
    using Testing;


    [TestFixture]
    public class Responding_through_the_outbox
    {
        [Test]
        public async Task Should_fault_when_failed_to_start()
        {
            using var tracerProvider = TraceConfig.CreateTraceProvider("ef-core-tests");

            await using var provider = CreateServiceProvider();

            var harness = provider.GetTestHarness();
            harness.TestInactivityTimeout = TimeSpan.FromSeconds(5);

            await harness.Start();

            try
            {
                IRequestClient<Start> client = harness.GetRequestClient<Start>();

                Assert.That(async () => await client.GetResponse<StartupComplete>(new Start { FailToStart = true }, harness.CancellationToken),
                    Throws.TypeOf<RequestFaultException>());
            }
            finally
            {
                await harness.Stop();
            }
        }

        [Test]
        public async Task Should_only_publish_one_fault()
        {
            using var tracerProvider = TraceConfig.CreateTraceProvider("ef-core-tests");

            await using var provider = CreateServiceProvider();

            var harness = provider.GetTestHarness();
            harness.TestInactivityTimeout = TimeSpan.FromSeconds(5);

            await harness.Start();

            var count = 0;
            harness.Bus.ConnectHandler<Fault<Start>>(async context =>
            {
                Interlocked.Increment(ref count);
            });

            try
            {
                IRequestClient<Start> client = harness.GetRequestClient<Start>();

                Assert.That(async () => await client.GetResponse<StartupComplete>(new Start { FailToStart = true }, harness.CancellationToken),
                    Throws.TypeOf<RequestFaultException>());

                await harness.InactivityTask;

                Assert.That(count, Is.EqualTo(1));
            }
            finally
            {
                await harness.Stop();
            }
        }

        [Test]
        public async Task Should_start_successfully()
        {
            using var tracerProvider = TraceConfig.CreateTraceProvider("ef-core-tests");

            await using var provider = CreateServiceProvider();

            var harness = provider.GetTestHarness();

            await harness.Start();

            try
            {
                IRequestClient<Start> client = harness.GetRequestClient<Start>();

                Response<StartupComplete> complete = await client.GetResponse<StartupComplete>(new Start(), harness.CancellationToken);
            }
            finally
            {
                await harness.Stop();
            }
        }

        [Test]
        [Explicit]
        public async Task Should_start_with_delay_successfully()
        {
            using var tracerProvider = TraceConfig.CreateTraceProvider("ef-core-tests");

            await using var provider = CreateServiceProvider(x =>
            {
                x.UsingInMemory((context,cfg)=>
                {
                    cfg.UseDelayedMessageScheduler();
                    cfg.UseNewtonsoftJsonSerializer();

                    cfg.ConfigureEndpoints(context);
                });
            });

            var harness = provider.GetTestHarness();

            harness.TestInactivityTimeout = TimeSpan.FromSeconds(5);

            await harness.Start();

            try
            {
                IRequestClient<Start> client = harness.GetRequestClient<Start>();

                var startTime = DateTime.UtcNow;

                Response<StartupComplete> complete = await client.GetResponse<StartupComplete>(new Start() { Delay = TimeSpan.FromSeconds(3) },
                    harness.CancellationToken);

                var endTime = DateTime.UtcNow;

                Assert.That(endTime - startTime, Is.GreaterThanOrEqualTo(TimeSpan.FromSeconds(2.9)));
            }
            finally
            {
                await harness.Stop();
            }
        }

        static ServiceProvider CreateServiceProvider(Action<IBusRegistrationConfigurator> callback = null)
        {
            var services = new ServiceCollection();

            services.AddDbContext<ResponsibleDbContext>(builder =>
            {
                ResponsibleDbContextFactory.Apply(builder);
            });

            services
                .AddHostedService<MigrationHostedService<ResponsibleDbContext>>()
                .AddMassTransitTestHarness(x =>
                {
                    x.AddEntityFrameworkOutbox<ResponsibleDbContext>();

                    x.AddSagaStateMachine<ResponsibleStateMachine, ResponsibleState, ResponsibleStateDefinition>()
                        .EntityFrameworkRepository(r => r.ExistingDbContext<ResponsibleDbContext>());

                    callback?.Invoke(x);
                });

            services.AddOptions<TextWriterLoggerOptions>().Configure(options => options.Disable("Microsoft"));

            return services.BuildServiceProvider(true);
        }
    }


    namespace Responsible
    {
        using System;
        using System.Collections.Generic;
        using System.Reflection;
        using Microsoft.EntityFrameworkCore;
        using Microsoft.EntityFrameworkCore.Design;
        using Microsoft.EntityFrameworkCore.Metadata.Builders;
        using TestFramework;


        public class ResponsibleStateDefinition :
            SagaDefinition<ResponsibleState>
        {
            readonly IServiceProvider _provider;

            public ResponsibleStateDefinition(IServiceProvider provider)
            {
                _provider = provider;
            }

            protected override void ConfigureSaga(IReceiveEndpointConfigurator endpointConfigurator,
                ISagaConfigurator<ResponsibleState> consumerConfigurator)
            {
                endpointConfigurator.UseMessageRetry(r => r.Intervals(10, 50, 100, 100, 100, 100, 100, 100));

                endpointConfigurator.UseEntityFrameworkOutbox<ResponsibleDbContext>(_provider);
            }
        }


        class ResponsibleStateMachine :
            MassTransitStateMachine<ResponsibleState>
        {
            public ResponsibleStateMachine()
            {
                InstanceState(x => x.CurrentState);

                Schedule(() => DelayStart, x => x.DelayStartTokenId, x => x.Received = e => e.CorrelateById(m => m.Message.CorrelationId));

                Initially(
                    When(Started, x => x.Data.FailToStart)
                        .Then(context => throw new IntentionalTestException()),
                    When(Started, x => x.Data.FailToStart == false && x.Data.Delay.HasValue == false)
                        .Respond(new StartupComplete())
                        .TransitionTo(Running),
                    When(Started, x => x.Data.FailToStart == false && x.Data.Delay.HasValue)
                        .Then(context =>
                        {
                            context.Saga.RequestId = context.RequestId;
                            context.Saga.ResponseAddress = context.ResponseAddress;
                        })
                        .Schedule(DelayStart, x => new DelayStart { CorrelationId = x.Saga.CorrelationId }, x => x.Data.Delay.Value)
                        .TransitionTo(Delayed));

                During(Delayed,
                    When(DelayStart.Received)
                        .Send(x => x.Saga.ResponseAddress, x => new StartupComplete(), (context, x) => x.RequestId = context.Saga.RequestId)
                        .TransitionTo(Running));
            }

            public Schedule<ResponsibleState, DelayStart> DelayStart { get; set; }

            public State Running { get; private set; }
            public State Delayed { get; private set; }
            public Event<Start> Started { get; private set; }
        }


        public class Start :
            CorrelatedBy<Guid>
        {
            public Start()
            {
                CorrelationId = NewId.NextGuid();
            }

            public bool FailToStart { get; set; }
            public TimeSpan? Delay { get; set; }

            public Guid CorrelationId { get; private set; }
        }


        public class DelayStart :
            CorrelatedBy<Guid>
        {
            public Guid CorrelationId { get; set; }
        }


        class StartupComplete
        {
        }


        public class ResponsibleState :
            SagaStateMachineInstance
        {
            public string CurrentState { get; set; }

            public Guid? RequestId { get; set; }
            public Uri ResponseAddress { get; set; }
            public Guid? DelayStartTokenId { get; set; }
            public Guid CorrelationId { get; set; }
        }


        public class ResponsibleStateMap :
            SagaClassMap<ResponsibleState>
        {
            protected override void Configure(EntityTypeBuilder<ResponsibleState> entity, ModelBuilder model)
            {
                entity.Property(x => x.CurrentState);
                entity.Property(x => x.RequestId);
                entity.Property(x => x.ResponseAddress);
                entity.Property(x => x.DelayStartTokenId);
            }
        }


        public class ResponsibleDbContext :
            SagaDbContext
        {
            public ResponsibleDbContext(DbContextOptions<ResponsibleDbContext> options)
                : base(options)
            {
            }

            protected override IEnumerable<ISagaClassMap> Configurations
            {
                get { yield return new ResponsibleStateMap(); }
            }

            protected override void OnModelCreating(ModelBuilder modelBuilder)
            {
                base.OnModelCreating(modelBuilder);

                modelBuilder.AddInboxStateEntity();
                modelBuilder.AddOutboxMessageEntity();
            }
        }


        public class ResponsibleDbContextFactory :
            IDesignTimeDbContextFactory<ResponsibleDbContext>
        {
            public ResponsibleDbContext CreateDbContext(string[] args)
            {
                var builder = new DbContextOptionsBuilder<ResponsibleDbContext>();

                Apply(builder);

                return new ResponsibleDbContext(builder.Options);
            }

            public static void Apply(DbContextOptionsBuilder builder)
            {
                builder.UseSqlServer(LocalDbConnectionStringProvider.GetLocalDbConnectionString(), options =>
                {
                    options.MigrationsAssembly(Assembly.GetExecutingAssembly().GetName().Name);
                    options.MigrationsHistoryTable($"__{nameof(ResponsibleDbContext)}");

                    options.MinBatchSize(1);
                });

                builder.EnableSensitiveDataLogging();
            }

            public ResponsibleDbContext CreateDbContext(DbContextOptionsBuilder<ResponsibleDbContext> optionsBuilder)
            {
                return new ResponsibleDbContext(optionsBuilder.Options);
            }
        }
    }
}
