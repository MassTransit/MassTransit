namespace MassTransit.Containers.Tests.Common_Tests
{
    using System.Threading.Tasks;
    using GreenPipes;
    using NUnit.Framework;
    using Scenarios;
    using TestFramework;
    using TestFramework.Messages;


    public abstract class Common_ScopeRequestClient<TScope> :
        InMemoryTestFixture
        where TScope : class
    {
        readonly TaskCompletionSource<SendContext> _taskCompletionSource;

        protected Common_ScopeRequestClient()
        {
            _taskCompletionSource = GetTask<SendContext>();
        }

        [Test]
        public async Task Should_contains_scope_on_send()
        {
            var client = GetSendRequestClient();
            await client.GetResponse<ISimpleMessageResponse>(new SimpleMessageClass("test"));

            SendContext sent = await _taskCompletionSource.Task;

            Assert.IsTrue(sent.TryGetPayload<TScope>(out var scope));
            AssertScopesAreEqual(scope);
        }

        [Test]
        public async Task Should_contains_scope_on_publish()
        {
            var client = GetPublishRequestClient();
            await client.GetResponse<PongMessage>(new PingMessage(NewId.NextGuid()));

            SendContext sent = await _taskCompletionSource.Task;

            Assert.IsTrue(sent.TryGetPayload<TScope>(out var scope));
            AssertScopesAreEqual(scope);
        }

        protected abstract IRequestClient<SimpleMessageClass> GetSendRequestClient();
        protected abstract IRequestClient<PingMessage> GetPublishRequestClient();
        protected abstract void AssertScopesAreEqual(TScope actual);

        protected override void ConfigureInMemoryReceiveEndpoint(IInMemoryReceiveEndpointConfigurator configurator)
        {
            Handler<SimpleMessageClass>(configurator, context => context.RespondAsync<ISimpleMessageResponse>(new { }));
            Handler<PingMessage>(configurator, context => context.RespondAsync<PongMessage>(new { }));
        }

        protected override void ConfigureInMemoryBus(IInMemoryBusFactoryConfigurator configurator)
        {
            configurator.ConfigureSend(cfg => cfg.UseFilter(new TestScopeFilter(_taskCompletionSource)));
            configurator.ConfigurePublish(cfg => cfg.UseFilter(new TestScopeFilter(_taskCompletionSource)));
        }


        public interface ISimpleMessageResponse
        {
        }


        class TestScopeFilter :
            IFilter<SendContext>,
            IFilter<PublishContext>
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
