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
    using Testing;


    [TestFixture]
    public class Using_the_testing_framework_built_into_masstransit
    {
        [Test]
        public void Should_handle_the_initial_state()
        {
            Guid sagaId = Guid.NewGuid();

            SagaTest<BusTestScenario, Instance> test = TestFactory.ForSaga<Instance>().New(x =>
                {
                    x.UseStateMachineBuilder(_machine, sm =>
                        {
                            sm.Correlate(_machine.Stopped, (s, m) => s.CorrelationId == m.CorrelationId);
                        });

                    x.Publish(new Start
                        {
                            CorrelationId = sagaId
                        });
                });

            test.Execute();

            Assert.IsTrue(test.Received.Any<Start>(), "Message not received");

            Instance instance = test.Saga.Created.Contains(sagaId);
            Assert.IsNotNull(instance, "Saga instance not found");

            Assert.AreEqual(instance.CurrentState, _machine.Running);
        }

        [Test]
        public void Should_handle_the_stop_state()
        {
            Guid sagaId = Guid.NewGuid();

            SagaTest<BusTestScenario, Instance> test = TestFactory.ForSaga<Instance>().New(x =>
                {
                    x.UseStateMachineBuilder(_machine, sm =>
                        {
                            sm.Correlate(_machine.Stopped, (s, m) => s.CorrelationId == m.CorrelationId);
                        });

                    x.Publish(new Start
                        {
                            CorrelationId = sagaId
                        });
                    x.Publish(new Stop
                        {
                            CorrelationId = sagaId
                        });
                });

            test.Execute();

            Assert.IsTrue(test.Received.Any<Start>(), "Start not received");
            Assert.IsTrue(test.Received.Any<Stop>(), "Stop not received");

            Instance instance = test.Saga.Created.Contains(sagaId);
            Assert.IsNotNull(instance, "Saga instance not found");

            Assert.AreEqual(instance.CurrentState, _machine.Final);
        }


        TestStateMachine _machine;

        public Using_the_testing_framework_built_into_masstransit()
        {
            _machine = new TestStateMachine();
        }


        class Instance :
            SagaStateMachineInstance
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
            public IServiceBus Bus { get; set; }
        }


        class TestStateMachine :
            AutomatonymousStateMachine<Instance>
        {
            public TestStateMachine()
            {
                InstanceState(x => x.CurrentState);

                State(() => Running);
                Event(() => Started);
                Event(() => Stopped);
                Event(() => ShouldNotBind);

                Initially(
                    When(Started)
                        .TransitionTo(Running));

                During(Running,
                    When(Stopped)
                        .Finalize());
            }

            public State Running { get; private set; }
            public Event<Start> Started { get; private set; }
            public Event<Stop> Stopped { get; private set; }
            public Event<int> ShouldNotBind { get; private set; }
        }


        class Start :
            CorrelatedBy<Guid>
        {
            public Guid CorrelationId { get; set; }
        }


        class Stop
        {
            public Guid CorrelationId { get; set; }
        }
    }
}