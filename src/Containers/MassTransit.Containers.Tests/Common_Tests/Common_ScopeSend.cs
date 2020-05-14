namespace MassTransit.Containers.Tests.Common_Tests
{
    using System.Threading.Tasks;
    using GreenPipes;
    using NUnit.Framework;
    using Scenarios;
    using TestFramework;


    public abstract class Common_ScopeSend<TScope> :
        InMemoryTestFixture
        where TScope : class
    {
        readonly TaskCompletionSource<SendContext> _taskCompletionSource;

        protected Common_ScopeSend()
        {
            _taskCompletionSource = GetTask<SendContext>();
        }

        [Test]
        public async Task Should_contains_scope_on_send()
        {
            var provider = GetSendEndpointProvider();
            var endpoint = await provider.GetSendEndpoint(InputQueueAddress);
            await endpoint.Send(new SimpleMessageClass("test"));

            var sent = await _taskCompletionSource.Task;

            Assert.IsTrue(sent.TryGetPayload<TScope>(out var scope));
            AssertScopesAreEqual(scope);
        }

        protected abstract ISendEndpointProvider GetSendEndpointProvider();
        protected abstract void AssertScopesAreEqual(TScope actual);

        protected override void ConfigureInMemoryReceiveEndpoint(IInMemoryReceiveEndpointConfigurator configurator)
        {
            Handled<SimpleMessageClass>(configurator);
        }

        protected override void ConfigureInMemoryBus(IInMemoryBusFactoryConfigurator configurator)
        {
            configurator.ConfigureSend(cfg => cfg.UseFilter(new TestScopeFilter(_taskCompletionSource)));
        }


        class TestScopeFilter :
            IFilter<SendContext>
        {
            readonly TaskCompletionSource<SendContext> _taskCompletionSource;

            public TestScopeFilter(TaskCompletionSource<SendContext> taskCompletionSource)
            {
                _taskCompletionSource = taskCompletionSource;
            }

            public async Task Send(SendContext context, IPipe<SendContext> next)
            {
                _taskCompletionSource.TrySetResult(context);
                await next.Send(context);
            }

            public void Probe(ProbeContext context)
            {
            }
        }
    }


    public abstract class Common_Send_Filter :
        InMemoryTestFixture
    {
        protected readonly TaskCompletionSource<MyId> TaskCompletionSource;

        protected Common_Send_Filter()
        {
            TaskCompletionSource = GetTask<MyId>();
        }

        [Test]
        public async Task Should_use_scope()
        {
            var endpoint = await SendEndpointProvider.GetSendEndpoint(InputQueueAddress);
            await endpoint.Send<SimpleMessageInterface>(new {Name = "test"});

            var result = await TaskCompletionSource.Task;
            Assert.AreEqual(MyId, result);
        }

        protected override void ConfigureInMemoryBus(IInMemoryBusFactoryConfigurator configurator)
        {
            ConfigureFilter(configurator);
        }

        protected abstract void ConfigureFilter(ISendPipelineConfigurator sendPipelineConfigurator);
        protected abstract IRegistration Registration { get; }

        protected abstract MyId MyId { get; }

        protected abstract ISendEndpointProvider SendEndpointProvider { get; }

        protected override void ConfigureInMemoryReceiveEndpoint(IInMemoryReceiveEndpointConfigurator configurator)
        {
            configurator.ConfigureConsumer<SimplerConsumer>(Registration);
        }

        protected void ConfigureRegistration<T>(IRegistrationConfigurator<T> configurator)
            where T : class
        {
            configurator.AddConsumer<SimplerConsumer>();
            configurator.AddBus(provider => BusControl);
        }


        protected class ScopedFilter<T> :
            IFilter<SendContext<T>>
            where T : class
        {
            readonly TaskCompletionSource<MyId> _taskCompletionSource;
            readonly MyId _myId;

            public ScopedFilter(TaskCompletionSource<MyId> taskCompletionSource, MyId myId)
            {
                _taskCompletionSource = taskCompletionSource;
                _myId = myId;
            }

            public Task Send(SendContext<T> context, IPipe<SendContext<T>> next)
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
