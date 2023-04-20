namespace MassTransit.MongoDbIntegration.Tests
{
    using System.Threading;
    using System.Threading.Tasks;
    using Logging;
    using Microsoft.Extensions.DependencyInjection;
    using NUnit.Framework;
    using Responsible;
    using Testing;


    [Explicit]
    [TestFixture]
    public class Responding_through_the_outbox
    {
        [Test]
        public async Task Should_fault_when_failed_to_start()
        {
            await using var provider = CreateServiceProvider();

            var harness = provider.GetTestHarness();

            await harness.Start();

            IRequestClient<Start> client = harness.GetRequestClient<Start>();

            Assert.That(async () => await client.GetResponse<StartupComplete>(new Start { FailToStart = true }, harness.CancellationToken),
                Throws.TypeOf<RequestFaultException>());

            await harness.Stop();
        }

        [Test]
        public async Task Should_only_publish_one_fault()
        {
            await using var provider = CreateServiceProvider();

            var harness = provider.GetTestHarness();

            await harness.Start();

            var count = 0;
            harness.Bus.ConnectHandler<Fault<Start>>(async context =>
            {
                Interlocked.Increment(ref count);
            });

            IRequestClient<Start> client = harness.GetRequestClient<Start>();

            Assert.That(async () => await client.GetResponse<StartupComplete>(new Start { FailToStart = true }, harness.CancellationToken),
                Throws.TypeOf<RequestFaultException>());

            await harness.InactivityTask;

            Assert.That(count, Is.EqualTo(1));

            await harness.Stop();
        }

        [Test]
        public async Task Should_start_successfully()
        {
            await using var provider = CreateServiceProvider();

            var harness = provider.GetTestHarness();

            await harness.Start();

            IRequestClient<Start> client = harness.GetRequestClient<Start>();

            Response<StartupComplete> complete = await client.GetResponse<StartupComplete>(new Start(), harness.CancellationToken);

            await harness.Stop();
        }

        static ServiceProvider CreateServiceProvider()
        {
            var services = new ServiceCollection();

            services
                .AddMassTransitTestHarness(x =>
                {
                    x.AddMongoDbOutbox(r =>
                    {
                        r.Connection = "mongodb://127.0.0.1:27021";
                        r.DatabaseName = "sagaTest";
                    });

                    x.AddSagaStateMachine<ResponsibleStateMachine, ResponsibleState, ResponsibleStateDefinition>()
                        .MongoDbRepository(r =>
                        {
                            r.Connection = "mongodb://127.0.0.1:27021";
                            r.DatabaseName = "sagaTest";
                        });
                });

            services.AddOptions<TextWriterLoggerOptions>().Configure(options => options.Disable("Microsoft"));

            return services.BuildServiceProvider(true);
        }
    }


    namespace Responsible
    {
        using System;
        using TestFramework;


        public class ResponsibleStateDefinition :
            SagaDefinition<ResponsibleState>
        {
            protected override void ConfigureSaga(IReceiveEndpointConfigurator endpointConfigurator,
                ISagaConfigurator<ResponsibleState> consumerConfigurator, IRegistrationContext context)
            {
                endpointConfigurator.UseMessageRetry(r => r.Intervals(100, 1000));

                endpointConfigurator.UseMongoDbOutbox(context);
            }
        }


        class ResponsibleStateMachine :
            MassTransitStateMachine<ResponsibleState>
        {
            public ResponsibleStateMachine()
            {
                InstanceState(x => x.CurrentState);

                Initially(
                    When(Started, x => x.Data.FailToStart)
                        .Then(context => throw new IntentionalTestException()),
                    When(Started, x => x.Data.FailToStart == false)
                        .Respond(new StartupComplete())
                        .TransitionTo(Running));
            }

            public State Running { get; private set; }
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

            public Guid CorrelationId { get; private set; }
        }


        class StartupComplete
        {
        }


        public class ResponsibleState :
            SagaStateMachineInstance,
            ISagaVersion
        {
            public string CurrentState { get; set; }
            public int Version { get; set; }
            public Guid CorrelationId { get; set; }
        }
    }
}
