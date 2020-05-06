namespace MassTransit.Containers.Tests.DependencyInjection_Tests
{
    using System;
    using System.Threading.Tasks;
    using ExtensionsDependencyInjectionIntegration;
    using Microsoft.Extensions.DependencyInjection;
    using NUnit.Framework;
    using Scenarios;
    using TestFramework;
    using TestFramework.Messages;


    [TestFixture]
    public class DependencyInjection_MultipleBus :
        InMemoryTestFixture
    {
        readonly IServiceProvider _provider;
        readonly TaskCompletionSource<ConsumeContext<SimpleMessageInterface>> _task1;
        readonly TaskCompletionSource<ConsumeContext<PingMessage>> _task2;

        public DependencyInjection_MultipleBus()
        {
            _task1 = GetTask<ConsumeContext<SimpleMessageInterface>>();
            _task2 = GetTask<ConsumeContext<PingMessage>>();

            var collection = new ServiceCollection();

            collection.AddSingleton(_task1);
            collection.AddSingleton(_task2);

            collection.AddMassTransit<Bus1>(x =>
            {
                x.AddConsumer<Consumer1>();
                x.AddBus(provider => MassTransit.Bus.Factory.CreateUsingInMemory(cfg =>
                {
                    cfg.ConfigureEndpoints(provider, x.Name);
                }));
            });

            collection.AddMassTransit<Bus2>(x =>
            {
                x.AddConsumer<Consumer2>();
                x.AddBus(provider => MassTransit.Bus.Factory.CreateUsingInMemory(cfg =>
                {
                    cfg.ConfigureEndpoints(provider, x.Name);
                }));
            });

            _provider = collection.BuildServiceProvider(true);
        }

        [OneTimeSetUp]
        public async Task Setup()
        {
            await _provider.GetService<IBusControl<Bus1>>().StartAsync();
            await _provider.GetService<IBusControl<Bus2>>().StartAsync();
        }

        [OneTimeTearDown]
        public async Task TearDown()
        {
            await _provider.GetService<IBusControl<Bus1>>().StopAsync();
            await _provider.GetService<IBusControl<Bus2>>().StopAsync();
        }

        [Test]
        public async Task Should_receive()
        {
            var publishEndpoint = _provider.GetService<IBus<Bus1>>();
            await publishEndpoint.Publish<SimpleMessageInterface>(new SimpleMessageClass("abc"));

            await _task1.Task;
            await _task2.Task;
        }


        class Consumer1 :
            IConsumer<SimpleMessageInterface>
        {
            readonly IPublishEndpoint<Bus2> _publishEndpoint;
            readonly TaskCompletionSource<ConsumeContext<SimpleMessageInterface>> _taskCompletionSource;

            public Consumer1(IPublishEndpoint publishEndpointDefault, IPublishEndpoint<Bus2> publishEndpoint,
                TaskCompletionSource<ConsumeContext<SimpleMessageInterface>> taskCompletionSource)
            {
                _publishEndpoint = publishEndpoint;
                _taskCompletionSource = taskCompletionSource;
            }

            public async Task Consume(ConsumeContext<SimpleMessageInterface> context)
            {
                _taskCompletionSource.TrySetResult(context);
                await _publishEndpoint.Publish(new PingMessage());
            }
        }


        class Consumer2 :
            IConsumer<PingMessage>
        {
            readonly TaskCompletionSource<ConsumeContext<PingMessage>> _taskCompletionSource;

            public Consumer2(IPublishEndpoint publishEndpoint, TaskCompletionSource<ConsumeContext<PingMessage>> taskCompletionSource)
            {
                _taskCompletionSource = taskCompletionSource;
            }

            public async Task Consume(ConsumeContext<PingMessage> context)
            {
                _taskCompletionSource.TrySetResult(context);
            }
        }


        interface Bus1
        {
        }


        interface Bus2
        {
        }
    }
}
