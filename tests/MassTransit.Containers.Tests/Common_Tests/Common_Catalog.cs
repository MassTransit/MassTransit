namespace MassTransit.Containers.Tests.Common_Tests
{
    using System.Threading.Tasks;
    using DirectoryComponents;
    using CatalogContracts;
    using Conductor;
    using Conductor.Directory;
    using NUnit.Framework;
    using TestComponents.ForkJoint.Consumers;
    using TestComponents.ForkJoint.Contracts;
    using TestFramework;
    using TestFramework.Messages;


    public abstract class Common_Catalog :
        InMemoryTestFixture
    {
        protected abstract IServiceDirectory Directory { get; }
        protected abstract IOrchestrationProvider ExecutorFactory { get; }
        protected abstract IBusRegistrationContext Registration { get; }
        protected abstract ServiceDirectoryConfigurator ServiceDirectoryConfigurator { get; }

        protected abstract IRequestClient<T> GetRequestClient<T>()
            where T : class;

        [Test]
        public async Task Should_complete_the_request()
        {
            IRequestClient<PrepareOrder> client = GetRequestClient<PrepareOrder>();

            using RequestHandle<PrepareOrder> requestHandle = client.Create(new
            {
                OrderId = NewId.NextGuid(),
                Fry = new
                {
                    FryId = NewId.NextGuid(),
                    Size = Size.Large,
                }
            });

            requestHandle.TimeToLive = default;

            Response<OrderReady> response = await requestHandle.GetResponse<OrderReady>();
        }

        [Test]
        public void Should_add_a_service_definition()
        {
            IOrchestration<PingMessage, PongMessage> orchestration = ExecutorFactory.GetOrchestration<PingMessage, PongMessage>();
        }

        [Test]
        public void Should_add_a_two_step_service_definition()
        {
            IOrchestration<BlingMessage, PongMessage> orchestration = ExecutorFactory.GetOrchestration<BlingMessage, PongMessage>();
        }

        protected override void ConfigureInMemoryBus(IInMemoryBusFactoryConfigurator configurator)
        {
            ServiceDirectoryConfigurator.Connect(configurator);

            configurator.ConfigureEndpoints(Registration);
        }

        protected void ConfigureRegistration(IBusRegistrationConfigurator configurator)
        {
            configurator.SetKebabCaseEndpointNameFormatter();

            configurator.AddConsumer<CookFryConsumer, CookFryConsumerDefinition>();
            configurator.AddConsumer<PrepareOrderConsumer, PrepareOrderConsumerDefinition>();

            configurator.AddConsumer<PingConsumer, PingConsumerDefinition>();
            configurator.AddConsumer<BlingConsumer, BlingConsumerDefinition>();


            configurator.AddBus(provider => BusControl);
        }
    }


    namespace CatalogContracts
    {
        using System;
        using TestComponents.ForkJoint.Contracts;


        public interface PrepareOrder
        {
            Guid OrderId { get; }
            Fry Fry { get; }
        }


        public interface OrderReady
        {
            Guid OrderId { get; }
            FryCompleted Fry { get; }
        }
    }


    namespace DirectoryComponents
    {
        using System;
        using CatalogContracts;
        using Conductor;
        using Conductor.Orchestration;
        using Definition;
        using TestComponents.ForkJoint.Contracts;
        using TestFramework.Messages;


        class PingConsumerDefinition :
            ConsumerDefinition<PingConsumer>,
            IConfigureServiceDirectory
        {
            public PingConsumerDefinition()
            {
                ConcurrentMessageLimit = 64;
            }

            public void Configure(IServiceDirectoryConfigurator configurator)
            {
                configurator.AddService<PingMessage, PongMessage>(x => x.Consumer<PingConsumer>(), definition =>
                {
                    definition.PartitionBy(x => x.CorrelationId);
                });
            }
        }


        class BlingConsumerDefinition :
            ConsumerDefinition<BlingConsumer>,
            IConfigureServiceDirectory
        {
            public BlingConsumerDefinition()
            {
                ConcurrentMessageLimit = 64;
            }

            public void Configure(IServiceDirectoryConfigurator configurator)
            {
                configurator.AddService<BlingMessage, PingMessage>(x => x.Consumer<BlingConsumer>(), definition =>
                {
                    definition.PartitionBy(x => x.CorrelationId);
                });
            }
        }


        class BlingMessage
        {
            public Guid CorrelationId { get; set; }
        }


        class BlingConsumer :
            IConsumer<BlingMessage>
        {
            public Task Consume(ConsumeContext<BlingMessage> context)
            {
                return context.RespondAsync(new PingMessage(context.Message.CorrelationId));
            }
        }


        class PingConsumer :
            IConsumer<PingMessage>
        {
            public Task Consume(ConsumeContext<PingMessage> context)
            {
                return context.RespondAsync(new PongMessage(context.Message.CorrelationId));
            }
        }


        public class PrepareOrderConsumer :
            IConsumer<PrepareOrder>
        {
            readonly IOrchestration<Fry, FryCompleted> _orchestration;

            public PrepareOrderConsumer(IOrchestrationProvider orchestrationProvider)
            {
                _orchestration = orchestrationProvider.GetOrchestration<Fry, FryCompleted>();
            }

            public async Task Consume(ConsumeContext<PrepareOrder> context)
            {
                var planContext = new ConsumeOrchestrationContext<PrepareOrder>(context, context.Message);

                var fryCompleted = await _orchestration.Execute(planContext.Push(context.Message.Fry));

                await context.RespondAsync<OrderReady>(new
                {
                    context.Message.OrderId,
                    FryCompletd = fryCompleted
                });
            }
        }


        public class PrepareOrderConsumerDefinition :
            ConsumerDefinition<PrepareOrderConsumer>,
            IConfigureServiceDirectory
        {
            public void Configure(IServiceDirectoryConfigurator configurator)
            {
                configurator.AddService<PrepareOrder, OrderReady>(x => x.Consumer<PrepareOrderConsumer>());

                configurator.AddMessageInitializer<Fry, CookFry>(x => new
                {
                    x.Select<PrepareOrder>().OrderId,
                    OrderLineId = x.Data.FryId,
                    x.Data.Size
                });

                configurator.AddMessageInitializer<FryReady, FryCompleted>(x => new
                {
                    Created = DateTime.UtcNow,
                    Completed = DateTime.UtcNow,
                    x.Data.OrderId,
                    x.Data.OrderLineId,
                    x.Data.Size,
                    Description = $"{x.Data.Size} Fries"
                });
            }
        }
    }
}
