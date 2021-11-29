namespace MassTransit.Containers.Tests.Common_Tests
{
    using System;
    using System.Threading.Tasks;
    using ConductorComponents;
    using ConductorContracts;
    using NUnit.Framework;
    using TestFramework;


    public class Common_Conductor<TContainer> :
        CommonContainerTestFixture<TContainer>
        where TContainer : ITestFixtureContainerFactory, new()
    {
        [Test]
        public async Task Should_resolve_an_use_the_service()
        {
            var clientFactory = GetClientFactory();

            IRequestClient<SubmitOrder> client = clientFactory.CreateRequestClient<SubmitOrder>();

            Response<OrderSubmissionAccepted> response = await client.GetResponse<OrderSubmissionAccepted>(new
            {
                OrderId = NewId.NextGuid(),
                InVar.Timestamp
            });

            Assert.That(response.SourceAddress,
                Is.EqualTo(new Uri(InMemoryTestHarness.BaseAddress, KebabCaseEndpointNameFormatter.Instance.Consumer<SubmitOrderConsumer>())));
        }

        public Common_Conductor()
        {
            Options = new ServiceInstanceOptions();
        }

        protected ServiceInstanceOptions Options { get; private set; }

        protected override void ConfigureMassTransit(IBusRegistrationConfigurator configurator)
        {
            configurator.SetKebabCaseEndpointNameFormatter();

            configurator.AddConsumersFromNamespaceContaining<SubmitOrderConsumer>();

            configurator.AddRequestClient<SubmitOrder>();
            configurator.AddRequestClient<AuthorizeOrder>();
        }

        protected override void ConfigureInMemoryBus(IInMemoryBusFactoryConfigurator configurator)
        {
            configurator.ConfigureServiceEndpoints(BusRegistrationContext, Options);
        }
    }


    namespace ConductorContracts
    {
        using System;


        public interface SubmitOrder
        {
            Guid OrderId { get; }
            DateTime Timestamp { get; }
        }


        public interface OrderSubmissionAccepted
        {
            Guid OrderId { get; }
            DateTime Timestamp { get; }
        }


        public interface AuthorizeOrder
        {
            Guid OrderId { get; }
            DateTime Timestamp { get; }
        }


        public interface OrderAuthorized
        {
            Guid OrderId { get; }
            DateTime Timestamp { get; }
        }
    }


    namespace ConductorComponents
    {
        using ConductorContracts;


        public class SubmitOrderConsumer :
            IConsumer<SubmitOrder>
        {
            readonly IRequestClient<AuthorizeOrder> _authorizeClient;

            public SubmitOrderConsumer(IRequestClient<AuthorizeOrder> authorizeClient)
            {
                _authorizeClient = authorizeClient;
            }

            public async Task Consume(ConsumeContext<SubmitOrder> context)
            {
                Response<OrderAuthorized> authorization = await _authorizeClient.GetResponse<OrderAuthorized>(new
                {
                    context.Message.OrderId,
                    InVar.Timestamp
                });

                await context.RespondAsync<OrderSubmissionAccepted>(new
                {
                    authorization.Message.OrderId,
                    InVar.Timestamp
                });
            }
        }


        public class AuthorizeOrderConsumer :
            IConsumer<AuthorizeOrder>
        {
            public Task Consume(ConsumeContext<AuthorizeOrder> context)
            {
                return context.RespondAsync<OrderAuthorized>(new
                {
                    context.Message.OrderId,
                    InVar.Timestamp
                });
            }
        }
    }
}
