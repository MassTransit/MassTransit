namespace MassTransit.Containers.Tests.Common_Tests
{
    using System.Threading.Tasks;
    using GreenPipes;
    using NUnit.Framework;
    using Scenarios;
    using TestFramework;


    public abstract class Common_ScopePublish<TScope> :
        InMemoryTestFixture
        where TScope : class
    {
        readonly TaskCompletionSource<PublishContext> _taskCompletionSource;

        protected Common_ScopePublish()
        {
            _taskCompletionSource = GetTask<PublishContext>();
        }

        [Test]
        public async Task Should_contains_scope_on_publish()
        {
            var endpoint = GetPublishEndpoint();
            await endpoint.Publish(new SimpleMessageClass("test"));

            PublishContext published = await _taskCompletionSource.Task;

            Assert.IsTrue(published.TryGetPayload<TScope>(out var scope));
            AssertScopesAreEqual(scope);
        }

        protected abstract IPublishEndpoint GetPublishEndpoint();

        protected abstract void AssertScopesAreEqual(TScope actual);

        protected override void ConfigureInMemoryReceiveEndpoint(IInMemoryReceiveEndpointConfigurator configurator)
        {
            Handled<SimpleMessageClass>(configurator);
        }

        protected override void ConfigureInMemoryBus(IInMemoryBusFactoryConfigurator configurator)
        {
            configurator.ConfigurePublish(cfg => cfg.UseFilter(new TestScopeFilter(_taskCompletionSource)));
        }


        class TestScopeFilter :
            IFilter<PublishContext>
        {
            readonly TaskCompletionSource<PublishContext> _taskCompletionSource;

            public TestScopeFilter(TaskCompletionSource<PublishContext> taskCompletionSource)
            {
                _taskCompletionSource = taskCompletionSource;
            }

            public async Task Send(PublishContext context, IPipe<PublishContext> next)
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
