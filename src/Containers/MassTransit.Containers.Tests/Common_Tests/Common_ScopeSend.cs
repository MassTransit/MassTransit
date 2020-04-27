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

            SendContext sent = await _taskCompletionSource.Task;

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
}
