namespace MassTransit.Containers.Tests
{
    using System;
    using System.Threading.Tasks;
    using Automatonymous;
    using Microsoft.Extensions.DependencyInjection;
    using NUnit.Framework;
    using TestFramework;
    using Testing;


    public interface StartCommand
    {
        public Guid CorrelationId { get; set; }
    }


    public interface StartCommandResponse
    {
        public Guid CorrelationId { get; set; }
    }


    [TestFixture]
    public class Container_based_tests_with_request_client
    {
        [Test]
        public async Task Using_DI_should_not_timeout()
        {
            var provider = new ServiceCollection()
                .AddMassTransitInMemoryTestHarness(cfg =>
                {
                    cfg.AddSagaStateMachine<TestSagaStateMachine, TestSaga>()
                        .InMemoryRepository();

                    cfg.AddSagaStateMachineTestHarness<TestSagaStateMachine, TestSaga>();
                    cfg.AddPublishMessageScheduler();
                    cfg.AddRequestClient<StartCommand>();
                })
                .BuildServiceProvider(true);

            var harness = provider.GetRequiredService<InMemoryTestHarness>();
            harness.TestTimeout = TimeSpan.FromSeconds(15);
            harness.OnConfigureBus += x => BusTestFixture.ConfigureBusDiagnostics(x);
            await harness.Start();

            // Act
            try
            {
                using var scope = provider.CreateScope();
                IRequestClient<StartCommand> client = scope.ServiceProvider.GetRequiredService<IRequestClient<StartCommand>>();
                Response<StartCommandResponse> response = await client.GetResponse<StartCommandResponse>(
                    new
                    {
                        CorrelationId = InVar.Id,
                    });

                // Assert
                // did the actual saga consume the message
                var sagaHarness = provider.GetRequiredService<IStateMachineSagaTestHarness<TestSaga, TestSagaStateMachine>>();
                Assert.True(await sagaHarness.Consumed.Any<StartCommand>());
            }
            finally
            {
                await harness.Stop();
            }
        }

        [Test]
        public async Task Should_not_timeout()
        {
            var harness = new InMemoryTestHarness();

            var machine = new TestSagaStateMachine();

            StateMachineSagaTestHarness<TestSaga, TestSagaStateMachine> sagaHarness = harness.StateMachineSaga<TestSaga, TestSagaStateMachine>(machine);

            harness.TestTimeout = TimeSpan.FromSeconds(15);
            harness.OnConfigureBus += x => BusTestFixture.ConfigureBusDiagnostics(x);
            await harness.Start();

            // Act
            try
            {
                IRequestClient<StartCommand> client = harness.CreateRequestClient<StartCommand>();
                Response<StartCommandResponse> response = await client.GetResponse<StartCommandResponse>(
                    new
                    {
                        CorrelationId = InVar.Id,
                    });

                // Assert
                // did the actual saga consume the message
                Assert.True(await sagaHarness.Consumed.Any<StartCommand>());
            }
            finally
            {
                await harness.Stop();
            }
        }


        class TestSaga : SagaStateMachineInstance
        {
            public string State { get; set; }
            public Guid CorrelationId { get; set; }
        }


        class TestSagaStateMachine : MassTransitStateMachine<TestSaga>
        {
            public TestSagaStateMachine()
            {
                InstanceState(x => x.State);

                Initially(
                    When(Start)
                        .TransitionTo(Started)
                        .RespondAsync(ctx => ctx.Init<StartCommandResponse>(new
                        {
                            ctx.Instance.CorrelationId,
                        })));
            }

            public Event<StartCommand> Start { get; private set; }

            public State Started { get; private set; }
        }
    }
}
