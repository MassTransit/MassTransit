namespace MassTransit.Containers.Tests.Common_Tests
{
    using System;
    using System.Threading.Tasks;
    using Microsoft.Extensions.DependencyInjection;
    using NUnit.Framework;
    using Scenarios;
    using TestFramework;


    [TestFixture(typeof(DependencyInjectionTestFixtureContainerFactory))]
    public class Common_ScopeSend<TContainer> :
        InMemoryContainerTestFixture<TContainer>
        where TContainer : ITestFixtureContainerFactory, new()
    {
        [Test]
        public async Task Should_contains_scope_on_send()
        {
            var endpoint = await SendEndpointProvider.GetSendEndpoint(InputQueueAddress);
            await endpoint.Send(new SimpleMessageClass("test"));

            var sent = await _taskCompletionSource.Task;

            Assert.IsTrue(sent.TryGetPayload<IServiceScope>(out var scope));

            Assert.AreEqual(scope.ServiceProvider, ServiceScope.ServiceProvider);
        }

        readonly TaskCompletionSource<SendContext> _taskCompletionSource;

        public Common_ScopeSend()
        {
            _taskCompletionSource = GetTask<SendContext>();
        }

        ISendEndpointProvider SendEndpointProvider => ServiceScope.ServiceProvider.GetRequiredService<ISendEndpointProvider>();

        protected override void ConfigureInMemoryReceiveEndpoint(IInMemoryReceiveEndpointConfigurator configurator)
        {
            Handled<SimpleMessageClass>(configurator);
        }

        protected override void ConfigureInMemoryBus(IInMemoryBusFactoryConfigurator configurator)
        {
            configurator.ConfigureSend(cfg => cfg.UseFilter(new TestScopeFilter(_taskCompletionSource)));
        }
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


    public class Common_Send_Filter<TContainer> :
        CommonContainerTestFixture<TContainer>
        where TContainer : ITestFixtureContainerFactory, new()
    {
        [Test]
        public async Task Should_use_scope()
        {
            var endpoint = await SendEndpointProvider.GetSendEndpoint(InputQueueAddress);
            await endpoint.Send<SimpleMessageInterface>(new { Name = "test" });

            var result = await _taskCompletionSource.Task;
            Assert.AreEqual(MyId, result);
        }

        readonly TaskCompletionSource<MyId> _taskCompletionSource;

        public Common_Send_Filter()
        {
            _taskCompletionSource = GetTask<MyId>();
        }

        MyId MyId => ServiceScope.ServiceProvider.GetRequiredService<MyId>();

        ISendEndpointProvider SendEndpointProvider => ServiceScope.ServiceProvider.GetRequiredService<ISendEndpointProvider>();

        protected override void ConfigureMassTransit(IBusRegistrationConfigurator configurator)
        {
            configurator.AddConsumer<SimplerConsumer>();
        }

        protected override IServiceCollection ConfigureServices(IServiceCollection collection)
        {
            collection.AddScoped(_ => new MyId(Guid.NewGuid()));
            collection.AddSingleton(_taskCompletionSource);

            return collection;
        }

        protected override void ConfigureInMemoryBus(IInMemoryBusFactoryConfigurator configurator)
        {
            configurator.UseSendFilter(typeof(CommonSendScopedFilter<>), BusRegistrationContext);
        }

        protected override void ConfigureInMemoryReceiveEndpoint(IInMemoryReceiveEndpointConfigurator configurator)
        {
            configurator.ConfigureConsumer<SimplerConsumer>(BusRegistrationContext);
        }
    }


    class CommonSendScopedFilter<T> :
        IFilter<SendContext<T>>
        where T : class
    {
        readonly MyId _myId;
        readonly TaskCompletionSource<MyId> _taskCompletionSource;

        public CommonSendScopedFilter(TaskCompletionSource<MyId> taskCompletionSource, MyId myId)
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
