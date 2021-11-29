namespace MassTransit.Containers.Tests.Common_Tests
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Courier;
    using Courier.Contracts;
    using Discovery;
    using NUnit.Framework;
    using TestFramework;
    using TestFramework.Messages;


    namespace Discovery
    {
        using System;
        using System.Threading.Tasks;
        using TestFramework.Messages;


        public class DiscoveryTypes
        {
        }


        public class DiscoveryPingConsumer :
            IConsumer<PingMessage>
        {
            public async Task Consume(ConsumeContext<PingMessage> context)
            {
                await context.Publish(new PingReceived { CorrelationId = context.Message.CorrelationId });

                await Task.Delay(1000);

                await context.RespondAsync(new PongMessage(context.Message.CorrelationId));
            }
        }


        public class ByzantineFilter<T> :
            IFilter<T>
            where T : class, PipeContext
        {
            public Task Send(T context, IPipe<T> next)
            {
                return Task.CompletedTask;
            }

            public void Probe(ProbeContext context)
            {
            }
        }


        public class PingReceived :
            CorrelatedBy<Guid>
        {
            public Guid CorrelationId { get; set; }
        }


        public class PingAcknowledged :
            CorrelatedBy<Guid>
        {
            public Guid CorrelationId { get; set; }
        }


        public class PingCompleted :
            CorrelatedBy<Guid>
        {
            public Guid CorrelationId { get; set; }
        }


        public class DiscoveryPingSaga :
            ISaga,
            InitiatedBy<PingReceived>,
            Orchestrates<PingAcknowledged>
        {
            public Task Consume(ConsumeContext<PingReceived> context)
            {
                return Task.CompletedTask;
            }

            public Guid CorrelationId { get; set; }

            public Task Consume(ConsumeContext<PingAcknowledged> context)
            {
                return Task.CompletedTask;
            }
        }


        public class PingSagaDefinition :
            SagaDefinition<DiscoveryPingSaga>
        {
            protected override void ConfigureSaga(IReceiveEndpointConfigurator endpointConfigurator, ISagaConfigurator<DiscoveryPingSaga> sagaConfigurator)
            {
                var partition = endpointConfigurator.CreatePartitioner(Environment.ProcessorCount);

                sagaConfigurator.Message<PingReceived>(x => x.UsePartitioner(partition, m => m.Message.CorrelationId));
                sagaConfigurator.Message<PingAcknowledged>(x => x.UsePartitioner(partition, m => m.Message.CorrelationId));
            }
        }


        public class DiscoveryPingState :
            SagaStateMachineInstance
        {
            public State CurrentState { get; set; }
            public Guid CorrelationId { get; set; }
        }


        public class DiscoveryPingStateMachine :
            MassTransitStateMachine<DiscoveryPingState>
        {
            public DiscoveryPingStateMachine()
            {
                Initially(
                    When(Received)
                        .TransitionTo(Pinged));

                During(Pinged,
                    When(Acknowledged)
                        .Publish(context => new PingCompleted { CorrelationId = context.Data.CorrelationId })
                        .TransitionTo(Ponged));
            }

            public State Pinged { get; set; }
            public State Ponged { get; set; }
            public Event<PingReceived> Received { get; set; }
            public Event<PingAcknowledged> Acknowledged { get; set; }
        }


        public class PingStateMachineDefinition :
            SagaDefinition<DiscoveryPingState>
        {
            public PingStateMachineDefinition()
            {
                EndpointName = "discovery-ping-state";
            }

            protected override void ConfigureSaga(IReceiveEndpointConfigurator endpointConfigurator, ISagaConfigurator<DiscoveryPingState> sagaConfigurator)
            {
                var partition = endpointConfigurator.CreatePartitioner(Environment.ProcessorCount);

                sagaConfigurator.Message<PingReceived>(x => x.UsePartitioner(partition, m => m.Message.CorrelationId));
                sagaConfigurator.Message<PingAcknowledged>(x => x.UsePartitioner(partition, m => m.Message.CorrelationId));
            }
        }


        public class PingConsumerDefinition :
            ConsumerDefinition<DiscoveryPingConsumer>
        {
            public PingConsumerDefinition()
            {
                EndpointName = "ping-queue";

                ConcurrentMessageLimit = 1;
            }

            protected override void ConfigureConsumer(IReceiveEndpointConfigurator endpointConfigurator,
                IConsumerConfigurator<DiscoveryPingConsumer> consumerConfigurator)
            {
            }
        }


        public interface PingArguments
        {
            Guid CorrelationId { get; }
        }


        public interface PingLog
        {
            Guid CorrelationId { get; }
        }


        public class PingActivity :
            IActivity<PingArguments, PingLog>
        {
            public async Task<ExecutionResult> Execute(ExecuteContext<PingArguments> context)
            {
                return context.Completed<PingLog>(new { context.Arguments.CorrelationId });
            }

            public async Task<CompensationResult> Compensate(CompensateContext<PingLog> context)
            {
                return context.Compensated();
            }
        }


        public class PingSecondActivity :
            IExecuteActivity<PingArguments>
        {
            public async Task<ExecutionResult> Execute(ExecuteContext<PingArguments> context)
            {
                return context.Completed();
            }
        }


        public class DiscoveryPongConsumer :
            IConsumer<PongMessage>
        {
            public async Task Consume(ConsumeContext<PongMessage> context)
            {
            }
        }
    }


    public class ReceiveEndpointConfigurationObserver :
        IEndpointConfigurationObserver
    {
        readonly HashSet<string> _endpoints;

        public ReceiveEndpointConfigurationObserver()
        {
            _endpoints = new HashSet<string>();
        }

        public void EndpointConfigured<T>(T configurator)
            where T : IReceiveEndpointConfigurator
        {
            _endpoints.Add(configurator.InputAddress.AbsolutePath.Split('/').Last());
        }

        public bool WasConfigured(string name)
        {
            return _endpoints.Contains(name);
        }
    }


    public class Common_Discovery<TContainer> :
        CommonContainerTestFixture<TContainer>
        where TContainer : ITestFixtureContainerFactory, new()
    {
        [Test]
        public async Task Should_complete_the_routing_slip()
        {
            Task<ConsumeContext<RoutingSlipCompleted>> completed = SubscribeHandler<RoutingSlipCompleted>();

            var builder = new RoutingSlipBuilder(NewId.NextGuid());
            builder.AddSubscription(Bus.Address, RoutingSlipEvents.Completed);
            builder.AddActivity("Ping", new Uri("loopback://localhost/Ping_execute"));
            builder.AddActivity("PingSecond", new Uri("loopback://localhost/PingSecond_execute"));

            await Bus.Execute(builder.Build());

            ConsumeContext<RoutingSlipCompleted> routingSlipCompleted = await completed;
            Assert.That(routingSlipCompleted.Message.TrackingNumber, Is.EqualTo(builder.TrackingNumber));
        }

        [Test]
        public void Should_have_properly_configured_every_endpoint()
        {
            Assert.That(_endpointObserver.WasConfigured("ping-queue"));
            Assert.That(_endpointObserver.WasConfigured("discovery-ping-state"));
            Assert.That(_endpointObserver.WasConfigured("Ping_execute"));
            Assert.That(_endpointObserver.WasConfigured("Ping_compensate"));
            Assert.That(_endpointObserver.WasConfigured("DiscoveryPing"));

            // TODO, verify but the harness configures them anyway so
            // Assert.That(_endpointObserver.WasConfigured("DiscoveryPong"), Is.False);
        }

        [Test]
        public async Task Should_receive_the_response_from_the_consumer()
        {
            Task<ConsumeContext<PingReceived>> received = await ConnectPublishHandler<PingReceived>();
            Task<ConsumeContext<PingCompleted>> completed = await ConnectPublishHandler<PingCompleted>();

            IRequestClient<PingMessage> client = GetRequestClient<PingMessage>();

            var pingMessage = new PingMessage();

            Response<PongMessage> response = await client.GetResponse<PongMessage>(pingMessage);

            await Bus.Publish(new PingAcknowledged { CorrelationId = response.Message.CorrelationId });

            ConsumeContext<PingReceived> pingReceived = await received;
            Assert.That(pingReceived.Message.CorrelationId, Is.EqualTo(pingMessage.CorrelationId));

            ConsumeContext<PingCompleted> pingCompleted = await completed;
            Assert.That(pingCompleted.Message.CorrelationId, Is.EqualTo(pingMessage.CorrelationId));
        }

        ReceiveEndpointConfigurationObserver _endpointObserver;

        protected override void ConfigureMassTransit(IBusRegistrationConfigurator configurator)
        {
            configurator.SetInMemorySagaRepositoryProvider();

            configurator.AddConsumersFromNamespaceContaining(typeof(DiscoveryTypes));
            configurator.AddSagaStateMachinesFromNamespaceContaining(typeof(DiscoveryTypes));
            configurator.AddSagasFromNamespaceContaining(typeof(DiscoveryTypes));
            configurator.AddActivitiesFromNamespaceContaining(typeof(DiscoveryTypes));

            configurator.AddRequestClient<PingMessage>(new Uri("loopback://localhost/ping-queue"));
        }

        protected override void ConfigureInMemoryBus(IInMemoryBusFactoryConfigurator configurator)
        {
            _endpointObserver = new ReceiveEndpointConfigurationObserver();
            configurator.ConnectEndpointConfigurationObserver(_endpointObserver);

            configurator.ConfigureEndpoints(BusRegistrationContext, filter => filter.Exclude<DiscoveryPongConsumer>());
        }
    }
}
