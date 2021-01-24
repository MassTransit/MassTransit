namespace MassTransit.Containers.Tests.Common_Tests
{
    using System;
    using System.Linq;
    using System.Threading.Tasks;
    using ConsumeContextTestSubjects;
    using Context;
    using GreenPipes;
    using NUnit.Framework;
    using TestFramework;
    using TestFramework.Messages;


    public abstract class Common_ConsumeContext :
        InMemoryTestFixture
    {
        protected abstract IBusRegistrationContext Registration { get; }
        protected abstract Task<ConsumeContext> ConsumeContext { get; }
        protected abstract Task<IPublishEndpoint> PublishEndpoint { get; }
        protected abstract Task<ISendEndpointProvider> SendEndpointProvider { get; }

        [Test]
        public async Task Should_provide_the_consume_context()
        {
            await InputQueueSendEndpoint.Send(new PingMessage());

            var consumeContext = await ConsumeContext;

            Assert.That(consumeContext.TryGetPayload(out MessageConsumeContext<PingMessage> messageConsumeContext),
                "Is MessageConsumeContext");

            var publishEndpoint = await PublishEndpoint;
            var sendEndpointProvider = await SendEndpointProvider;

            Assert.That(publishEndpoint, Is.TypeOf<MessageConsumeContext<PingMessage>>());
            Assert.That(sendEndpointProvider, Is.TypeOf<MessageConsumeContext<PingMessage>>());
            Assert.That(ReferenceEquals(publishEndpoint, sendEndpointProvider), "ReferenceEquals(publishEndpoint, sendEndpointProvider)");
            Assert.That(ReferenceEquals(messageConsumeContext, sendEndpointProvider), "ReferenceEquals(messageConsumeContext, sendEndpointProvider)");
        }

        protected void ConfigureRegistration(IBusRegistrationConfigurator configurator)
        {
            configurator.AddConsumer<DependentConsumer>();
            configurator.AddBus(provider => BusControl);
        }

        protected override void ConfigureInMemoryReceiveEndpoint(IInMemoryReceiveEndpointConfigurator configurator)
        {
            configurator.ConfigureConsumers(Registration);
        }
    }


    public abstract class Common_ConsumeContext_Outbox :
        InMemoryTestFixture
    {
        protected Common_ConsumeContext_Outbox()
        {
            TestTimeout = TimeSpan.FromSeconds(3);
        }

        protected abstract IBusRegistrationContext Registration { get; }
        protected abstract Task<ConsumeContext> ConsumeContext { get; }
        protected abstract Task<IPublishEndpoint> PublishEndpoint { get; }
        protected abstract Task<ISendEndpointProvider> SendEndpointProvider { get; }

        [Test]
        public async Task Should_provide_the_outbox()
        {
            Task<ConsumeContext<Fault<PingMessage>>> fault = ConnectPublishHandler<Fault<PingMessage>>();

            await InputQueueSendEndpoint.Send(new PingMessage());

            var consumeContext = await ConsumeContext;

            Assert.That(
                consumeContext.TryGetPayload(out InMemoryOutboxConsumeContext<PingMessage> outboxConsumeContext),
                "Is ConsumerConsumeContext");

            var publishEndpoint = await PublishEndpoint;
            var sendEndpointProvider = await SendEndpointProvider;

            Assert.That(publishEndpoint, Is.TypeOf<InMemoryOutboxConsumeContext<PingMessage>>());
            Assert.That(sendEndpointProvider, Is.TypeOf<InMemoryOutboxConsumeContext<PingMessage>>());
            Assert.That(ReferenceEquals(publishEndpoint, sendEndpointProvider), "ReferenceEquals(publishEndpoint, sendEndpointProvider)");
            Assert.That(ReferenceEquals(outboxConsumeContext, sendEndpointProvider), "ReferenceEquals(outboxConsumeContext, sendEndpointProvider)");

            await fault;

            Assert.That(InMemoryTestHarness.Published.Select<ServiceDidIt>().Any(), Is.False, "Outbox Did Not Intercept!");
        }

        protected void ConfigureRegistration(IBusRegistrationConfigurator configurator)
        {
            configurator.AddConsumer<DependentConsumer>();
            configurator.AddBus(provider => BusControl);
        }

        protected override void ConfigureInMemoryReceiveEndpoint(IInMemoryReceiveEndpointConfigurator configurator)
        {
            configurator.UseInMemoryOutbox();

            configurator.ConfigureConsumers(Registration);
        }
    }


    public abstract class Common_ConsumeContext_Outbox_Batch :
        InMemoryTestFixture
    {
        protected Common_ConsumeContext_Outbox_Batch()
        {
            TestTimeout = TimeSpan.FromSeconds(3);
        }

        protected abstract IBusRegistrationContext Registration { get; }
        protected abstract Task<ConsumeContext> ConsumeContext { get; }
        protected abstract Task<IPublishEndpoint> PublishEndpoint { get; }
        protected abstract Task<ISendEndpointProvider> SendEndpointProvider { get; }

        [Test]
        public async Task Should_provide_the_outbox()
        {
            Task<ConsumeContext<Fault<PingMessage>>> fault = ConnectPublishHandler<Fault<PingMessage>>();

            await InputQueueSendEndpoint.Send(new PingMessage());
            await InputQueueSendEndpoint.Send(new PingMessage());
            await InputQueueSendEndpoint.Send(new PingMessage());
            await InputQueueSendEndpoint.Send(new PingMessage());

            var consumeContext = await ConsumeContext;

            Assert.That(
                consumeContext.TryGetPayload(out InMemoryOutboxConsumeContext<Batch<PingMessage>> outboxConsumeContext),
                "Is ConsumerConsumeContext");

            var publishEndpoint = await PublishEndpoint;
            var sendEndpointProvider = await SendEndpointProvider;

            Assert.That(publishEndpoint, Is.TypeOf<InMemoryOutboxConsumeContext<Batch<PingMessage>>>());
            Assert.That(sendEndpointProvider, Is.TypeOf<InMemoryOutboxConsumeContext<Batch<PingMessage>>>());
            Assert.That(ReferenceEquals(publishEndpoint, sendEndpointProvider), "ReferenceEquals(publishEndpoint, sendEndpointProvider)");
            Assert.That(ReferenceEquals(outboxConsumeContext, sendEndpointProvider), "ReferenceEquals(outboxConsumeContext, sendEndpointProvider)");

            await fault;

            Assert.That(InMemoryTestHarness.Published.Select<ServiceDidIt>().Any(), Is.False, "Outbox Did Not Intercept!");
        }

        protected void ConfigureRegistration(IBusRegistrationConfigurator configurator)
        {
            configurator.AddConsumer<DependentBatchConsumer>(x =>
                x.Options<BatchOptions>(b => b.SetTimeLimit(200).SetMessageLimit(4)));

            configurator.AddBus(provider => BusControl);
        }

        protected override void ConfigureInMemoryReceiveEndpoint(IInMemoryReceiveEndpointConfigurator configurator)
        {
            configurator.UseScheduledRedelivery(r => r.None());
            configurator.UseMessageRetry(r => r.None());
            configurator.UseInMemoryOutbox();

            ConfigureUnitOfWork(configurator);

            configurator.ConfigureConsumers(Registration);
        }

        protected virtual void ConfigureUnitOfWork(IInMemoryReceiveEndpointConfigurator configurator)
        {
        }
    }


    public abstract class Common_ConsumeContext_Filter_Batch :
        InMemoryTestFixture
    {
        protected Common_ConsumeContext_Filter_Batch()
        {
            TestTimeout = TimeSpan.FromSeconds(3);
        }

        protected abstract IBusRegistrationContext Registration { get; }
        protected abstract Task<ConsumeContext> ConsumeContext { get; }
        protected abstract Task<IPublishEndpoint> PublishEndpoint { get; }
        protected abstract Task<ISendEndpointProvider> SendEndpointProvider { get; }

        [Test]
        public async Task Should_provide_the_outbox()
        {
            Task<ConsumeContext<Fault<PingMessage>>> fault = ConnectPublishHandler<Fault<PingMessage>>();

            await InputQueueSendEndpoint.Send(new PingMessage());
            await InputQueueSendEndpoint.Send(new PingMessage());
            await InputQueueSendEndpoint.Send(new PingMessage());
            await InputQueueSendEndpoint.Send(new PingMessage());

            var consumeContext = await ConsumeContext;

            Assert.That(
                consumeContext.TryGetPayload(out InMemoryOutboxConsumeContext<Batch<PingMessage>> outboxConsumeContext),
                "Is ConsumerConsumeContext");

            var publishEndpoint = await PublishEndpoint;
            var sendEndpointProvider = await SendEndpointProvider;

            Assert.That(publishEndpoint, Is.TypeOf<InMemoryOutboxConsumeContext<Batch<PingMessage>>>());
            Assert.That(sendEndpointProvider, Is.TypeOf<InMemoryOutboxConsumeContext<Batch<PingMessage>>>());
            Assert.That(ReferenceEquals(publishEndpoint, sendEndpointProvider), "ReferenceEquals(publishEndpoint, sendEndpointProvider)");
            Assert.That(ReferenceEquals(outboxConsumeContext, sendEndpointProvider), "ReferenceEquals(outboxConsumeContext, sendEndpointProvider)");

            await fault;

            Assert.That(InMemoryTestHarness.Published.Select<ServiceDidIt>().Any(), Is.False, "Outbox Did Not Intercept!");
        }

        protected void ConfigureRegistration(IBusRegistrationConfigurator configurator)
        {
            configurator.AddConsumer<DependentBatchConsumer>(x =>
                x.Options<BatchOptions>(b => b.SetTimeLimit(200).SetMessageLimit(4)));

            configurator.AddBus(provider => BusControl);
        }

        protected override void ConfigureInMemoryReceiveEndpoint(IInMemoryReceiveEndpointConfigurator configurator)
        {
            configurator.UseInMemoryOutbox();

            ConfigureUnitOfWork(configurator);

            configurator.ConfigureConsumers(Registration);
        }

        protected virtual void ConfigureUnitOfWork(IInMemoryReceiveEndpointConfigurator configurator)
        {
        }
    }


    public abstract class Common_ConsumeContext_Outbox_Solo :
        InMemoryTestFixture
    {
        protected Common_ConsumeContext_Outbox_Solo()
        {
            TestTimeout = TimeSpan.FromSeconds(3);
        }

        protected abstract IBusRegistrationContext Registration { get; }
        protected abstract Task<ConsumeContext> ConsumeContext { get; }
        protected abstract Task<IPublishEndpoint> PublishEndpoint { get; }
        protected abstract Task<ISendEndpointProvider> SendEndpointProvider { get; }

        [Test]
        public async Task Should_provide_the_outbox_to_the_consumer()
        {
            Task<ConsumeContext<Fault<PingMessage>>> fault = ConnectPublishHandler<Fault<PingMessage>>();

            await InputQueueSendEndpoint.Send(new PingMessage());

            var consumeContext = await ConsumeContext;

            Assert.That(
                consumeContext.TryGetPayload(out InMemoryOutboxConsumeContext<PingMessage> outboxConsumeContext),
                "Is ConsumerConsumeContext");

            var publishEndpoint = await PublishEndpoint;
            var sendEndpointProvider = await SendEndpointProvider;

            Assert.That(publishEndpoint, Is.TypeOf<InMemoryOutboxConsumeContext<PingMessage>>());
            Assert.That(sendEndpointProvider, Is.TypeOf<InMemoryOutboxConsumeContext<PingMessage>>());
            Assert.That(ReferenceEquals(publishEndpoint, sendEndpointProvider), "ReferenceEquals(publishEndpoint, sendEndpointProvider)");
            Assert.That(ReferenceEquals(outboxConsumeContext, sendEndpointProvider), "ReferenceEquals(outboxConsumeContext, sendEndpointProvider)");

            await fault;

            Assert.That(InMemoryTestHarness.Published.Select<ServiceDidIt>().Any(), Is.False, "Outbox Did Not Intercept!");
        }

        protected void ConfigureRegistration(IBusRegistrationConfigurator configurator)
        {
            configurator.AddConsumer<FlyingSoloConsumer>();
            configurator.AddBus(provider => BusControl);
        }

        protected override void ConfigureInMemoryReceiveEndpoint(IInMemoryReceiveEndpointConfigurator configurator)
        {
            configurator.UseInMemoryOutbox();

            configurator.ConfigureConsumers(Registration);
        }
    }


    namespace ConsumeContextTestSubjects
    {
        using TestFramework.Messages;


        class DependentConsumer :
            IConsumer<PingMessage>
        {
            readonly IAnotherService _anotherService;
            readonly IService _service;

            public DependentConsumer(IService service, IAnotherService anotherService)
            {
                _service = service;
                _anotherService = anotherService;
            }

            public async Task Consume(ConsumeContext<PingMessage> context)
            {
                await _service.DoIt();

                _anotherService.Done();

                throw new IntentionalTestException();
            }
        }


        class DependentBatchConsumer :
            IConsumer<Batch<PingMessage>>
        {
            readonly IService _service;
            readonly UnitOfWork _unitOfWork;

            public DependentBatchConsumer(IService service, UnitOfWork unitOfWork)
            {
                _service = service;
                _unitOfWork = unitOfWork;
            }

            public async Task Consume(ConsumeContext<Batch<PingMessage>> context)
            {
                await _service.DoIt();

                _unitOfWork.Add();

                throw new IntentionalTestException();
            }
        }


        class FlyingSoloConsumer :
            IConsumer<PingMessage>
        {
            readonly TaskCompletionSource<ConsumeContext> _consumeContextTask;
            readonly IPublishEndpoint _publishEndpoint;

            public FlyingSoloConsumer(IPublishEndpoint publishEndpoint, ISendEndpointProvider sendEndpointProvider,
                TaskCompletionSource<ConsumeContext> consumeContextTask,
                TaskCompletionSource<IPublishEndpoint> publishEndpointTask,
                TaskCompletionSource<ISendEndpointProvider> sendEndpointProviderTask)
            {
                _publishEndpoint = publishEndpoint;
                _consumeContextTask = consumeContextTask;
                publishEndpointTask.TrySetResult(publishEndpoint);
                sendEndpointProviderTask.TrySetResult(sendEndpointProvider);
            }

            public async Task Consume(ConsumeContext<PingMessage> context)
            {
                _consumeContextTask.TrySetResult(context);

                await _publishEndpoint.Publish<ServiceDidIt>(new { });

                throw new IntentionalTestException();
            }
        }


        public interface ServiceDidIt
        {
        }


        interface IService
        {
            Task DoIt();
        }


        class Service :
            IService
        {
            readonly IPublishEndpoint _publishEndpoint;

            public Service(IPublishEndpoint publishEndpoint, ISendEndpointProvider sendEndpointProvider,
                TaskCompletionSource<IPublishEndpoint> publishEndpointTask,
                TaskCompletionSource<ISendEndpointProvider> sendEndpointProviderTask)
            {
                _publishEndpoint = publishEndpoint;
                publishEndpointTask.TrySetResult(publishEndpoint);
                sendEndpointProviderTask.TrySetResult(sendEndpointProvider);
            }

            public async Task DoIt()
            {
                await _publishEndpoint.Publish<ServiceDidIt>(new { });
            }
        }


        interface IAnotherService
        {
            void Done();
        }


        class AnotherService :
            IAnotherService
        {
            readonly TaskCompletionSource<ConsumeContext> _consumeContextTask;
            readonly ConsumeContext _context;

            public AnotherService(ConsumeContext context, TaskCompletionSource<ConsumeContext> consumeContextTask)
            {
                _context = context;
                _consumeContextTask = consumeContextTask;
            }

            public void Done()
            {
                _consumeContextTask.TrySetResult(_context);
            }
        }


        public class UnitOfWork
        {
            readonly TaskCompletionSource<ConsumeContext> _consumeContextTask;
            readonly ConsumeContext _context;

            public UnitOfWork(ConsumeContext context, TaskCompletionSource<ConsumeContext> consumeContextTask)
            {
                _context = context;
                _consumeContextTask = consumeContextTask;
            }

            public void Add()
            {
                _consumeContextTask.TrySetResult(_context);
            }
        }
    }
}
