namespace MassTransit.Azure.ServiceBus.Core.Tests
{
    using System;
    using System.Threading.Tasks;
    using Microsoft.Extensions.DependencyInjection;
    using NUnit.Framework;


    [TestFixture]
    public class Sending_a_request_from_a_state_machine :
        AzureServiceBusTestFixture
    {
        [Test]
        public async Task Should_handle_timeout_using_schedule()
        {
            Task<ConsumeContext<OrderCompleted>> completed = await ConnectPublishHandler<OrderCompleted>();

            await InputQueueSendEndpoint.Send(new OrderSubmitted
            {
                OrderId = NewId.NextGuid(),
                OrderNumber = "ORDER123",
                OrderTimestamp = DateTimeOffset.Now,
            });

            await completed;

            await Task.Delay(1000);
        }

        readonly ServiceProvider _provider;

        public Sending_a_request_from_a_state_machine()
            : base("saga_input_queue_session")
        {
            _provider = new ServiceCollection()
                .AddMassTransit(ConfigureRegistration)
                .BuildServiceProvider(true);
        }

        void ConfigureRegistration(IBusRegistrationConfigurator configurator)
        {
            configurator.AddConsumer<OrderVerificationConsumer>();

            configurator.AddSagaStateMachine<OrderShipmentStateMachine, OrderShipmentState, OrderShipmentSagaDefinition>()
                .MessageSessionRepository();

            configurator.AddBus(provider => BusControl);
        }

        protected override void ConfigureServiceBusBus(IServiceBusBusFactoryConfigurator configurator)
        {
            configurator.Send<OrderSubmitted>(s => s.UseSessionIdFormatter(c => c.Message.OrderId.ToString("D")));
            configurator.Send<VerifyOrder>(s => s.UseSessionIdFormatter(c => c.Message.OrderId.ToString("D")));
            configurator.Send<OrderVerified>(s => s.UseSessionIdFormatter(c => c.Message.OrderId.ToString("D")));
            configurator.Send<VerifyOrderTimeout>(s => s.UseSessionIdFormatter(c => c.Message.OrderId.ToString("D")));

            base.ConfigureServiceBusBus(configurator);
        }

        [OneTimeTearDown]
        public async Task CloseContainer()
        {
            await _provider.DisposeAsync();
        }

        protected override void ConfigureServiceBusReceiveEndpoint(IServiceBusReceiveEndpointConfigurator configurator)
        {
            configurator.UseInMemoryOutbox();

            var context = _provider.GetRequiredService<IBusRegistrationContext>();

            configurator.ConfigureSagas(context);
            configurator.ConfigureConsumers(context);
        }


        public class OrderShipmentState :
            SagaStateMachineInstance
        {
            public string CurrentState { get; set; }

            public Guid? TimeoutTokenId { get; set; }

            public Guid CorrelationId { get; set; }
        }


        public class OrderShipmentStateMachine :
            MassTransitStateMachine<OrderShipmentState>
        {
            public OrderShipmentStateMachine()
            {
                InstanceState(x => x.CurrentState);

                Event(() => Submitted, x => x.CorrelateById(context => context.Message.OrderId));

                Request(() => VerifyOrderRequest, x =>
                {
                    x.Timeout = TimeSpan.Zero;
                });

                Schedule(() => VerificationTimeoutExpired, x => x.TimeoutTokenId, x =>
                {
                    x.Received = configurator => configurator.CorrelateById(p => p.Message.OrderId);
                    x.Delay = TimeSpan.FromSeconds(10);
                });

                Initially(
                    When(Submitted)
                        .Request(VerifyOrderRequest, c => c.Init<VerifyOrder>(new { c.Message.OrderId }))
                        .Schedule(VerificationTimeoutExpired, context => context.Init<VerifyOrderTimeout>(new { context.Message.OrderId })),
                    When(VerifyOrderRequest.Faulted)
                        .Unschedule(VerificationTimeoutExpired)
                        .Then(x => Console.WriteLine("Faulted")),
                    When(VerifyOrderRequest.Completed)
                        .Unschedule(VerificationTimeoutExpired)
                        .PublishAsync(context => context.Init<OrderCompleted>(new { context.Message.OrderId }))
                );
            }

            //
            // ReSharper disable UnassignedGetOnlyAutoProperty
            public Request<OrderShipmentState, VerifyOrder, OrderVerified> VerifyOrderRequest { get; }
            public Schedule<OrderShipmentState, VerifyOrderTimeout> VerificationTimeoutExpired { get; }
            public Event<OrderSubmitted> Submitted { get; }
        }


        public class OrderSubmitted
        {
            public Guid OrderId { get; set; }
            public DateTimeOffset OrderTimestamp { get; set; }

            public string OrderNumber { get; set; }
        }


        public class OrderCompleted
        {
            public Guid OrderId { get; set; }
        }


        public class VerifyOrder
        {
            public Guid OrderId { get; set; }
        }


        public class OrderVerified
        {
            public Guid OrderId { get; set; }
        }


        public class VerifyOrderTimeout
        {
            public Guid OrderId { get; set; }
        }


        public class OrderVerificationConsumer :
            IConsumer<VerifyOrder>
        {
            public async Task Consume(ConsumeContext<VerifyOrder> context)
            {
                await context.RespondAsync<OrderVerified>(new { context.Message.OrderId });
            }
        }


        public class OrderShipmentSagaDefinition :
            SagaDefinition<OrderShipmentState>
        {
            protected override void ConfigureSaga(IReceiveEndpointConfigurator endpointConfigurator, ISagaConfigurator<OrderShipmentState> sagaConfigurator)
            {
                if (endpointConfigurator is IServiceBusReceiveEndpointConfigurator sb)
                    sb.RequiresSession = true;
            }
        }
    }
}
