namespace MassTransit.Containers.Tests.Common_Tests
{
    using System;
    using System.Threading.Tasks;
    using NUnit.Framework;
    using Scenarios;
    using TestFramework;
    using TestFramework.Messages;


    public class Using_the_request_client_to_publish_within_a_scope<TContainer> :
        CommonContainerTestFixture<TContainer>
        where TContainer : ITestFixtureContainerFactory, new()
    {
        [Test]
        public async Task Should_contains_scope_on_publish()
        {
            IRequestClient<PingMessage> client = GetRequestClient<PingMessage>();
            await client.GetResponse<PongMessage>(new PingMessage(NewId.NextGuid()));

            var sent = await _taskCompletionSource.Task;

            Assert.IsTrue(sent.TryGetPayload<IServiceProvider>(out var serviceProvider));

            Assert.AreEqual(serviceProvider, ServiceScope.ServiceProvider);
        }

        readonly TaskCompletionSource<SendContext> _taskCompletionSource;

        public Using_the_request_client_to_publish_within_a_scope()
        {
            _taskCompletionSource = GetTask<SendContext>();
        }

        protected override void ConfigureMassTransit(IBusRegistrationConfigurator configurator)
        {
            configurator.AddRequestClient<PingMessage>();
        }

        protected override void ConfigureInMemoryReceiveEndpoint(IInMemoryReceiveEndpointConfigurator configurator)
        {
            Handler<PingMessage>(configurator, context => context.RespondAsync<PongMessage>(new { }));
        }

        protected override void ConfigureInMemoryBus(IInMemoryBusFactoryConfigurator configurator)
        {
            configurator.ConfigureSend(cfg => cfg.UseFilter(new TestRequestClientScopeFilter(_taskCompletionSource)));
            configurator.ConfigurePublish(cfg => cfg.UseFilter(new TestRequestClientScopeFilter(_taskCompletionSource)));
        }
    }


    public class Using_the_request_client_to_send_within_a_scope<TContainer> :
        CommonContainerTestFixture<TContainer>
        where TContainer : ITestFixtureContainerFactory, new()
    {
        [Test]
        public async Task Should_contains_scope_on_send()
        {
            IRequestClient<SimpleMessageClass> client = GetRequestClient<SimpleMessageClass>();
            await client.GetResponse<ISimpleMessageResponse>(new SimpleMessageClass("test"));

            var sent = await _taskCompletionSource.Task;

            Assert.IsTrue(sent.TryGetPayload<IServiceProvider>(out var serviceProvider));

            Assert.AreEqual(serviceProvider, ServiceScope.ServiceProvider);
        }

        readonly TaskCompletionSource<SendContext> _taskCompletionSource;

        public Using_the_request_client_to_send_within_a_scope()
        {
            _taskCompletionSource = GetTask<SendContext>();
        }

        protected override void ConfigureMassTransit(IBusRegistrationConfigurator configurator)
        {
            configurator.AddRequestClient<SimpleMessageClass>(InputQueueAddress);
        }

        protected override void ConfigureInMemoryReceiveEndpoint(IInMemoryReceiveEndpointConfigurator configurator)
        {
            Handler<SimpleMessageClass>(configurator, context => context.RespondAsync<ISimpleMessageResponse>(new { }));
        }

        protected override void ConfigureInMemoryBus(IInMemoryBusFactoryConfigurator configurator)
        {
            configurator.ConfigureSend(cfg => cfg.UseFilter(new TestRequestClientScopeFilter(_taskCompletionSource)));
            configurator.ConfigurePublish(cfg => cfg.UseFilter(new TestRequestClientScopeFilter(_taskCompletionSource)));
        }
    }


    public interface ISimpleMessageResponse
    {
    }


    class TestRequestClientScopeFilter :
        IFilter<SendContext>,
        IFilter<PublishContext>
    {
        readonly TaskCompletionSource<SendContext> _taskCompletionSource;

        public TestRequestClientScopeFilter(TaskCompletionSource<SendContext> taskCompletionSource)
        {
            _taskCompletionSource = taskCompletionSource;
        }

        public async Task Send(PublishContext context, IPipe<PublishContext> next)
        {
            _taskCompletionSource.TrySetResult(context);
            await next.Send(context);
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
