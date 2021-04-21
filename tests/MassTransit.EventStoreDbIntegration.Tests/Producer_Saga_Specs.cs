namespace MassTransit.EventStoreDbIntegration.Tests
{
    using System;
    using System.Threading.Tasks;
    using Automatonymous;
    using EventStore.Client;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.DependencyInjection.Extensions;
    using Microsoft.Extensions.Logging;
    using NUnit.Framework;
    using TestFramework;
    using TestFramework.Sagas;


    public class Producer_Saga_Specs :
        InMemoryTestFixture
    {
        public const string SubscriptionName = "mt_producer_saga_specs_test";
        public const string ProducerStreamName = "mt_producer_saga_specs";

        [Test]
        public async Task Should_produce()
        {
            TaskCompletionSource<ConsumeContext<EventStoreDbMessage>> taskCompletionSource = GetTask<ConsumeContext<EventStoreDbMessage>>();
            var services = new ServiceCollection();
            services.AddSingleton(taskCompletionSource);

            services.TryAddSingleton<ILoggerFactory>(LoggerFactory);
            services.TryAddSingleton(typeof(ILogger<>), typeof(Logger<>));

            _ = services.AddSingleton<EventStoreClient>((provider) =>
            {
                var settings = EventStoreClientSettings.Create("esdb://localhost:2113?tls=false");
                settings.ConnectionName = "MassTransit Test Connection";

                return new EventStoreClient(settings);
            });

            services.AddMassTransit(x =>
            {
                x.AddSagaStateMachine<TestStateMachineSaga, TestInstance>()
                    .InMemoryRepository();

                x.UsingInMemory((context, cfg) => cfg.ConfigureEndpoints(context));
                x.AddRider(rider =>
                {
                    rider.AddConsumer<EventStoreDbMessageConsumer>();

                    rider.UsingEventStoreDB((context, esdb) =>
                    {
                        esdb.CatchupSubscription(StreamName.Custom(ProducerStreamName), SubscriptionName, c =>
                        {
                            c.UseEventStoreDBCheckpointStore(StreamName.ForCheckpoint(SubscriptionName));

                            c.ConfigureConsumer<EventStoreDbMessageConsumer>(context);
                        });
                    });
                });
            });

            var provider = services.BuildServiceProvider(true);

            var busControl = provider.GetRequiredService<IBusControl>();

            await busControl.StartAsync(TestCancellationToken);

            var serviceScope = provider.CreateScope();

            var publishEndpoint = serviceScope.ServiceProvider.GetRequiredService<IPublishEndpoint>();

            try
            {
                var correlationId = NewId.NextGuid();

                await publishEndpoint.Publish(new StartTest
                {
                    CorrelationId = correlationId,
                    TestKey = "ABC123"
                }, TestCancellationToken);

                ConsumeContext<EventStoreDbMessage> result = await taskCompletionSource.Task;

                Assert.AreEqual("Key: ABC123", result.Message.Text);
                Assert.AreEqual(correlationId, result.InitiatorId);
            }
            finally
            {
                serviceScope.Dispose();

                await busControl.StopAsync(TestCancellationToken);

                await provider.DisposeAsync();
            }
        }


        class EventStoreDbMessageConsumer :
            IConsumer<EventStoreDbMessage>
        {
            readonly TaskCompletionSource<ConsumeContext<EventStoreDbMessage>> _taskCompletionSource;

            public EventStoreDbMessageConsumer(TaskCompletionSource<ConsumeContext<EventStoreDbMessage>> taskCompletionSource)
            {
                _taskCompletionSource = taskCompletionSource;
            }

            public async Task Consume(ConsumeContext<EventStoreDbMessage> context)
            {
                _taskCompletionSource.TrySetResult(context);
            }
        }


        public interface EventStoreDbMessage
        {
            string Text { get; }
        }


        public class TestInstance :
            SagaStateMachineInstance
        {
            public State CurrentState { get; set; }
            public string Key { get; set; }
            public Guid CorrelationId { get; set; }
        }


        public class TestStateMachineSaga :
            MassTransitStateMachine<TestInstance>
        {
            public TestStateMachineSaga()
            {
                InstanceState(x => x.CurrentState);

                Initially(
                    When(Started)
                        .Then(context => context.Instance.Key = context.Data.TestKey)
                        .Produce(x => StreamName.Custom(Producer_Saga_Specs.ProducerStreamName),
                                 x => x.Init<EventStoreDbMessage>(new {Text = $"Key: {x.Data.TestKey}"}))
                        .TransitionTo(Active));

                SetCompletedWhenFinalized();
            }

            public State Active { get; private set; }

            public Event<StartTest> Started { get; private set; }
        }
    }
}
