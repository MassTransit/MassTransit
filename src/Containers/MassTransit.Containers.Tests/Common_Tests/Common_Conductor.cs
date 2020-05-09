namespace MassTransit.Containers.Tests.Common_Tests
{
    using System;
    using System.Threading.Tasks;
    using Conductor.Configuration;
    using ConductorComponents;
    using ConductorContracts;
    using Definition;
    using NUnit.Framework;
    using TestFramework;


    [TestFixture(true)]
    [TestFixture(false)]
    public abstract class Common_Conductor :
        InMemoryTestFixture
    {
        protected Common_Conductor(bool instanceEndpoint)
        {
            Options = new ServiceInstanceOptions();

            if (instanceEndpoint)
                Options.EnableInstanceEndpoint();
        }

        protected ServiceInstanceOptions Options { get; private set; }

        [Test]
        public async Task Should_resolve_an_use_the_service()
        {
            var clientFactory = GetClientFactory();

            var client = clientFactory.CreateRequestClient<SubmitOrder>();

            var response = await client.GetResponse<OrderSubmissionAccepted>(new
            {
                OrderId = NewId.NextGuid(),
                InVar.Timestamp
            });

            Assert.That(response.SourceAddress, Is.EqualTo(new Uri(Host.Address, KebabCaseEndpointNameFormatter.Instance.Consumer<SubmitOrderConsumer>())));
        }

        protected override void ConfigureInMemoryBus(IInMemoryBusFactoryConfigurator configurator)
        {
            ConfigureServiceEndpoints(configurator);
        }

        protected abstract void ConfigureServiceEndpoints(IReceiveConfigurator<IInMemoryReceiveEndpointConfigurator> configurator);

        protected abstract IClientFactory GetClientFactory();

        protected void ConfigureRegistration<T>(IRegistrationConfigurator<T> configurator)
            where T : class
        {
            configurator.SetKebabCaseEndpointNameFormatter();

            configurator.AddConsumersFromNamespaceContaining<SubmitOrderConsumer>();

            configurator.AddServiceClient();

            configurator.AddRequestClient<SubmitOrder>();
            configurator.AddRequestClient<AuthorizeOrder>();

            configurator.AddBus(provider => BusControl);
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
                var authorization = await _authorizeClient.GetResponse<OrderAuthorized>(new
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
