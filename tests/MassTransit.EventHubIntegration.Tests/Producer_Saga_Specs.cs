namespace MassTransit.EventHubIntegration.Tests
{
    using System;
    using System.Threading.Tasks;
    using Contracts;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.DependencyInjection.Extensions;
    using Microsoft.Extensions.Logging;
    using NUnit.Framework;
    using TestFramework;
    using TestFramework.Sagas;


    public class Saga_Producer_Specs :
        InMemoryTestFixture
    {
        [Test]
        public async Task Should_produce()
        {
            TaskCompletionSource<ConsumeContext<EventHubMessage>> taskCompletionSource = GetTask<ConsumeContext<EventHubMessage>>();
            var services = new ServiceCollection();
            services.AddSingleton(taskCompletionSource);

            services.TryAddSingleton<ILoggerFactory>(LoggerFactory);
            services.TryAddSingleton(typeof(ILogger<>), typeof(Logger<>));

            services.AddMassTransit(x =>
            {
                x.AddSagaStateMachine<TestStateMachineSaga, TestInstance>()
                    .InMemoryRepository();

                x.UsingInMemory((context, cfg) => cfg.ConfigureEndpoints(context));
                x.AddRider(rider =>
                {
                    rider.AddConsumer<EventHubMessageConsumer>();

                    rider.UsingEventHub((context, k) =>
                    {
                        k.Host(Configuration.EventHubNamespace);
                        k.Storage(Configuration.StorageAccount);

                        k.ReceiveEndpoint(Configuration.EventHubName, c =>
                        {
                            c.ConfigureConsumer<EventHubMessageConsumer>(context);
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

                ConsumeContext<EventHubMessage> result = await taskCompletionSource.Task;

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


        class EventHubMessageConsumer :
            IConsumer<EventHubMessage>
        {
            readonly TaskCompletionSource<ConsumeContext<EventHubMessage>> _taskCompletionSource;

            public EventHubMessageConsumer(TaskCompletionSource<ConsumeContext<EventHubMessage>> taskCompletionSource)
            {
                _taskCompletionSource = taskCompletionSource;
            }

            public async Task Consume(ConsumeContext<EventHubMessage> context)
            {
                _taskCompletionSource.TrySetResult(context);
            }
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
                        .Produce(x => Configuration.EventHubName, x => x.Init<EventHubMessage>(new { Text = $"Key: {x.Data.TestKey}" }))
                        .TransitionTo(Active));

                SetCompletedWhenFinalized();
            }

            public State Active { get; private set; }

            public Event<StartTest> Started { get; private set; }
        }
    }
}
