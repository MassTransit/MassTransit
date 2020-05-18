// Copyright 2007-2016 Chris Patterson, Dru Sellers, Travis Smith, et. al.
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
namespace MassTransit.Tests.AutomatonymousIntegration
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Automatonymous;
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

            await Bus.Publish(new Start
            {
                CorrelationId = sagaId
            });

            Guid? saga = await _repository.ShouldContainSaga(x => x.CorrelationId == sagaId
                && x.CurrentState == _machine.Running, TestTimeout);
            Assert.IsTrue(saga.HasValue);

            await Bus.Publish(new Start
            {
                CorrelationId = sagaId
            });

            var faultMessage = await GetFaultMessage(TimeSpan.FromSeconds(3));
            Assert.IsNull(faultMessage?.Exceptions.Select(ex => $"{ex.ExceptionType}: {ex.Message}").First());
        }

        protected async Task<Fault> GetFaultMessage(TimeSpan timeout)
        {
            var giveUpAt = DateTime.Now + timeout;

            while (DateTime.Now < giveUpAt)
            {
                if (_faultMessageContexts.Any())
                {
                    return _faultMessageContexts.First().Message;
                }

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