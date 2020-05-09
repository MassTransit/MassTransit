namespace MassTransit.Containers.Tests.Common_Tests
{
    using System;
    using System.Threading.Tasks;
    using GreenPipes.Internals.Extensions;
    using Mediator;
    using NUnit.Framework;
    using Saga;
    using Scenarios;
    using Shouldly;
    using TestFramework;
    using Testing;
    using Util;


    public abstract class Common_Mediator :
        InMemoryTestFixture
    {
        [Test]
        public async Task Should_dispatch_to_the_consumer()
        {
            const string name = "Joe";

            await Mediator.Send(new SimpleMessageClass(name));

            SimplerConsumer lastConsumer = await SimplerConsumer.LastConsumer.OrCanceled(TestCancellationToken);
            lastConsumer.ShouldNotBe(null);

            await lastConsumer.Last.OrCanceled(TestCancellationToken);
        }

        protected abstract IMediator Mediator { get; }

        protected void ConfigureRegistration<T>(IRegistrationConfigurator<T> configurator)
            where T : class
        {
            configurator.AddConsumer<SimplerConsumer>();

            configurator.AddMediator();
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

        protected void ConfigureRegistration<T>(IRegistrationConfigurator<T> configurator)
            where T : class
        {
            configurator.AddConsumer<InitialConsumer>();
            configurator.AddConsumer<SubsequentConsumer>();

            configurator.AddRequestClient<InitialRequest>();
            configurator.AddRequestClient<SubsequentRequest>();
            configurator.AddMediator();
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


    public abstract class Common_Mediator_Saga :
        InMemoryTestFixture
    {
        Guid _correlationId;

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

        protected abstract IMediator Mediator { get; }

        protected abstract ISagaRepository<T> GetSagaRepository<T>()
            where T : class, ISaga;

        protected void ConfigureRegistration<T>(IRegistrationConfigurator<T> configurator)
            where T : class
        {
            configurator.AddConsumer<OrderConsumer>();
            configurator.AddSaga<OrderSaga>()
                .InMemoryRepository();

            configurator.AddMediator();
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
            public Task Consume(ConsumeContext<OrderSubmitted> context)
            {
                OrderNumber = context.Message.OrderNumber;

                return TaskUtil.Completed;
            }

            public Guid CorrelationId { get; set; }
            public string OrderNumber { get; set; }
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
}
