// Copyright 2007-2019 Chris Patterson, Dru Sellers, Travis Smith, et. al.
//
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use
// this file except in compliance with the License. You may obtain a copy of the
// License at
//
//     http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software distributed
// under the License is distributed on an "AS IS" BASIS, WITHOUT WARRANTIES OR
// CONDITIONS OF ANY KIND, either express or implied. See the License for the
// specific language governing permissions and limitations under the License.
namespace MassTransit.Containers.Tests.Common_Tests
{
    using System;
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
        using Automatonymous;
        using ConsumeConfigurators;
        using Courier;
        using Definition;
        using GreenPipes;
        using Saga;
        using TestFramework.Messages;


        public class DiscoveryTypes
        {
        }


        public class DiscoveryPingConsumer :
            IConsumer<PingMessage>
        {
            public async Task Consume(ConsumeContext<PingMessage> context)
            {
                await context.Publish(new PingReceived() {CorrelationId = context.Message.CorrelationId});

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
            public Guid CorrelationId { get; set; }

            public Task Consume(ConsumeContext<PingReceived> context)
            {
                return Task.CompletedTask;
            }

            public Task Consume(ConsumeContext<PingAcknowledged> context)
            {
                return Task.CompletedTask;
            }
        }


        public class PingSagaDefinition :
            SagaDefinition<DiscoveryPingSaga>
        {
            public PingSagaDefinition()
            {
            }

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
            public Guid CorrelationId { get; set; }
            public State CurrentState { get; set; }
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
                        .Publish(context => new PingCompleted() {CorrelationId = context.Data.CorrelationId})
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

                Request<PingMessage>(x =>
                {
                    x.PartitionBy(m => m.CorrelationId);

                    x.Responds<PongMessage>();
                });
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
                return context.Completed<PingLog>(new {context.Arguments.CorrelationId});
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
    }


    public abstract class Common_Discovery :
        InMemoryTestFixture
    {
        [Test]
        public async Task Should_receive_the_response_from_the_consumer()
        {
            var received = ConnectPublishHandler<PingReceived>();
            var completed = ConnectPublishHandler<PingCompleted>();

            var client = GetRequestClient();

            var pingMessage = new PingMessage();

            var response = await client.GetResponse<PongMessage>(pingMessage);

            await Bus.Publish(new PingAcknowledged() {CorrelationId = response.Message.CorrelationId});

            var pingReceived = await received;
            Assert.That(pingReceived.Message.CorrelationId, Is.EqualTo(pingMessage.CorrelationId));

            var pingCompleted = await completed;
            Assert.That(pingCompleted.Message.CorrelationId, Is.EqualTo(pingMessage.CorrelationId));
        }

        [Test]
        public async Task Should_complete_the_routing_slip()
        {
            var completed = SubscribeHandler<RoutingSlipCompleted>();

            var builder = new RoutingSlipBuilder(NewId.NextGuid());
            builder.AddSubscription(Bus.Address, RoutingSlipEvents.Completed);
            builder.AddActivity("Ping", new Uri("loopback://localhost/Ping_execute"));
            builder.AddActivity("PingSecond", new Uri("loopback://localhost/PingSecond_execute"));

            await Bus.Execute(builder.Build());

            var routingSlipCompleted = await completed;
            Assert.That(routingSlipCompleted.Message.TrackingNumber, Is.EqualTo(builder.TrackingNumber));
        }

        protected abstract IRequestClient<PingMessage> GetRequestClient();

        protected override void ConfigureInMemoryBus(IInMemoryBusFactoryConfigurator configurator)
        {
            ConfigureEndpoints(configurator);
        }

        protected abstract void ConfigureEndpoints(IInMemoryBusFactoryConfigurator configurator);
    }
}
