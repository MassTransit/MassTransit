namespace MassTransit.Containers.Tests.Common_Tests
{
    using System;
    using System.Threading.Tasks;
    using Context;
    using GreenPipes;
    using NUnit.Framework;
    using Scenarios;
    using TestFramework;
    using Util;


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

            var published = await _taskCompletionSource.Task;

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


    public abstract class Common_Publish_Filter :
        InMemoryTestFixture
    {
        protected readonly TaskCompletionSource<MyId> TaskCompletionSource;

        protected Common_Publish_Filter()
        {
            TaskCompletionSource = GetTask<MyId>();
        }

        protected abstract IBusRegistrationContext Registration { get; }

        protected abstract MyId MyId { get; }

        protected abstract IPublishEndpoint PublishEndpoint { get; }

        [Test]
        public async Task Should_use_scope()
        {
            await PublishEndpoint.Publish<SimpleMessageInterface>(new {Name = "test"});

            var result = await TaskCompletionSource.Task;
            Assert.AreEqual(MyId, result);
        }

        protected override void ConfigureInMemoryBus(IInMemoryBusFactoryConfigurator configurator)
        {
            ConfigureFilter(configurator);
        }

        protected abstract void ConfigureFilter(IPublishPipelineConfigurator publishPipelineConfigurator);

        protected override void ConfigureInMemoryReceiveEndpoint(IInMemoryReceiveEndpointConfigurator configurator)
        {
            configurator.ConfigureConsumer<SimplerConsumer>(Registration);
        }

        protected void ConfigureRegistration(IBusRegistrationConfigurator configurator)
        {
            configurator.AddConsumer<SimplerConsumer>();
            configurator.AddBus(provider => BusControl);
        }


        protected class ScopedFilter<T> :
            IFilter<PublishContext<T>>
            where T : class
        {
            readonly MyId _myId;
            readonly TaskCompletionSource<MyId> _taskCompletionSource;

            public ScopedFilter(TaskCompletionSource<MyId> taskCompletionSource, MyId myId)
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
    }


    public abstract class Common_Publish_Filter_Outbox :
        InMemoryTestFixture
    {
        protected readonly TaskCompletionSource<MyId> MyIdSource;
        protected readonly TaskCompletionSource<ProducingConsumer> ConsumerSource;

        protected Common_Publish_Filter_Outbox()
        {
            MyIdSource = GetTask<MyId>();
            ConsumerSource = GetTask<ProducingConsumer>();
        }

        protected abstract IBusRegistrationContext Registration { get; }

        [Test]
        public async Task Should_use_scope()
        {
            await InputQueueSendEndpoint.Send<SimpleMessageInterface>(new {Name = "test"});

            var myId = await MyIdSource.Task;

            var consumer = await ConsumerSource.Task;

            Assert.AreEqual(myId, consumer.MyId);
        }

        protected override void ConfigureInMemoryBus(IInMemoryBusFactoryConfigurator configurator)
        {
            ConfigureFilter(configurator);
        }

        protected abstract void ConfigureFilter(IPublishPipelineConfigurator publishPipelineConfigurator);

        protected override void ConfigureInMemoryReceiveEndpoint(IInMemoryReceiveEndpointConfigurator configurator)
        {
            configurator.UseInMemoryOutbox();
            configurator.ConfigureConsumer<ProducingConsumer>(Registration);
        }

        protected void ConfigureRegistration(IBusRegistrationConfigurator configurator)
        {
            configurator.AddConsumer<ProducingConsumer>();
            configurator.AddBus(provider => BusControl);
        }


        protected class ScopedFilter<T> :
            IFilter<PublishContext<T>>
            where T : class
        {
            readonly MyId _myId;
            readonly TaskCompletionSource<MyId> _taskCompletionSource;

            public ScopedFilter(TaskCompletionSource<MyId> taskCompletionSource, MyId myId)
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


    public abstract class Common_Publish_Filter_Fault :
        InMemoryTestFixture
    {
        protected readonly TaskCompletionSource<ConsumeContext> TaskCompletionSource;

        protected Common_Publish_Filter_Fault()
        {
            TaskCompletionSource = GetTask<ConsumeContext>();
        }

        protected abstract IBusRegistrationContext Registration { get; }

        [Test]
        public async Task Should_use_scope()
        {
            await InputQueueSendEndpoint.Send<SimpleMessageInterface>(new {Name = "test"});

            var lastConsumer = await FaultyConsumer.LastConsumer;

            ConsumeContext<SimpleMessageInterface> lastConsumeContext = await lastConsumer.Last;

            var result = await TaskCompletionSource.Task;

            Assert.That(lastConsumeContext, Is.Not.InstanceOf<MissingConsumeContext>());
        }

        protected override void ConfigureInMemoryBus(IInMemoryBusFactoryConfigurator configurator)
        {
            ConfigureFilter(configurator);
        }

        protected abstract void ConfigureFilter(IPublishPipelineConfigurator publishPipelineConfigurator);

        protected override void ConfigureInMemoryReceiveEndpoint(IInMemoryReceiveEndpointConfigurator configurator)
        {
            configurator.ConfigureConsumer<FaultyConsumer>(Registration);
        }

        protected void ConfigureRegistration(IBusRegistrationConfigurator configurator)
        {
            configurator.AddConsumer<FaultyConsumer>();
            configurator.AddBus(provider => BusControl);
        }


        protected class ScopedFilter<T> :
            IFilter<PublishContext<T>>
            where T : class
        {
            readonly ConsumeContext _consumeContext;
            readonly TaskCompletionSource<ConsumeContext> _taskCompletionSource;

            public ScopedFilter(TaskCompletionSource<ConsumeContext> taskCompletionSource, ConsumeContext consumeContext)
            {
                _taskCompletionSource = taskCompletionSource;
                _consumeContext = consumeContext;
            }

            public Task Send(PublishContext<T> context, IPipe<PublishContext<T>> next)
            {
                _taskCompletionSource.TrySetResult(_consumeContext);
                return next.Send(context);
            }

            public void Probe(ProbeContext context)
            {
            }
        }


        public class FaultyConsumer :
            IConsumer<SimpleMessageInterface>
        {
            static readonly TaskCompletionSource<FaultyConsumer> _consumerCreated = TaskUtil.GetTask<FaultyConsumer>();

            readonly TaskCompletionSource<ConsumeContext<SimpleMessageInterface>> _received;

            public FaultyConsumer()
            {
                _received = TaskUtil.GetTask<ConsumeContext<SimpleMessageInterface>>();

                _consumerCreated.TrySetResult(this);
            }

            public Task<ConsumeContext<SimpleMessageInterface>> Last => _received.Task;

            public static Task<FaultyConsumer> LastConsumer => _consumerCreated.Task;

            public async Task Consume(ConsumeContext<SimpleMessageInterface> message)
            {
                _received.TrySetResult(message);

                throw new IntentionalTestException("Nothing to see here, move along!");
            }
        }
    }
}
