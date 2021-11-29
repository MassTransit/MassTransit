namespace MassTransit.Containers.Tests.Common_Tests
{
    using System;
    using System.Threading.Tasks;
    using Microsoft.Extensions.DependencyInjection;
    using NUnit.Framework;
    using TestFramework;
    using TestFramework.Sagas;


    public class Common_SagaStateMachine<TContainer> :
        CommonContainerTestFixture<TContainer>
        where TContainer : ITestFixtureContainerFactory, new()
    {
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

            await InputQueueSendEndpoint.Send(new UpdateTest { TestKey = "Unique" });

            await updated;
        }

        protected override IServiceCollection ConfigureServices(IServiceCollection collection)
        {
            return collection
                .AddScoped<PublishTestStartedActivity>();
        }

        protected override void ConfigureMassTransit(IBusRegistrationConfigurator configurator)
        {
            configurator.AddSagaStateMachine<TestStateMachineSaga, TestInstance>()
                .InMemoryRepository();
        }

        protected override void ConfigureInMemoryReceiveEndpoint(IInMemoryReceiveEndpointConfigurator configurator)
        {
            configurator.ConfigureSaga<TestInstance>(BusRegistrationContext);
        }
    }


    public class Common_StateMachine_Filter<TContainer> :
        CommonContainerTestFixture<TContainer>
        where TContainer : ITestFixtureContainerFactory, new()
    {
        [Test]
        public async Task Should_use_scope()
        {
            await InputQueueSendEndpoint.Send(new StartTest
            {
                CorrelationId = NewId.NextGuid(),
                TestKey = "Unique"
            });

            var result = await _taskCompletionSource.Task;
            Assert.NotNull(result);
        }

        readonly TaskCompletionSource<MyId> _taskCompletionSource;

        public Common_StateMachine_Filter()
        {
            _taskCompletionSource = GetTask<MyId>();
        }

        protected override IServiceCollection ConfigureServices(IServiceCollection collection)
        {
            return collection
                .AddScoped(_ => new MyId(Guid.NewGuid()))
                .AddSingleton(_taskCompletionSource)
                .AddScoped(typeof(CommonSendScopedFilter<>));
        }

        protected override void ConfigureMassTransit(IBusRegistrationConfigurator configurator)
        {
            configurator.AddSagaStateMachine<TestStateMachineSaga, TestInstance>()
                .InMemoryRepository();
        }

        protected override void ConfigureInMemoryBus(IInMemoryBusFactoryConfigurator configurator)
        {
            configurator.UseConsumeFilter(typeof(CommonScopedConsumeFilter<>), BusRegistrationContext);
        }

        protected override void ConfigureInMemoryReceiveEndpoint(IInMemoryReceiveEndpointConfigurator configurator)
        {
            configurator.ConfigureSaga<TestInstance>(BusRegistrationContext);
        }
    }


    public class Common_StateMachine_FilterOrder<TContainer> :
        CommonContainerTestFixture<TContainer>
        where TContainer : ITestFixtureContainerFactory, new()
    {
        [Test]
        public async Task Should_include_container_scope()
        {
            await InputQueueSendEndpoint.Send(new StartTest
            {
                CorrelationId = NewId.NextGuid(),
                TestKey = "Unique"
            });

            await _messageCompletion.Task;

            await _sagaCompletion.Task;

            await _sagaMessageCompletion.Task;
        }

        readonly TaskCompletionSource<ConsumeContext<StartTest>> _messageCompletion;
        readonly TaskCompletionSource<SagaConsumeContext<TestInstance>> _sagaCompletion;
        readonly TaskCompletionSource<SagaConsumeContext<TestInstance, StartTest>> _sagaMessageCompletion;

        public Common_StateMachine_FilterOrder()
        {
            _messageCompletion = GetTask<ConsumeContext<StartTest>>();
            _sagaCompletion = GetTask<SagaConsumeContext<TestInstance>>();
            _sagaMessageCompletion = GetTask<SagaConsumeContext<TestInstance, StartTest>>();
        }

        IFilter<SagaConsumeContext<TestInstance, StartTest>> CreateSagaMessageFilter()
        {
            return ServiceProvider.GetRequiredService<IFilter<SagaConsumeContext<TestInstance, StartTest>>>();
        }

        IFilter<SagaConsumeContext<TestInstance>> CreateSagaFilter()
        {
            return ServiceProvider.GetRequiredService<IFilter<SagaConsumeContext<TestInstance>>>();
        }

        IFilter<ConsumeContext<StartTest>> CreateMessageFilter()
        {
            return ServiceProvider.GetRequiredService<IFilter<ConsumeContext<StartTest>>>();
        }

        protected override IServiceCollection ConfigureServices(IServiceCollection collection)
        {
            return collection.AddSingleton(_messageCompletion)
                .AddSingleton(_sagaCompletion)
                .AddSingleton(_sagaMessageCompletion)
                .AddSingleton<IFilter<ConsumeContext<StartTest>>, MessageFilter<StartTest, IServiceProvider>>()
                .AddSingleton<IFilter<SagaConsumeContext<TestInstance>>, SagaFilter<TestInstance, IServiceProvider>>()
                .AddSingleton<IFilter<SagaConsumeContext<TestInstance, StartTest>>, SagaMessageFilter<TestInstance, StartTest, IServiceProvider>>();
        }

        protected override void ConfigureMassTransit(IBusRegistrationConfigurator configurator)
        {
            configurator.AddSagaStateMachine<TestStateMachineSaga, TestInstance>()
                .InMemoryRepository();
        }

        protected override void ConfigureInMemoryReceiveEndpoint(IInMemoryReceiveEndpointConfigurator configurator)
        {
            configurator.ConfigureSaga<TestInstance>(BusRegistrationContext, x =>
            {
                x.Message<StartTest>(m => m.UseFilter(CreateMessageFilter()));

                x.UseFilter(CreateSagaFilter());

                x.SagaMessage<StartTest>(m => m.UseFilter(CreateSagaMessageFilter()));
            });
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
