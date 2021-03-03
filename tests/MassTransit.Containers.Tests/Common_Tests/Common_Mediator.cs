namespace MassTransit.Containers.Tests.Common_Tests
{
    using System;
    using System.Threading.Tasks;
    using Context;
    using GreenPipes;
    using GreenPipes.Internals.Extensions;
    using Mediator;
    using NUnit.Framework;
    using Registration;
    using Saga;
    using Scenarios;
    using Shouldly;
    using TestFramework;
    using TestFramework.Logging;
    using Testing;
    using Util;


    public abstract class Common_Mediator :
        InMemoryTestFixture
    {
        protected abstract IMediator Mediator { get; }

        [Test]
        public async Task Should_dispatch_to_the_consumer()
        {
            const string name = "Joe";

            await Mediator.Send(new SimpleMessageClass(name));

            var lastConsumer = await SimplerConsumer.LastConsumer.OrCanceled(TestCancellationToken);
            lastConsumer.ShouldNotBe(null);

            await lastConsumer.Last.OrCanceled(TestCancellationToken);
        }

        protected void ConfigureRegistration(IMediatorRegistrationConfigurator configurator)
        {
            configurator.AddConsumer<SimplerConsumer>();
        }
    }


    public abstract class Common_Mediator_Bus :
        InMemoryTestFixture
    {
        protected abstract IMediator Mediator { get; }
        protected abstract IBusRegistrationContext Registration { get; }

        [Test]
        public async Task Should_transfer_message_headers()
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

        protected override void ConfigureInMemoryReceiveEndpoint(IInMemoryReceiveEndpointConfigurator configurator)
        {
        }

        protected void ConfigureRegistration(IMediatorRegistrationConfigurator configurator)
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


    public abstract class Common_Mediator_Request :
        InMemoryTestFixture
    {
        Guid _correlationId;

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

        protected abstract IRequestClient<T> GetRequestClient<T>()
            where T : class;

        protected void ConfigureRegistration(IMediatorRegistrationConfigurator configurator)
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


    public abstract class Common_Mediator_Request_Filter :
        InMemoryTestFixture
    {
        [Test]
        public async Task Should_receive_the_response()
        {

            Response<Pong> response = await GetRequestClient<Ping>().GetResponse<Pong>(new Ping());

            Assert.That(response.Message.Message, Is.EqualTo("PONG!"));
        }

        protected abstract IRequestClient<T> GetRequestClient<T>()
            where T : class;

        protected abstract void ConfigureFilters(IMediatorRegistrationContext context, IMediatorConfigurator configurator);

        protected void ConfigureRegistration(IMediatorRegistrationConfigurator configurator)
        {
            configurator.AddConsumer<PingConsumer>();

            configurator.AddRequestClient<Ping>();

            configurator.ConfigureMediator((context, cfg) =>
            {
                LogContext.ConfigureCurrentLogContext(LoggerFactory);
                ConfigureFilters(context, cfg);
            });
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


        public class PingConsumer :
            IConsumer<Ping>
        {
            public async Task Consume(ConsumeContext<Ping> context)
            {
                await context.RespondAsync(new Pong {Message = "Pong!"});
            }
        }


        public class Ping
        {
        }


        public class Pong
        {
            public string Message { get; set; }
        }
    }


    public abstract class Common_Mediator_Saga :
        InMemoryTestFixture
    {
        Guid _correlationId;

        protected abstract IMediator Mediator { get; }

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

        protected abstract ISagaRepository<T> GetSagaRepository<T>()
            where T : class, ISaga;

        protected void ConfigureRegistration(IMediatorRegistrationConfigurator configurator)
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

                return TaskUtil.Completed;
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


    public abstract class Common_Mediator_FilterScope :
        BusTestFixture
    {
        protected readonly TaskCompletionSource<ConsumeContext<EasyA>> EasyASource;
        protected readonly TaskCompletionSource<ConsumeContext<EasyB>> EasyBSource;
        protected readonly TaskCompletionSource<ScopedContext> ScopedContextSource;

        protected Common_Mediator_FilterScope()
            : base(new InMemoryTestHarness())
        {
            ScopedContextSource = GetTask<ScopedContext>();
            EasyASource = GetTask<ConsumeContext<EasyA>>();
            EasyBSource = GetTask<ConsumeContext<EasyB>>();
        }

        protected abstract IMediator Mediator { get; }

        [Test]
        public async Task Should_use_the_same_scope_for_consume_and_send()
        {
            await Mediator.Send<EasyA>(new {InVar.CorrelationId});

            await EasyASource.Task;
            await EasyBSource.Task;

            var context = await ScopedContextSource.Task.OrTimeout(TimeSpan.FromSeconds(1));

            await context.ConsumeContext.Task.OrTimeout(TimeSpan.FromSeconds(1));

            await context.ConsumeContextEasyA.Task.OrTimeout(TimeSpan.FromSeconds(1));

            await context.SendContext.Task.OrTimeout(TimeSpan.FromSeconds(1));

            Assert.ThrowsAsync<TimeoutException>(async () => await context.ConsumeContextEasyB.Task.OrTimeout(100));
        }

        protected abstract void ConfigureFilters(IMediatorRegistrationContext context, IMediatorConfigurator configurator);

        protected void ConfigureRegistration(IMediatorRegistrationConfigurator configurator)
        {
            configurator.AddConsumer<EasyAConsumer>();
            configurator.AddConsumer<EasyBConsumer>();

            configurator.ConfigureMediator((context, cfg) =>
            {
                ConfigureFilters(context, cfg);
            });
        }


        public class EasyA
        {
            public Guid CorrelationId { get; set; }
        }


        public class EasyB
        {
            public Guid CorrelationId { get; set; }
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


        public class ScopedContext
        {
            public ScopedContext(AsyncTestHarness harness)
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


        protected class ScopedConsumeFilter<T> :
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


        protected class ScopedSendFilter<T> :
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
}
