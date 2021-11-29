namespace MassTransit.Containers.Tests.Common_Tests
{
    using System;
    using System.Linq;
    using System.Threading.Tasks;
    using ConsumeContextTestSubjects;
    using Context;
    using Microsoft.Extensions.DependencyInjection;
    using Middleware.InMemoryOutbox;
    using NUnit.Framework;
    using TestFramework;
    using TestFramework.Messages;
    using Testing;
    using UnitOfWorkComponents;


    public class Common_ConsumeContext<TContainer> :
        CommonContainerTestFixture<TContainer>
        where TContainer : ITestFixtureContainerFactory, new()
    {
        [Test]
        public async Task Should_provide_the_consume_context()
        {
            await InputQueueSendEndpoint.Send(new PingMessage());

            var consumeContext = await ConsumeContext;

            Assert.That(consumeContext.TryGetPayload(out MessageConsumeContext<PingMessage> _), "Is MessageConsumeContext");

            var publishEndpoint = await PublishEndpoint;
            var sendEndpointProvider = await SendEndpointProvider;

            Assert.That(ReferenceEquals(publishEndpoint, sendEndpointProvider), "ReferenceEquals(publishEndpoint, sendEndpointProvider)");
            Assert.That(ReferenceEquals(consumeContext, sendEndpointProvider), "ReferenceEquals(messageConsumeContext, sendEndpointProvider)");
        }

        Task<ConsumeContext> ConsumeContext => ServiceProvider.GetRequiredService<TaskCompletionSource<ConsumeContext>>().Task;
        Task<IPublishEndpoint> PublishEndpoint => ServiceProvider.GetRequiredService<TaskCompletionSource<IPublishEndpoint>>().Task;
        Task<ISendEndpointProvider> SendEndpointProvider => ServiceProvider.GetRequiredService<TaskCompletionSource<ISendEndpointProvider>>().Task;

        protected override IServiceCollection ConfigureServices(IServiceCollection collection)
        {
            TaskCompletionSource<ConsumeContext> pingTask = GetTask<ConsumeContext>();
            TaskCompletionSource<IPublishEndpoint> sendEndpointProviderTask = GetTask<IPublishEndpoint>();
            TaskCompletionSource<ISendEndpointProvider> publishEndpointTask = GetTask<ISendEndpointProvider>();

            return collection.AddSingleton(pingTask)
                .AddSingleton(sendEndpointProviderTask)
                .AddSingleton(publishEndpointTask)
                .AddScoped<IService, Service>()
                .AddScoped<IAnotherService, AnotherService>();
        }

        protected override void ConfigureMassTransit(IBusRegistrationConfigurator configurator)
        {
            configurator.AddConsumer<DependentConsumer>();
        }

        protected override void ConfigureInMemoryReceiveEndpoint(IInMemoryReceiveEndpointConfigurator configurator)
        {
            configurator.ConfigureConsumers(BusRegistrationContext);
        }
    }


    public class Common_ConsumeContext_Outbox<TContainer> :
        CommonContainerTestFixture<TContainer>
        where TContainer : ITestFixtureContainerFactory, new()
    {
        [Test]
        public async Task Should_provide_the_outbox()
        {
            Task<ConsumeContext<Fault<PingMessage>>> fault = await ConnectPublishHandler<Fault<PingMessage>>();

            await InputQueueSendEndpoint.Send(new PingMessage());

            var consumeContext = await ConsumeContext;

            Assert.That(consumeContext.TryGetPayload(out InMemoryOutboxConsumeContext<PingMessage> _), "Is ConsumerConsumeContext");

            var publishEndpoint = await PublishEndpoint;
            var sendEndpointProvider = await SendEndpointProvider;

            Assert.That(ReferenceEquals(publishEndpoint, sendEndpointProvider), "ReferenceEquals(publishEndpoint, sendEndpointProvider)");
            Assert.That(ReferenceEquals(consumeContext, sendEndpointProvider), "ReferenceEquals(outboxConsumeContext, sendEndpointProvider)");

            await fault;

            Assert.That(InMemoryTestHarness.Published.Select<ServiceDidIt>().Any(), Is.False, "Outbox Did Not Intercept!");
        }

        Task<ConsumeContext> ConsumeContext => ServiceProvider.GetRequiredService<TaskCompletionSource<ConsumeContext>>().Task;
        Task<IPublishEndpoint> PublishEndpoint => ServiceProvider.GetRequiredService<TaskCompletionSource<IPublishEndpoint>>().Task;
        Task<ISendEndpointProvider> SendEndpointProvider => ServiceProvider.GetRequiredService<TaskCompletionSource<ISendEndpointProvider>>().Task;

        protected override void ConfigureInMemoryTestHarness(InMemoryTestHarness harness)
        {
            harness.TestTimeout = TimeSpan.FromSeconds(3);
        }

        protected override IServiceCollection ConfigureServices(IServiceCollection collection)
        {
            TaskCompletionSource<ConsumeContext> pingTask = GetTask<ConsumeContext>();
            TaskCompletionSource<IPublishEndpoint> sendEndpointProviderTask = GetTask<IPublishEndpoint>();
            TaskCompletionSource<ISendEndpointProvider> publishEndpointTask = GetTask<ISendEndpointProvider>();

            return collection
                .AddSingleton(pingTask)
                .AddSingleton(sendEndpointProviderTask)
                .AddSingleton(publishEndpointTask)
                .AddScoped<IService, Service>()
                .AddScoped<IAnotherService, AnotherService>();
        }

        protected override void ConfigureMassTransit(IBusRegistrationConfigurator configurator)
        {
            configurator.AddConsumer<DependentConsumer>();
        }

        protected override void ConfigureInMemoryReceiveEndpoint(IInMemoryReceiveEndpointConfigurator configurator)
        {
            configurator.UseInMemoryOutbox();

            configurator.ConfigureConsumers(BusRegistrationContext);
        }
    }


    public class Common_ConsumeContext_Outbox_Batch<TContainer> :
        CommonContainerTestFixture<TContainer>
        where TContainer : ITestFixtureContainerFactory, new()
    {
        [Test]
        public async Task Should_provide_the_outbox()
        {
            Task<ConsumeContext<Fault<PingMessage>>> fault = await ConnectPublishHandler<Fault<PingMessage>>();

            await InputQueueSendEndpoint.Send(new PingMessage());
            await InputQueueSendEndpoint.Send(new PingMessage());
            await InputQueueSendEndpoint.Send(new PingMessage());
            await InputQueueSendEndpoint.Send(new PingMessage());

            var consumeContext = await ConsumeContext;

            Assert.That(consumeContext.TryGetPayload(out InMemoryOutboxConsumeContext<Batch<PingMessage>> _), "Is ConsumerConsumeContext");

            var publishEndpoint = await PublishEndpoint;
            var sendEndpointProvider = await SendEndpointProvider;

            Assert.That(ReferenceEquals(publishEndpoint, sendEndpointProvider), "ReferenceEquals(publishEndpoint, sendEndpointProvider)");
            Assert.That(ReferenceEquals(consumeContext, sendEndpointProvider), "ReferenceEquals(outboxConsumeContext, sendEndpointProvider)");

            await fault;

            Assert.That(InMemoryTestHarness.Published.Select<ServiceDidIt>().Any(), Is.False, "Outbox Did Not Intercept!");
        }

        Task<ConsumeContext> ConsumeContext => ServiceProvider.GetRequiredService<TaskCompletionSource<ConsumeContext>>().Task;
        Task<IPublishEndpoint> PublishEndpoint => ServiceProvider.GetRequiredService<TaskCompletionSource<IPublishEndpoint>>().Task;
        Task<ISendEndpointProvider> SendEndpointProvider => ServiceProvider.GetRequiredService<TaskCompletionSource<ISendEndpointProvider>>().Task;

        protected override void ConfigureInMemoryTestHarness(InMemoryTestHarness harness)
        {
            harness.TestTimeout = TimeSpan.FromSeconds(3);
        }

        protected override IServiceCollection ConfigureServices(IServiceCollection collection)
        {
            TaskCompletionSource<ConsumeContext> pingTask = GetTask<ConsumeContext>();
            TaskCompletionSource<IPublishEndpoint> sendEndpointProviderTask = GetTask<IPublishEndpoint>();
            TaskCompletionSource<ISendEndpointProvider> publishEndpointTask = GetTask<ISendEndpointProvider>();

            return collection
                .AddSingleton(pingTask)
                .AddSingleton(sendEndpointProviderTask)
                .AddSingleton(publishEndpointTask)
                .AddScoped<IService, Service>()
                .AddScoped<UnitOfWork>();
        }

        protected override void ConfigureMassTransit(IBusRegistrationConfigurator configurator)
        {
            configurator.AddConsumer<DependentBatchConsumer>(x =>
                x.Options<BatchOptions>(b => b.SetTimeLimit(200).SetMessageLimit(4)));
        }

        protected override void ConfigureInMemoryReceiveEndpoint(IInMemoryReceiveEndpointConfigurator configurator)
        {
            configurator.UseDelayedRedelivery(r => r.None());
            configurator.UseMessageRetry(r => r.None());
            configurator.UseInMemoryOutbox();
            configurator.UseUnitOfWork();

            configurator.ConfigureConsumers(BusRegistrationContext);
        }
    }


    public class Common_ConsumeContext_Filter_Batch<TContainer> :
        CommonContainerTestFixture<TContainer>
        where TContainer : ITestFixtureContainerFactory, new()
    {
        [Test]
        public async Task Should_provide_the_outbox()
        {
            Task<ConsumeContext<Fault<PingMessage>>> fault = await ConnectPublishHandler<Fault<PingMessage>>();

            await InputQueueSendEndpoint.Send(new PingMessage());
            await InputQueueSendEndpoint.Send(new PingMessage());
            await InputQueueSendEndpoint.Send(new PingMessage());
            await InputQueueSendEndpoint.Send(new PingMessage());

            var consumeContext = await ConsumeContext;

            Assert.That(consumeContext.TryGetPayload(out InMemoryOutboxConsumeContext<Batch<PingMessage>> _), "Is ConsumerConsumeContext");

            var publishEndpoint = await PublishEndpoint;
            var sendEndpointProvider = await SendEndpointProvider;

            Assert.That(ReferenceEquals(publishEndpoint, sendEndpointProvider), "ReferenceEquals(publishEndpoint, sendEndpointProvider)");
            Assert.That(ReferenceEquals(consumeContext, sendEndpointProvider), "ReferenceEquals(outboxConsumeContext, sendEndpointProvider)");

            await fault;

            Assert.That(InMemoryTestHarness.Published.Select<ServiceDidIt>().Any(), Is.False, "Outbox Did Not Intercept!");
        }

        Task<ConsumeContext> ConsumeContext => ServiceProvider.GetRequiredService<TaskCompletionSource<ConsumeContext>>().Task;
        Task<IPublishEndpoint> PublishEndpoint => ServiceProvider.GetRequiredService<TaskCompletionSource<IPublishEndpoint>>().Task;
        Task<ISendEndpointProvider> SendEndpointProvider => ServiceProvider.GetRequiredService<TaskCompletionSource<ISendEndpointProvider>>().Task;

        protected override void ConfigureInMemoryTestHarness(InMemoryTestHarness harness)
        {
            harness.TestTimeout = TimeSpan.FromSeconds(3);
        }

        protected override IServiceCollection ConfigureServices(IServiceCollection collection)
        {
            TaskCompletionSource<ConsumeContext> pingTask = GetTask<ConsumeContext>();
            TaskCompletionSource<IPublishEndpoint> sendEndpointProviderTask = GetTask<IPublishEndpoint>();
            TaskCompletionSource<ISendEndpointProvider> publishEndpointTask = GetTask<ISendEndpointProvider>();

            return collection
                .AddSingleton(pingTask)
                .AddSingleton(sendEndpointProviderTask)
                .AddSingleton(publishEndpointTask)
                .AddScoped<IService, Service>()
                .AddScoped<UnitOfWork>();
        }

        protected override void ConfigureMassTransit(IBusRegistrationConfigurator configurator)
        {
            configurator.AddConsumer<DependentBatchConsumer>(x =>
                x.Options<BatchOptions>(b => b.SetTimeLimit(200).SetMessageLimit(4)));
        }

        protected override void ConfigureInMemoryReceiveEndpoint(IInMemoryReceiveEndpointConfigurator configurator)
        {
            configurator.UseInMemoryOutbox();
            configurator.UseUnitOfWork();

            configurator.ConfigureConsumers(BusRegistrationContext);
        }
    }


    public class Common_ConsumeContext_Outbox_Solo<TContainer> :
        CommonContainerTestFixture<TContainer>
        where TContainer : ITestFixtureContainerFactory, new()
    {
        [Test]
        public async Task Should_provide_the_outbox_to_the_consumer()
        {
            Task<ConsumeContext<Fault<PingMessage>>> fault = await ConnectPublishHandler<Fault<PingMessage>>();

            await InputQueueSendEndpoint.Send(new PingMessage());

            var consumeContext = await ConsumeContext;

            Assert.That(consumeContext.TryGetPayload(out InMemoryOutboxConsumeContext<PingMessage> _), "Is ConsumerConsumeContext");

            var publishEndpoint = await PublishEndpoint;
            var sendEndpointProvider = await SendEndpointProvider;

            Assert.That(ReferenceEquals(publishEndpoint, sendEndpointProvider), "ReferenceEquals(publishEndpoint, sendEndpointProvider)");
            Assert.That(ReferenceEquals(consumeContext, sendEndpointProvider), "ReferenceEquals(outboxConsumeContext, sendEndpointProvider)");

            await fault;

            Assert.That(InMemoryTestHarness.Published.Select<ServiceDidIt>().Any(), Is.False, "Outbox Did Not Intercept!");
        }

        Task<ConsumeContext> ConsumeContext => ServiceProvider.GetRequiredService<TaskCompletionSource<ConsumeContext>>().Task;
        Task<IPublishEndpoint> PublishEndpoint => ServiceProvider.GetRequiredService<TaskCompletionSource<IPublishEndpoint>>().Task;
        Task<ISendEndpointProvider> SendEndpointProvider => ServiceProvider.GetRequiredService<TaskCompletionSource<ISendEndpointProvider>>().Task;

        protected override void ConfigureInMemoryTestHarness(InMemoryTestHarness harness)
        {
            harness.TestTimeout = TimeSpan.FromSeconds(3);
        }

        protected override IServiceCollection ConfigureServices(IServiceCollection collection)
        {
            TaskCompletionSource<ConsumeContext> pingTask = GetTask<ConsumeContext>();
            TaskCompletionSource<IPublishEndpoint> sendEndpointProviderTask = GetTask<IPublishEndpoint>();
            TaskCompletionSource<ISendEndpointProvider> publishEndpointTask = GetTask<ISendEndpointProvider>();

            return collection
                .AddSingleton(pingTask)
                .AddSingleton(sendEndpointProviderTask)
                .AddSingleton(publishEndpointTask)
                .AddScoped<IService, Service>()
                .AddScoped<UnitOfWork>();
        }

        protected override void ConfigureMassTransit(IBusRegistrationConfigurator configurator)
        {
            configurator.AddConsumer<FlyingSoloConsumer>();
        }

        protected override void ConfigureInMemoryReceiveEndpoint(IInMemoryReceiveEndpointConfigurator configurator)
        {
            configurator.UseInMemoryOutbox();

            configurator.ConfigureConsumers(BusRegistrationContext);
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
            readonly ConsumeContext _consumeContext;
            readonly TaskCompletionSource<ConsumeContext> _consumeContextTask;
            readonly IPublishEndpoint _publishEndpoint;

            public FlyingSoloConsumer(IPublishEndpoint publishEndpoint, ISendEndpointProvider sendEndpointProvider, ConsumeContext consumeContext,
                TaskCompletionSource<ConsumeContext> consumeContextTask,
                TaskCompletionSource<IPublishEndpoint> publishEndpointTask,
                TaskCompletionSource<ISendEndpointProvider> sendEndpointProviderTask)
            {
                _publishEndpoint = publishEndpoint;
                _consumeContext = consumeContext;
                _consumeContextTask = consumeContextTask;
                publishEndpointTask.TrySetResult(publishEndpoint);
                sendEndpointProviderTask.TrySetResult(sendEndpointProvider);
            }

            public async Task Consume(ConsumeContext<PingMessage> context)
            {
                await _publishEndpoint.Publish<ServiceDidIt>(new { });

                _consumeContextTask.TrySetResult(_consumeContext);

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


    namespace UnitOfWorkComponents
    {
        using Configuration;


        public class UnitOfWorkFilter<TMessage> :
            IFilter<ConsumeContext<TMessage>>
            where TMessage : class
        {
            public void Probe(ProbeContext context)
            {
                context.CreateFilterScope("uow");
            }

            public async Task Send(ConsumeContext<TMessage> context, IPipe<ConsumeContext<TMessage>> next)
            {
                var provider = context.GetPayload<IServiceProvider>();
                var unitOfWork = provider.GetRequiredService<UnitOfWork>();

                await next.Send(context);
            }
        }


        public class UnitOfWorkFilter<TContext, TConsumer> :
            IFilter<TContext>
            where TConsumer : class
            where TContext : class, ConsumerConsumeContext<TConsumer>
        {
            public void Probe(ProbeContext context)
            {
                context.CreateFilterScope("uow");
            }

            public async Task Send(TContext context, IPipe<TContext> next)
            {
                var provider = context.GetPayload<IServiceProvider>();
                var unitOfWork = provider.GetRequiredService<UnitOfWork>();

                await next.Send(context);
            }
        }


        public class UnitOfWorkConfigurationObserver :
            IConsumerConfigurationObserver
        {
            public void ConsumerConfigured<TConsumer>(IConsumerConfigurator<TConsumer> configurator)
                where TConsumer : class
            {
                var filter = new UnitOfWorkFilter<ConsumerConsumeContext<TConsumer>, TConsumer>();
                var specification = new FilterPipeSpecification<ConsumerConsumeContext<TConsumer>>(filter);
                configurator.AddPipeSpecification(specification);
            }

            public void ConsumerMessageConfigured<TConsumer, TMessage>(IConsumerMessageConfigurator<TConsumer, TMessage> configurator)
                where TConsumer : class
                where TMessage : class
            {
            }
        }


        public static class UnitOfWorkMiddlewareConfiguratorExtensions
        {
            public static void UseUnitOfWork(this IConsumePipeConfigurator configurator)
            {
                configurator.ConnectConsumerConfigurationObserver(new UnitOfWorkConfigurationObserver());
            }
        }
    }
}
