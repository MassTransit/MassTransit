namespace MassTransit.Tests.SagaStateMachineTests
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using MassTransit.Saga;
    using MassTransit.Testing;
    using NUnit.Framework;
    using TestFramework;


    [TestFixture]
    public class When_an_event_is_defined_as_ignored_for_state :
        InMemoryTestFixture
    {
        [Test]
        public async Task Should_not_throw_an_exception_when_receiving_ignored_event()
        {
            var sagaId = Guid.NewGuid();

            await Bus.Publish(new Start {CorrelationId = sagaId});

            Guid? saga = await _repository.ShouldContainSagaInState(sagaId, _machine, x => x.Running, TestTimeout);
            Assert.IsTrue(saga.HasValue);

            await Bus.Publish(new Start {CorrelationId = sagaId});

            var faultMessage = await GetFaultMessage(TimeSpan.FromSeconds(3));
            Assert.IsNull(faultMessage?.Exceptions.Select(ex => $"{ex.ExceptionType}: {ex.Message}").First());
        }

        protected async Task<Fault> GetFaultMessage(TimeSpan timeout)
        {
            var giveUpAt = DateTime.Now + timeout;

            while (DateTime.Now < giveUpAt)
            {
                if (_faultMessageContexts.Any())
                    return _faultMessageContexts.First().Message;

                await Task.Delay(10).ConfigureAwait(false);
            }

            return null;
        }

        protected override void ConfigureInMemoryReceiveEndpoint(IInMemoryReceiveEndpointConfigurator configurator)
        {
            configurator.StateMachineSaga(_machine, _repository);
        }

        protected override void ConnectObservers(IBus bus)
        {
            Bus.ConnectHandler((MessageHandler<Fault>)(context =>
            {
                _faultMessageContexts.Add(context);
                return Task.FromResult(0);
            }));
        }

        readonly TestStateMachine _machine;
        readonly InMemorySagaRepository<Instance> _repository;
        readonly List<ConsumeContext<Fault>> _faultMessageContexts;

        public When_an_event_is_defined_as_ignored_for_state()
        {
            _machine = new TestStateMachine();
            _repository = new InMemorySagaRepository<Instance>();
            _faultMessageContexts = new List<ConsumeContext<Fault>>();
        }


        class Instance : SagaStateMachineInstance
        {
            public Instance(Guid correlationId)
            {
                CorrelationId = correlationId;
            }

            protected Instance()
            {
            }

            public State CurrentState { get; set; }
            public Guid CorrelationId { get; set; }
        }


        class TestStateMachine : MassTransitStateMachine<Instance>
        {
            public TestStateMachine()
            {
                Initially(
                    When(Started)
                        .TransitionTo(Running));

                During(Running,
                    When(Stopped)
                        .Finalize(),
                    Ignore(Started));

                SetCompletedWhenFinalized();
            }

            public State Running { get; private set; }
            public Event<Start> Started { get; private set; }
            public Event<Stop> Stopped { get; private set; }
        }


        class Start : CorrelatedBy<Guid>
        {
            public Guid CorrelationId { get; set; }
        }


        class Stop : CorrelatedBy<Guid>
        {
            public Guid CorrelationId { get; set; }
        }
    }
}
