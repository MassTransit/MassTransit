namespace MassTransit.Containers.Tests.Common_Tests
{
    using System;
    using System.Threading.Tasks;
    using Contracts;
    using Microsoft.Extensions.DependencyInjection;
    using NUnit.Framework;
    using TestFramework;


    public class Using_the_request_client_across_consumers<TContainer> :
        CommonContainerTestFixture<TContainer>
        where TContainer : ITestFixtureContainerFactory, new()
    {
        [Test]
        public async Task Should_receive_the_response()
        {
            IRequestClient<InitialRequest> client = GetRequestClient<InitialRequest>();

            _correlationId = NewId.NextGuid();

            Response<InitialResponse> response = await client.GetResponse<InitialResponse>(new
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

        public Using_the_request_client_across_consumers()
        {
            SubsequentQueueName = "subsequent_queue";
            SubsequentQueueAddress = new Uri($"queue:{SubsequentQueueName}");
        }

        protected Uri SubsequentQueueAddress { get; }
        string SubsequentQueueName { get; }

        protected override void ConfigureMassTransit(IBusRegistrationConfigurator configurator)
        {
            configurator.AddConsumer<InitialConsumer>();
            configurator.AddConsumer<SubsequentConsumer>();

            configurator.AddRequestClient<InitialRequest>(InputQueueAddress);
            configurator.AddRequestClient(typeof(SubsequentRequest), SubsequentQueueAddress);
        }

        protected override void ConfigureInMemoryBus(IInMemoryBusFactoryConfigurator configurator)
        {
            configurator.ReceiveEndpoint(SubsequentQueueName, cfg =>
            {
                cfg.ConfigureConsumer<SubsequentConsumer>(BusRegistrationContext);
            });
        }

        protected override void ConfigureInMemoryReceiveEndpoint(IInMemoryReceiveEndpointConfigurator configurator)
        {
            configurator.ConfigureConsumer<InitialConsumer>(BusRegistrationContext);
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
    }


    public class Using_the_generic_request_client_provider<TContainer> :
        CommonContainerTestFixture<TContainer>
        where TContainer : ITestFixtureContainerFactory, new()
    {
        [Test]
        public async Task Should_receive_the_response()
        {
            IRequestClient<InitialRequest> client = GetRequestClient<InitialRequest>();

            var correlationId = NewId.NextGuid();

            Response<InitialResponse> response = await client.GetResponse<InitialResponse>(new
            {
                CorrelationId = correlationId,
                Value = "World"
            });

            Assert.That(response.Message.Value, Is.EqualTo("Hello, World"));
        }

        protected override void ConfigureInMemoryReceiveEndpoint(IInMemoryReceiveEndpointConfigurator configurator)
        {
            configurator.ConfigureConsumer<InitialConsumer>(BusRegistrationContext);
        }

        protected override void ConfigureMassTransit(IBusRegistrationConfigurator configurator)
        {
            configurator.AddConsumer<InitialConsumer>();
            configurator.AddConsumer<SubsequentConsumer>();
        }

        protected override IServiceCollection ConfigureServices(IServiceCollection collection)
        {
            return base.ConfigureServices(collection)
                .AddGenericRequestClient();
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
                return context.RespondAsync<SubsequentResponse>(new { Value = $"Hello, {context.Message.Value}" });
            }
        }
    }


    public class Using_the_scoped_client_factory_in_a_consumer<TContainer> :
        CommonContainerTestFixture<TContainer>
        where TContainer : ITestFixtureContainerFactory, new()
    {
        [Test]
        public async Task Should_receive_the_response()
        {
            IRequestClient<InitialRequest> client = GetRequestClient<InitialRequest>();

            _correlationId = NewId.NextGuid();

            Response<InitialResponse> response = await client.GetResponse<InitialResponse>(new
            {
                CorrelationId = _correlationId,
                Value = "World"
            });

            Assert.That(response.Message.Value, Is.EqualTo("Hello, World"));
        }

        Guid _correlationId;

        protected override void ConfigureMassTransit(IBusRegistrationConfigurator configurator)
        {
            configurator.AddConsumer<InitialConsumer>();
            configurator.AddConsumer<SubsequentConsumer>();
            configurator.AddRequestClient<InitialRequest>();
        }

        protected override void ConfigureInMemoryReceiveEndpoint(IInMemoryReceiveEndpointConfigurator configurator)
        {
            configurator.ConfigureConsumer<InitialConsumer>(BusRegistrationContext);
        }


        class InitialConsumer :
            IConsumer<InitialRequest>
        {
            readonly IRequestClient<SubsequentRequest> _client;

            public InitialConsumer(IScopedClientFactory clientFactory)
            {
                _client = clientFactory.CreateRequestClient<SubsequentRequest>();
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
                return context.RespondAsync<SubsequentResponse>(new { Value = $"Hello, {context.Message.Value}" });
            }
        }
    }


    namespace Contracts
    {
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
}
