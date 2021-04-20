namespace MassTransit.Containers.Tests.Common_Tests
{
    using System.Threading.Tasks;
    using GreenPipes;
    using NUnit.Framework;
    using Saga;
    using TestFramework;
    using TestFramework.Sagas;


    public abstract class Common_SagaStateMachine :
        InMemoryTestFixture
    {
        protected abstract IBusRegistrationContext Registration { get; }

        [Test]
        public async Task Should_handle_the_first_event()
        {
            Task<ConsumeContext<TestStarted>> started = await ConnectPublishHandler<TestStarted>();
            Task<ConsumeContext<TestUpdated>> updated = await ConnectPublishHandler<TestUpdated>();

            await InputQueueSendEndpoint.Send(new StartTest
            {
                CorrelationId = NewId.NextGuid(),
                TestKey = "Unique"
            });

            await started;

            await InputQueueSendEndpoint.Send(new UpdateTest {TestKey = "Unique"});

            await updated;
        }

        protected void ConfigureRegistration(IBusRegistrationConfigurator configurator)
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

        protected abstract IBusRegistrationContext Registration { get; }

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

        protected override void ConfigureInMemoryReceiveEndpoint(IInMemoryReceiveEndpointConfigurator configurator)
        {
            configurator.ConfigureSaga<TestInstance>(Registration);
        }

        protected void ConfigureRegistration(IBusRegistrationConfigurator configurator)
        {
            configurator.AddSagaStateMachine<TestStateMachineSaga, TestInstance>()
                .InMemoryRepository();
            configurator.AddBus(provider => BusControl);
        }


        protected class ScopedFilter<T> :
            IFilter<ConsumeContext<T>>
            where T : class
        {
            readonly MyId _myId;
            readonly TaskCompletionSource<MyId> _taskCompletionSource;

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


    public abstract class Common_StateMachine_FilterOrder :
        InMemoryTestFixture
    {
        protected readonly TaskCompletionSource<ConsumeContext<StartTest>> MessageCompletion;
        protected readonly TaskCompletionSource<SagaConsumeContext<TestInstance, StartTest>> SagaMessageCompletion;
        protected readonly TaskCompletionSource<SagaConsumeContext<TestInstance>> SagaCompletion;

        protected Common_StateMachine_FilterOrder()
        {
            MessageCompletion = GetTask<ConsumeContext<StartTest>>();
            SagaCompletion = GetTask<SagaConsumeContext<TestInstance>>();
            SagaMessageCompletion = GetTask<SagaConsumeContext<TestInstance, StartTest>>();
        }

        protected abstract IBusRegistrationContext Registration { get; }

        [Test]
        public async Task Should_include_container_scope()
        {
            await InputQueueSendEndpoint.Send(new StartTest
            {
                CorrelationId = NewId.NextGuid(),
                TestKey = "Unique"
            });

            await MessageCompletion.Task;

            await SagaCompletion.Task;

            await SagaMessageCompletion.Task;
        }

        protected abstract IFilter<SagaConsumeContext<TestInstance>> CreateSagaFilter();
        protected abstract IFilter<SagaConsumeContext<TestInstance, StartTest>> CreateSagaMessageFilter();
        protected abstract IFilter<ConsumeContext<StartTest>> CreateMessageFilter();

        protected override void ConfigureInMemoryReceiveEndpoint(IInMemoryReceiveEndpointConfigurator configurator)
        {
            configurator.ConfigureSaga<TestInstance>(Registration, x =>
            {
                x.Message<StartTest>(m => m.UseFilter(CreateMessageFilter()));

                x.UseFilter(CreateSagaFilter());

                x.SagaMessage<StartTest>(m => m.UseFilter(CreateSagaMessageFilter()));
            });
        }

        protected void ConfigureRegistration(IBusRegistrationConfigurator configurator)
        {
            configurator.AddSagaStateMachine<TestStateMachineSaga, TestInstance>()
                .InMemoryRepository();
            configurator.AddBus(provider => BusControl);
        }


        protected class SagaFilter<TInstance, TScope> :
            IFilter<SagaConsumeContext<TInstance>>
            where TInstance : class, ISaga
            where TScope : class
        {
            readonly TaskCompletionSource<SagaConsumeContext<TInstance>> _taskCompletionSource;

            public SagaFilter(TaskCompletionSource<SagaConsumeContext<TInstance>> taskCompletionSource)
            {
                _taskCompletionSource = taskCompletionSource;
            }

            public Task Send(SagaConsumeContext<TInstance> context, IPipe<SagaConsumeContext<TInstance>> next)
            {
                if (context.TryGetPayload(out TScope _))
                    _taskCompletionSource.TrySetResult(context);
                else
                    _taskCompletionSource.TrySetException(new PayloadException("Service Provider not found"));

                return next.Send(context);
            }

            public void Probe(ProbeContext context)
            {
            }
        }


        protected class SagaMessageFilter<TInstance, TMessage, TScope> :
            IFilter<SagaConsumeContext<TInstance, TMessage>>
            where TInstance : class, ISaga
            where TMessage : class
            where TScope : class
        {
            readonly TaskCompletionSource<SagaConsumeContext<TInstance, TMessage>> _taskCompletionSource;

            public SagaMessageFilter(TaskCompletionSource<SagaConsumeContext<TInstance, TMessage>> taskCompletionSource)
            {
                _taskCompletionSource = taskCompletionSource;
            }

            public Task Send(SagaConsumeContext<TInstance, TMessage> context, IPipe<SagaConsumeContext<TInstance, TMessage>> next)
            {
                if (context.TryGetPayload(out TScope _))
                    _taskCompletionSource.TrySetResult(context);
                else
                    _taskCompletionSource.TrySetException(new PayloadException("Service Provider not found"));

                return next.Send(context);
            }

            public void Probe(ProbeContext context)
            {
            }
        }


        protected class MessageFilter<TMessage, TScope> :
            IFilter<ConsumeContext<TMessage>>
            where TMessage : class
            where TScope : class
        {
            readonly TaskCompletionSource<ConsumeContext<TMessage>> _taskCompletionSource;

            public MessageFilter(TaskCompletionSource<ConsumeContext<TMessage>> taskCompletionSource)
            {
                _taskCompletionSource = taskCompletionSource;
            }

            public Task Send(ConsumeContext<TMessage> context, IPipe<ConsumeContext<TMessage>> next)
            {
                if (context.TryGetPayload(out TScope _))
                    _taskCompletionSource.TrySetException(new PayloadException("Service Provider should not be present"));
                else
                    _taskCompletionSource.TrySetResult(context);

                return next.Send(context);
            }

            public void Probe(ProbeContext context)
            {
            }
        }
    }
}
