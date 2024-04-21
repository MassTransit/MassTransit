namespace MassTransit.Tests.ContainerTests.Common_Tests
{
    using System;
    using System.Threading.Tasks;
    using Microsoft.Extensions.DependencyInjection;
    using NUnit.Framework;
    using Scenarios;
    using TestFramework;
    using TestFramework.Messages;


    public class Using_the_request_client_to_publish_within_a_scope :
        InMemoryContainerTestFixture
    {
        readonly TaskCompletionSource<SendContext> _taskCompletionSource;

        public Using_the_request_client_to_publish_within_a_scope()
        {
            _taskCompletionSource = GetTask<SendContext>();
        }

        [Test]
        public async Task Should_contains_scope_on_publish()
        {
            IRequestClient<PingMessage> client = GetRequestClient<PingMessage>();
            await client.GetResponse<PongMessage>(new PingMessage(NewId.NextGuid()));

            var sent = await _taskCompletionSource.Task;

            Assert.Multiple(() =>
            {
                Assert.That(sent.TryGetPayload<IServiceProvider>(out var serviceProvider), Is.True);

                Assert.That(ServiceScope.ServiceProvider, Is.EqualTo(serviceProvider));
            });
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


    public class Using_the_mediator_to_publish_within_a_scope :
        InMemoryContainerTestFixture
    {
        readonly TaskCompletionSource<SendContext> _taskCompletionSource;

        public Using_the_mediator_to_publish_within_a_scope()
        {
            _taskCompletionSource = GetTask<SendContext>();
        }

        [Test]
        public async Task Should_contains_scope_on_publish()
        {
            IRequestClient<PingMessage> client = GetRequestClient<PingMessage>();
            await client.GetResponse<PongMessage>(new PingMessage(NewId.NextGuid()));

            var sent = await _taskCompletionSource.Task;

            Assert.Multiple(() =>
            {
                Assert.That(sent.TryGetPayload<IServiceProvider>(out var serviceProvider), Is.True);

                Assert.That(ServiceScope.ServiceProvider, Is.EqualTo(serviceProvider));
            });
        }

        protected override IServiceCollection ConfigureServices(IServiceCollection collection)
        {
            return collection.AddMediator(ConfigureRegistration);
        }

        void ConfigureRegistration(IMediatorRegistrationConfigurator configurator)
        {
            configurator.AddRequestClient<PingMessage>();
            configurator.AddConsumer<Consumer>();
            configurator.ConfigureMediator(ConfigureMediator);
        }

        void ConfigureMediator(IMediatorRegistrationContext context, IMediatorConfigurator configurator)
        {
            configurator.ConfigureSend(cfg => cfg.UseFilter(new TestRequestClientScopeFilter(_taskCompletionSource)));
            configurator.ConfigurePublish(cfg => cfg.UseFilter(new TestRequestClientScopeFilter(_taskCompletionSource)));
        }


        class Consumer :
            IConsumer<PingMessage>
        {
            public Task Consume(ConsumeContext<PingMessage> context)
            {
                return context.RespondAsync<PongMessage>(new { });
            }
        }
    }


    public class Using_the_request_client_to_send_within_a_scope :
        InMemoryContainerTestFixture
    {
        readonly TaskCompletionSource<SendContext> _taskCompletionSource;

        public Using_the_request_client_to_send_within_a_scope()
        {
            _taskCompletionSource = GetTask<SendContext>();
        }

        [Test]
        public async Task Should_contains_scope_on_send()
        {
            IRequestClient<SimpleMessageClass> client = GetRequestClient<SimpleMessageClass>();
            await client.GetResponse<ISimpleMessageResponse>(new SimpleMessageClass("test"));

            var sent = await _taskCompletionSource.Task;

            Assert.Multiple(() =>
            {
                Assert.That(sent.TryGetPayload<IServiceProvider>(out var serviceProvider), Is.True);

                Assert.That(ServiceScope.ServiceProvider, Is.EqualTo(serviceProvider));
            });
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
