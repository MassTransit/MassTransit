// Copyright 2011 Chris Patterson, Dru Sellers
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
namespace MassTransit.AutomatonymousTests
{
    using System;
    using Automatonymous;
    using NUnit.Framework;
    using Saga;


//    [TestFixture]
//    public class Using_a_simple_state_machine :
//        MassTransitTestFixture
//    {
//        [Test]
//        public void Should_handle_a_double_state()
//        {
//            Guid sagaId = Guid.NewGuid();
//
//            Bus.Publish(new Start
//                {
//                    CorrelationId = sagaId
//                });
//
//            Instance instance = _repository.ShouldContainSaga(sagaId, 8.Seconds());
//            Assert.IsNotNull(instance);
//
//            Bus.Publish(new Stop
//                {
//                    CorrelationId = sagaId
//                });
//
//            instance = _repository.ShouldContainSagaInState(sagaId, _machine.Final, _machine, 8.Seconds());
//            Assert.IsNotNull(instance);
//        }
//
//        [Test]
//        public void Should_handle_the_initial_state()
//        {
//            Guid sagaId = Guid.NewGuid();
//
//            Bus.Publish(new Start
//                {
//                    CorrelationId = sagaId
//                });
//
//            Instance instance = _repository.ShouldContainSagaInState(sagaId, _machine.Running, _machine, 8.Seconds());
//            Assert.IsNotNull(instance);
//        }
//
//        protected override void ConfigureSubscriptions(SubscriptionBusServiceConfigurator configurator)
//        {
//            configurator.StateMachineSaga(_machine, _repository);
//        }
//
//        TestStateMachine _machine;
//        readonly InMemorySagaRepository<Instance> _repository;
//
//        public Using_a_simple_state_machine()
//        {
//            _machine = new TestStateMachine();
//            _repository = new InMemorySagaRepository<Instance>();
//        }
//
//
//        class Instance :
//            SagaStateMachineInstance
//        {
//            public Instance(Guid correlationId)
//            {
//                CorrelationId = correlationId;
//            }
//
//            protected Instance()
//            {
//            }
//
//            public State CurrentState { get; set; }
//            public Guid CorrelationId { get; set; }
//            public IServiceBus Bus { get; set; }
//        }
//
//
//        class TestStateMachine :
//            AutomatonymousStateMachine<Instance>
//        {
//            public TestStateMachine()
//            {
//                InstanceState(x => x.CurrentState);
//
//                State(() => Running);
//                Event(() => Started);
//                Event(() => Stopped);
//                Event(() => ShouldNotBind);
//
//                Initially(
//                    When(Started)
//                        .TransitionTo(Running));
//
//                During(Running,
//                    When(Stopped)
//                        .Finalize());
//            }
//
//            public State Running { get; private set; }
//            public Event<Start> Started { get; private set; }
//            public Event<Stop> Stopped { get; private set; }
//            public Event<int> ShouldNotBind { get; private set; }
//        }
//
//
//        class Start :
//            CorrelatedBy<Guid>
//        {
//            public Guid CorrelationId { get; set; }
//        }
//
//
//        class Stop :
//            CorrelatedBy<Guid>
//        {
//            public Guid CorrelationId { get; set; }
//        }
//    }
}