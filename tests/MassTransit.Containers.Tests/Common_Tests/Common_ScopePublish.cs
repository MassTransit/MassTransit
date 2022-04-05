namespace MassTransit.Containers.Tests.Common_Tests
{
    using System;
    using System.Threading.Tasks;
    using Internals;
    using Microsoft.Extensions.DependencyInjection;
    using NUnit.Framework;
    using Scenarios;
    using TestFramework;
    using Util;


    public class Common_ScopePublish<TContainer> :
        CommonContainerTestFixture<TContainer>
        where TContainer : ITestFixtureContainerFactory, new()
    {
        [Test]
        public async Task Should_contains_scope_on_publish()
        {
            await PublishEndpoint.Publish(new SimpleMessageClass("test"));

            var published = await _taskCompletionSource.Task;

            Assert.IsTrue(published.TryGetPayload<IServiceProvider>(out var serviceProvider));

            Assert.AreEqual(serviceProvider, ServiceScope.ServiceProvider);
        }

        readonly TaskCompletionSource<PublishContext> _taskCompletionSource;

        public Common_ScopePublish()
        {
            _taskCompletionSource = GetTask<PublishContext>();
        }

        IPublishEndpoint PublishEndpoint => ServiceScope.ServiceProvider.GetRequiredService<IPublishEndpoint>();

        protected override void ConfigureInMemoryReceiveEndpoint(IInMemoryReceiveEndpointConfigurator configurator)
        {
            Handled<SimpleMessageClass>(configurator);
        }

        protected override void ConfigureInMemoryBus(IInMemoryBusFactoryConfigurator configurator)
        {
            configurator.ConfigurePublish(cfg => cfg.UseFilter(new TestPublishContextFilter(_taskCompletionSource)));
        }
    }


    class TestPublishContextFilter :
        IFilter<PublishContext>
    {
        readonly TaskCompletionSource<PublishContext> _taskCompletionSource;

        public TestPublishContextFilter(TaskCompletionSource<PublishContext> taskCompletionSource)
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


    public class Common_Publish_Filter<TContainer> :
        CommonContainerTestFixture<TContainer>
        where TContainer : ITestFixtureContainerFactory, new()
    {
        [Test]
        public async Task Should_use_scope()
        {
            await PublishEndpoint.Publish<SimpleMessageInterface>(new { Name = "test" });

            var result = await TaskCompletionSource.Task;
            Assert.AreEqual(MyId, result);
        }

        protected readonly TaskCompletionSource<MyId> TaskCompletionSource;

        public Common_Publish_Filter()
        {
            TaskCompletionSource = GetTask<MyId>();
        }

        MyId MyId => ServiceScope.ServiceProvider.GetRequiredService<MyId>();

        IPublishEndpoint PublishEndpoint => ServiceScope.ServiceProvider.GetRequiredService<IPublishEndpoint>();

        protected override IServiceCollection ConfigureServices(IServiceCollection collection)
        {
            collection.AddScoped(_ => new MyId(Guid.NewGuid()));
            collection.AddSingleton(TaskCompletionSource);

            return collection;
        }

        protected override void ConfigureMassTransit(IBusRegistrationConfigurator configurator)
        {
            configurator.AddConsumer<SimplerConsumer>();
        }

        protected override void ConfigureInMemoryBus(IInMemoryBusFactoryConfigurator configurator)
        {
            configurator.UsePublishFilter(typeof(TestScopedPublishFilter<>), BusRegistrationContext);
        }

        protected override void ConfigureInMemoryReceiveEndpoint(IInMemoryReceiveEndpointConfigurator configurator)
        {
            configurator.ConfigureConsumer<SimplerConsumer>(BusRegistrationContext);
        }
    }


    class TestScopedPublishFilter<T> :
        IFilter<PublishContext<T>>
        where T : class
    {
        readonly MyId _myId;
        readonly TaskCompletionSource<MyId> _taskCompletionSource;

        public TestScopedPublishFilter(TaskCompletionSource<MyId> taskCompletionSource, MyId myId)
        {
            _taskCompletionSource = taskCompletionSource;
            _myId = myId;
        }

        public Task Send(PublishContext<T> context, IPipe<PublishContext<T>> next)
        {
            _taskCompletionSource.TrySetResult(_myId);
            return next.Send(context);
        }

        public void Probe(ProbeContext context)
        {
        }
    }


    public class Common_Publish_Filter_Outbox<TContainer> :
        CommonContainerTestFixture<TContainer>
        where TContainer : ITestFixtureContainerFactory, new()
    {
        [Test]
        public async Task Should_use_scope()
        {
            await InputQueueSendEndpoint.Send<SimpleMessageInterface>(new { Name = "test" });

            var myId = await MyIdSource.Task.OrCanceled(InMemoryTestHarness.InactivityToken);

            var consumer = await ConsumerSource.Task.OrCanceled(InMemoryTestHarness.InactivityToken);

            Assert.AreEqual(myId, consumer.MyId);
        }

        protected readonly TaskCompletionSource<ProducingConsumer> ConsumerSource;
        protected readonly TaskCompletionSource<MyId> MyIdSource;

        public Common_Publish_Filter_Outbox()
        {
            MyIdSource = GetTask<MyId>();
            ConsumerSource = GetTask<ProducingConsumer>();
        }

        protected override IServiceCollection ConfigureServices(IServiceCollection collection)
        {
            collection.AddScoped(_ => new MyId(Guid.NewGuid()));
            collection.AddSingleton(ConsumerSource);
            collection.AddSingleton(MyIdSource);

            return collection;
        }

        protected override void ConfigureMassTransit(IBusRegistrationConfigurator configurator)
        {
            configurator.AddConsumer<ProducingConsumer>();
        }

        protected override void ConfigureInMemoryBus(IInMemoryBusFactoryConfigurator configurator)
        {
            configurator.UsePublishFilter(typeof(ProducingScopedFilter<>), BusRegistrationContext);
        }

        protected override void ConfigureInMemoryReceiveEndpoint(IInMemoryReceiveEndpointConfigurator configurator)
        {
            configurator.UseMessageScope(ServiceProvider);
            configurator.UseInMemoryOutbox();
            configurator.ConfigureConsumer<ProducingConsumer>(BusRegistrationContext);
        }


        public class ProducingConsumer :
            IConsumer<SimpleMessageInterface>
        {
            readonly TaskCompletionSource<ConsumeContext<SimpleMessageInterface>> _received;

            public ProducingConsumer(TaskCompletionSource<ProducingConsumer> consumerCreated, MyId myId)
            {
                MyId = myId;

                _received = TaskUtil.GetTask<ConsumeContext<SimpleMessageInterface>>();

                consumerCreated.TrySetResult(this);
            }

            public MyId MyId { get; }

            public Task<ConsumeContext<SimpleMessageInterface>> Last => _received.Task;

            public async Task Consume(ConsumeContext<SimpleMessageInterface> context)
            {
                await context.Publish<SimplePublishedInterface>(context.Message);

                _received.TrySetResult(context);
            }
        }
    }


    public class ProducingScopedFilter<T> :
        IFilter<PublishContext<T>>
        where T : class
    {
        readonly MyId _myId;
        readonly TaskCompletionSource<MyId> _taskCompletionSource;

        public ProducingScopedFilter(TaskCompletionSource<MyId> taskCompletionSource, MyId myId)
        {
            _taskCompletionSource = taskCompletionSource;
            _myId = myId;
        }

        public Task Send(PublishContext<T> context, IPipe<PublishContext<T>> next)
        {
            _taskCompletionSource.TrySetResult(_myId);
            return next.Send(context);
        }

        public void Probe(ProbeContext context)
        {
        }
    }


    public class Common_Publish_Filter_Fault<TContainer> :
        CommonContainerTestFixture<TContainer>
        where TContainer : ITestFixtureContainerFactory, new()
    {
        [Test]
        public async Task Should_not_use_scoped_filter_to_publish_fault()
        {
            await InputQueueSendEndpoint.Send<SimpleMessageInterface>(new { Name = "test" });

            ConsumeContext<Fault<SimpleMessageInterface>> result = await TaskCompletionSource.Task;
            Assert.IsNotNull(result);
            Assert.IsFalse(Marker.Called);
        }

        protected readonly FilterMarker Marker;
        protected readonly TaskCompletionSource<ConsumeContext<Fault<SimpleMessageInterface>>> TaskCompletionSource;

        public Common_Publish_Filter_Fault()
        {
            TaskCompletionSource = GetTask<ConsumeContext<Fault<SimpleMessageInterface>>>();
            Marker = new FilterMarker();
        }

        protected override IServiceCollection ConfigureServices(IServiceCollection collection)
        {
            collection.AddSingleton(TaskCompletionSource);
            collection.AddSingleton(Marker);

            return collection;
        }

        protected override void ConfigureMassTransit(IBusRegistrationConfigurator configurator)
        {
            configurator.AddConsumer<FaultyConsumer>();
        }

        protected override void ConfigureInMemoryBus(IInMemoryBusFactoryConfigurator configurator)
        {
            configurator.UsePublishFilter(typeof(FaultyScopedFilter<>), BusRegistrationContext);
        }

        protected override void ConfigureInMemoryReceiveEndpoint(IInMemoryReceiveEndpointConfigurator configurator)
        {
            configurator.ConfigureConsumer<FaultyConsumer>(BusRegistrationContext);
        }


        class FaultyConsumer :
            IConsumer<SimpleMessageInterface>,
            IConsumer<Fault<SimpleMessageInterface>>
        {
            readonly TaskCompletionSource<ConsumeContext<Fault<SimpleMessageInterface>>> _taskCompletionSource;

            public FaultyConsumer(TaskCompletionSource<ConsumeContext<Fault<SimpleMessageInterface>>> taskCompletionSource)
            {
                _taskCompletionSource = taskCompletionSource;
            }

            public Task Consume(ConsumeContext<Fault<SimpleMessageInterface>> context)
            {
                _taskCompletionSource.TrySetResult(context);
                return Task.CompletedTask;
            }

            public Task Consume(ConsumeContext<SimpleMessageInterface> context)
            {
                throw new IntentionalTestException();
            }
        }
    }


    public class FilterMarker
    {
        public bool Called { get; set; }
    }


    public class FaultyScopedFilter<T> :
        IFilter<PublishContext<T>>
        where T : class
    {
        public FaultyScopedFilter(FilterMarker marker)
        {
            marker.Called = true;
        }

        public Task Send(PublishContext<T> context, IPipe<PublishContext<T>> next)
        {
            return next.Send(context);
        }

        public void Probe(ProbeContext context)
        {
        }
    }
}
