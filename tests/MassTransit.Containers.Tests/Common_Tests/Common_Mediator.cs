namespace MassTransit.Containers.Tests.Common_Tests
{
    using System;
    using System.Threading.Tasks;
    using Internals;
    using Mediator;
    using Microsoft.Extensions.DependencyInjection;
    using NUnit.Framework;
    using Scenarios;
    using Shouldly;
    using TestFramework;
    using Testing;


    public class Using_mediator_alongside_the_bus<TContainer> :
        CommonContainerTestFixture<TContainer>
        where TContainer : ITestFixtureContainerFactory, new()
    {
        [Test]
        public async Task Should_dispatch_to_the_consumer()
        {
            const string name = "Joe";

            await Mediator.Send(new SimpleMessageClass(name));

            var lastConsumer = await SimplerConsumer.LastConsumer.OrCanceled(InMemoryTestHarness.TestCancellationToken);
            lastConsumer.ShouldNotBe(null);

            await lastConsumer.Last.OrCanceled(InMemoryTestHarness.TestCancellationToken);
        }

        IMediator Mediator => ServiceProvider.GetRequiredService<IMediator>();

        protected override IServiceCollection ConfigureServices(IServiceCollection collection)
        {
            return collection.AddMediator(ConfigureRegistration);
        }

        void ConfigureRegistration(IMediatorRegistrationConfigurator configurator)
        {
            configurator.AddConsumer<SimplerConsumer>();
        }
    }


    public class Publishing_a_message_from_a_mediator_consumer<TContainer> :
        CommonContainerTestFixture<TContainer>
        where TContainer : ITestFixtureContainerFactory, new()
    {
        [Test]
        public async Task Should_not_transfer_message_headers()
        {
            Task<ConsumeContext<OrderSubmitted>> orderSubmitted = await ConnectPublishHandler<OrderSubmitted>();

            await Mediator.Send<SubmitOrder>(new
            {
                InVar.CorrelationId,
                OrderNumber = "12345"
            });

            ConsumeContext<OrderSubmitted> submitted = await orderSubmitted;

            // headers are not transferred from the mediator to the bus automatically
            Assert.That(submitted.InitiatorId.HasValue, Is.False);
        }

        IMediator Mediator => ServiceProvider.GetRequiredService<IMediator>();

        protected override IServiceCollection ConfigureServices(IServiceCollection collection)
        {
            return collection.AddMediator(ConfigureRegistration);
        }

        void ConfigureRegistration(IMediatorRegistrationConfigurator configurator)
        {
            configurator.AddConsumer<SubmitOrderConsumer>();
        }


        class SubmitOrderConsumer :
            IConsumer<SubmitOrder>
        {
            readonly IBus _bus;

            public SubmitOrderConsumer(IBus bus)
            {
                _bus = bus;
            }

            public async Task Consume(ConsumeContext<SubmitOrder> context)
            {
                await _bus.Publish<OrderSubmitted>(context.Message);
            }
        }


        public interface SubmitOrder
        {
            Guid CorrelationId { get; }
            string OrderNumber { get; }
        }


        public interface OrderSubmitted
        {
            Guid CorrelationId { get; }
            string OrderNumber { get; }
        }
    }


    public class Common_Mediator_Request<TContainer> :
        CommonContainerTestFixture<TContainer>
        where TContainer : ITestFixtureContainerFactory, new()
    {
        [Test]
        public async Task Should_receive_the_response()
        {
            _correlationId = NewId.NextGuid();

            Response<InitialResponse> response = await GetRequestClient<InitialRequest>().GetResponse<InitialResponse>(new
            {
                CorrelationId = _correlationId,
                Value = "World"
            });

            Assert.That(response.Message.Value, Is.EqualTo("Hello, World"));
            Assert.That(response.ConversationId.Value, Is.EqualTo(response.Message.OriginalConversationId));
            Assert.That(response.InitiatorId.Value, Is.EqualTo(_correlationId));
            Assert.That(response.Message.OriginalInitiatorId, Is.EqualTo(_correlationId));
        }

        Guid _correlationId;

        protected override IServiceCollection ConfigureServices(IServiceCollection collection)
        {
            return collection.AddMediator(ConfigureRegistration);
        }

        void ConfigureRegistration(IMediatorRegistrationConfigurator configurator)
        {
            configurator.AddConsumer<InitialConsumer>();
            configurator.AddConsumer<SubsequentConsumer>();

            configurator.AddRequestClient<InitialRequest>();
            configurator.AddRequestClient<SubsequentRequest>();
        }


        class InitialConsumer :
            IConsumer<InitialRequest>
        {
            readonly IRequestClient<SubsequentRequest> _client;

            public InitialConsumer(IRequestClient<SubsequentRequest> client)
            {
                _client = client;
            }

            public async Task Consume(ConsumeContext<InitialRequest> context)
            {
                Response<SubsequentResponse> response = await _client.GetResponse<SubsequentResponse>(context.Message);

                await context.RespondAsync<InitialResponse>(response.Message);
            }
        }


        class SubsequentConsumer :
            IConsumer<SubsequentRequest>
        {
            public Task Consume(ConsumeContext<SubsequentRequest> context)
            {
                return context.RespondAsync<SubsequentResponse>(new
                {
                    OriginalConversationId = context.ConversationId,
                    OriginalInitiatorId = context.InitiatorId,
                    Value = $"Hello, {context.Message.Value}"
                });
            }
        }


        public interface InitialRequest
        {
            Guid CorrelationId { get; }
            string Value { get; }
        }


        public interface InitialResponse
        {
            Guid OriginalConversationId { get; }
            Guid OriginalInitiatorId { get; }
            string Value { get; }
        }


        public interface SubsequentRequest
        {
            Guid CorrelationId { get; }
            string Value { get; }
        }


        public interface SubsequentResponse
        {
            Guid OriginalConversationId { get; }
            Guid OriginalInitiatorId { get; }
            string Value { get; }
        }
    }


    public class Common_Mediator_Request_Filter<TContainer> :
        CommonContainerTestFixture<TContainer>
        where TContainer : ITestFixtureContainerFactory, new()
    {
        [Test]
        public async Task Should_receive_the_response()
        {
            Response<Pong> response = await GetRequestClient<Ping>().GetResponse<Pong>(new Ping());

            Assert.That(response.Message.Message, Is.EqualTo("PONG!"));
        }

        protected override IServiceCollection ConfigureServices(IServiceCollection collection)
        {
            return collection.AddMediator(ConfigureRegistration);
        }

        protected void ConfigureRegistration(IMediatorRegistrationConfigurator configurator)
        {
            configurator.AddConsumer<PingConsumer>();

            configurator.AddRequestClient<Ping>();

            configurator.ConfigureMediator((context, cfg) =>
            {
                cfg.UseSendFilter(typeof(PongFilter<>), context);
            });
        }


        public class PingConsumer :
            IConsumer<Ping>
        {
            public async Task Consume(ConsumeContext<Ping> context)
            {
                await context.RespondAsync(new Pong { Message = "Pong!" });
            }
        }
    }


    public class PongFilter<T> :
        IFilter<SendContext<T>>
        where T : class
    {
        public void Probe(ProbeContext context)
        {
        }

        public Task Send(SendContext<T> context, IPipe<SendContext<T>> next)
        {
            if (context.Message is Pong pong)
                pong.Message = pong.Message.ToUpper();

            return next.Send(context);
        }
    }


    public class Ping
    {
    }


    public class Pong
    {
        public string Message { get; set; }
    }


    public class Common_Mediator_Saga<TContainer> :
        CommonContainerTestFixture<TContainer>
        where TContainer : ITestFixtureContainerFactory, new()
    {
        [Test]
        public async Task Should_receive_the_response()
        {
            _correlationId = NewId.NextGuid();

            await Mediator.Send<SubmitOrder>(new
            {
                CorrelationId = _correlationId,
                OrderNumber = "90210"
            });

            Guid? foundId = await GetSagaRepository<OrderSaga>().ShouldContainSaga(_correlationId, TestTimeout);

            Assert.That(foundId.HasValue, Is.True);
        }

        Guid _correlationId;

        IMediator Mediator => ServiceProvider.GetRequiredService<IMediator>();

        protected override IServiceCollection ConfigureServices(IServiceCollection collection)
        {
            return collection.AddMediator(ConfigureRegistration);
        }

        void ConfigureRegistration(IMediatorRegistrationConfigurator configurator)
        {
            configurator.AddConsumer<OrderConsumer>();
            configurator.AddSaga<OrderSaga>()
                .InMemoryRepository();
        }


        class OrderConsumer :
            IConsumer<SubmitOrder>
        {
            public async Task Consume(ConsumeContext<SubmitOrder> context)
            {
                await context.Publish<OrderSubmitted>(context.Message);
            }
        }


        class OrderSaga :
            ISaga,
            InitiatedBy<OrderSubmitted>
        {
            public string OrderNumber { get; set; }

            public Task Consume(ConsumeContext<OrderSubmitted> context)
            {
                OrderNumber = context.Message.OrderNumber;

                return Task.CompletedTask;
            }

            public Guid CorrelationId { get; set; }
        }


        public interface SubmitOrder
        {
            Guid CorrelationId { get; }
            string OrderNumber { get; }
        }


        public interface OrderSubmitted :
            CorrelatedBy<Guid>
        {
            string OrderNumber { get; }
        }
    }


    public class Common_Mediator_FilterScope<TContainer> :
        CommonContainerTestFixture<TContainer>
        where TContainer : ITestFixtureContainerFactory, new()
    {
        [Test]
        public async Task Should_use_the_same_scope_for_consume_and_send()
        {
            await Mediator.Send<EasyA>(new { InVar.CorrelationId });

            await _easyASource.Task;
            await _easyBSource.Task;

            var context = await _scopedContextSource.Task.OrTimeout(TimeSpan.FromSeconds(1));

            await context.ConsumeContext.Task.OrTimeout(TimeSpan.FromSeconds(1));

            await context.ConsumeContextEasyA.Task.OrTimeout(TimeSpan.FromSeconds(1));

            await context.SendContext.Task.OrTimeout(TimeSpan.FromSeconds(1));

            Assert.ThrowsAsync<TimeoutException>(async () => await context.ConsumeContextEasyB.Task.OrTimeout(100));
        }

        readonly TaskCompletionSource<ConsumeContext<EasyA>> _easyASource;
        readonly TaskCompletionSource<ConsumeContext<EasyB>> _easyBSource;
        readonly TaskCompletionSource<ScopedContext> _scopedContextSource;

        public Common_Mediator_FilterScope()
        {
            _scopedContextSource = GetTask<ScopedContext>();
            _easyASource = GetTask<ConsumeContext<EasyA>>();
            _easyBSource = GetTask<ConsumeContext<EasyB>>();
        }

        IMediator Mediator => ServiceProvider.GetRequiredService<IMediator>();

        protected override IServiceCollection ConfigureServices(IServiceCollection collection)
        {
            return collection
                .AddMediator(ConfigureRegistration)
                .AddSingleton(_easyASource)
                .AddSingleton(_easyBSource)
                .AddSingleton(_scopedContextSource)
                .AddScoped<ScopedContext>()
                .AddScoped(typeof(ScopedConsumeFilter<>))
                .AddScoped(typeof(ScopedSendFilter<>));
        }

        void ConfigureRegistration(IMediatorRegistrationConfigurator configurator)
        {
            configurator.AddConsumer<EasyAConsumer>();
            configurator.AddConsumer<EasyBConsumer>();

            configurator.ConfigureMediator((context, cfg) =>
            {
                cfg.UseConsumeFilter(typeof(ScopedConsumeFilter<>), context);
                cfg.UseSendFilter(typeof(ScopedSendFilter<>), context);
            });
        }


        protected class EasyAConsumer :
            IConsumer<EasyA>
        {
            readonly TaskCompletionSource<ConsumeContext<EasyA>> _received;
            readonly ScopedContext _scopedContext;
            readonly TaskCompletionSource<ScopedContext> _scopedContextCompletionSource;

            public EasyAConsumer(TaskCompletionSource<ConsumeContext<EasyA>> received, ScopedContext scopedContext,
                TaskCompletionSource<ScopedContext> scopedContextCompletionSource)
            {
                _received = received;
                _scopedContext = scopedContext;
                _scopedContextCompletionSource = scopedContextCompletionSource;
            }

            public async Task Consume(ConsumeContext<EasyA> context)
            {
                _received.TrySetResult(context);
                _scopedContext.ConsumeContextEasyA.TrySetResult(context);

                await context.Send(context.ReceiveContext.InputAddress, new EasyB());

                _scopedContextCompletionSource.TrySetResult(_scopedContext);
            }
        }


        protected class EasyBConsumer :
            IConsumer<EasyB>
        {
            readonly TaskCompletionSource<ConsumeContext<EasyB>> _received;
            readonly ScopedContext _scopedContext;

            public EasyBConsumer(TaskCompletionSource<ConsumeContext<EasyB>> received, ScopedContext scopedContext)
            {
                _received = received;
                _scopedContext = scopedContext;
            }

            public async Task Consume(ConsumeContext<EasyB> context)
            {
                _received.TrySetResult(context);
                _scopedContext.ConsumeContextEasyB.TrySetResult(context);
            }
        }
    }


    public class EasyA
    {
        public Guid CorrelationId { get; set; }
    }


    public class EasyB
    {
        public Guid CorrelationId { get; set; }
    }


    public class ScopedContext
    {
        public ScopedContext(InMemoryTestHarness harness)
        {
            ConsumeContext = harness.GetTask<ConsumeContext>();
            ConsumeContextEasyA = harness.GetTask<ConsumeContext<EasyA>>();
            ConsumeContextEasyB = harness.GetTask<ConsumeContext<EasyB>>();
            SendContext = harness.GetTask<SendContext>();
        }

        public TaskCompletionSource<ConsumeContext> ConsumeContext { get; }
        public TaskCompletionSource<ConsumeContext<EasyA>> ConsumeContextEasyA { get; }
        public TaskCompletionSource<ConsumeContext<EasyB>> ConsumeContextEasyB { get; }
        public TaskCompletionSource<SendContext> SendContext { get; }
    }


    class ScopedConsumeFilter<T> :
        IFilter<ConsumeContext<T>>
        where T : class
    {
        readonly ScopedContext _scopedContext;

        public ScopedConsumeFilter(ScopedContext scopedContext)
        {
            _scopedContext = scopedContext;
        }

        public Task Send(ConsumeContext<T> context, IPipe<ConsumeContext<T>> next)
        {
            _scopedContext.ConsumeContext.TrySetResult(context);

            return next.Send(context);
        }

        public void Probe(ProbeContext context)
        {
        }
    }


    class ScopedSendFilter<T> :
        IFilter<SendContext<T>>
        where T : class
    {
        readonly ScopedContext _scopedContext;

        public ScopedSendFilter(ScopedContext scopedContext)
        {
            _scopedContext = scopedContext;
        }

        public Task Send(SendContext<T> context, IPipe<SendContext<T>> next)
        {
            _scopedContext.SendContext.TrySetResult(context);

            return next.Send(context);
        }

        public void Probe(ProbeContext context)
        {
        }
    }
}
