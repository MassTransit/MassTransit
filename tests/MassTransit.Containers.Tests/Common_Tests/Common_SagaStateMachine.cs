namespace MassTransit.Containers.Tests.Common_Tests
{
    using System.Threading.Tasks;
    using GreenPipes;
    using NUnit.Framework;
    using TestFramework;
    using TestFramework.Sagas;


    public abstract class Common_SagaStateMachine :
        InMemoryTestFixture
    {
        protected abstract IRegistration Registration { get; }

        [Test]
        public async Task Should_handle_the_first_event()
        {
            Task<ConsumeContext<TestStarted>> started = ConnectPublishHandler<TestStarted>();
            Task<ConsumeContext<TestUpdated>> updated = ConnectPublishHandler<TestUpdated>();

            await InputQueueSendEndpoint.Send(new StartTest
            {
                CorrelationId = NewId.NextGuid(),
                TestKey = "Unique"
            });

            await started;

            await InputQueueSendEndpoint.Send(new UpdateTest {TestKey = "Unique"});

            await updated;
        }

        protected void ConfigureRegistration<T>(IRegistrationConfigurator<T> configurator)
            where T : class
        {
            configurator.AddSagaStateMachine<TestStateMachineSaga, TestInstance>()
                .InMemoryRepository();

            configurator.AddBus(provider => BusControl);
        }

        protected override void ConfigureInMemoryReceiveEndpoint(IInMemoryReceiveEndpointConfigurator configurator)
        {
            configurator.ConfigureSaga<TestInstance>(Registration);
        }
    }

     public abstract class Common_StateMachine_Filter :
        InMemoryTestFixture
    {
        protected readonly TaskCompletionSource<MyId> TaskCompletionSource;

        protected Common_StateMachine_Filter()
        {
            TaskCompletionSource = GetTask<MyId>();
        }

        [Test]
        public async Task Should_use_scope()
        {
            await InputQueueSendEndpoint.Send(new StartTest
            {
                CorrelationId = NewId.NextGuid(),
                TestKey = "Unique"
            });

            var result = await TaskCompletionSource.Task;
            Assert.NotNull(result);
        }

        protected override void ConfigureInMemoryBus(IInMemoryBusFactoryConfigurator configurator)
        {
            ConfigureFilter(configurator);
        }

        protected abstract void ConfigureFilter(IConsumePipeConfigurator configurator);
        protected abstract IRegistration Registration { get; }

        protected override void ConfigureInMemoryReceiveEndpoint(IInMemoryReceiveEndpointConfigurator configurator)
        {
            configurator.ConfigureSaga<TestInstance>(Registration);
        }
        protected void ConfigureRegistration<T>(IRegistrationConfigurator<T> configurator)
            where T : class
        {
            configurator.AddSagaStateMachine<TestStateMachineSaga, TestInstance>()
                .InMemoryRepository();
            configurator.AddBus(provider => BusControl);
        }

        protected class ScopedFilter<T> :
            IFilter<ConsumeContext<T>>
            where T : class
        {
            readonly TaskCompletionSource<MyId> _taskCompletionSource;
            readonly MyId _myId;

            public ScopedFilter(TaskCompletionSource<MyId> taskCompletionSource, MyId myId)
            {
                _taskCompletionSource = taskCompletionSource;
                _myId = myId;
            }

            public Task Send(ConsumeContext<T> context, IPipe<ConsumeContext<T>> next)
            {
                _taskCompletionSource.TrySetResult(_myId);
                return next.Send(context);
            }

            public void Probe(ProbeContext context)
            {
            }
        }
    }
}
