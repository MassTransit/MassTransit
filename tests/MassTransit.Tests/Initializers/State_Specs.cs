namespace MassTransit.Tests.Initializers
{
    using System;
    using System.Threading.Tasks;
    using Automatonymous;
    using MassTransit.Saga;
    using NUnit.Framework;
    using TestFramework;


    [TestFixture]
    public class Initializing_a_property_using_state_machine_state :
        InMemoryTestFixture
    {
        [Test]
        public async Task Should_handle_a_double_state()
        {
            var sagaId = Guid.NewGuid();

            Task<ConsumeContext<StateUpdated>> handler = await ConnectPublishHandler<StateUpdated>(x => x.CorrelationId == sagaId);

            await Bus.Publish(new Start {CorrelationId = sagaId});

            ConsumeContext<StateUpdated> stateUpdated = await handler;

            Assert.That(stateUpdated.Message.CurrentState, Is.EqualTo(_machine.Running.Name));
        }

        protected override void ConfigureInMemoryReceiveEndpoint(IInMemoryReceiveEndpointConfigurator configurator)
        {
            configurator.StateMachineSaga(_machine, _repository);
        }

        readonly IntStateMachine _machine;
        readonly InMemorySagaRepository<IntInstance> _repository;

        public Initializing_a_property_using_state_machine_state()
        {
            _machine = new IntStateMachine();
            _repository = new InMemorySagaRepository<IntInstance>();
        }


        class IntInstance :
            SagaStateMachineInstance
        {
            public IntInstance(Guid correlationId)
            {
                CorrelationId = correlationId;
            }

            protected IntInstance()
            {
            }

            public int CurrentState { get; set; }
            public Guid CorrelationId { get; set; }
        }


        sealed class IntStateMachine :
            MassTransitStateMachine<IntInstance>
        {
            public IntStateMachine()
            {
                InstanceState(x => x.CurrentState, Running);

                Event(() => Started);
                Event(() => Stopped);

                Initially(
                    When(Started)
                        .TransitionTo(Running)
                        .PublishAsync(context => context.Init<StateUpdated>(new
                        {
                            context.Instance.CorrelationId,
                            CurrentState = this.GetState(context.Instance)
                        }))
                );

                During(Running,
                    When(Stopped)
                        .Finalize());
            } // ReSharper disable UnassignedGetOnlyAutoProperty
            public State Running { get; }
            public Event<Start> Started { get; }
            public Event<Stop> Stopped { get; }
        }


        class StateUpdated
        {
            public Guid CorrelationId { get; set; }
            public string CurrentState { get; set; }
        }


        class Start :
            CorrelatedBy<Guid>
        {
            public Guid CorrelationId { get; set; }
        }


        class Stop :
            CorrelatedBy<Guid>
        {
            public Guid CorrelationId { get; set; }
        }
    }
}
