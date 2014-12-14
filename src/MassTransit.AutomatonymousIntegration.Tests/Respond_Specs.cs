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
    using Magnum;
    using Magnum.Extensions;
    using NUnit.Framework;
    using Saga;
    using SubscriptionConfigurators;


    [TestFixture]
    public class Responding_from_within_a_saga :
        MassTransitTestFixture
    {
        [Test]
        public void Should_receive_the_response_message()
        {
            var responseReceived = new FutureMessage<StartupComplete>();

            Bus.PublishRequest(new Start(), x =>
                {
                    x.Handle<StartupComplete>(responseReceived.Set);
                    x.HandleTimeout(8.Seconds(), () => { });
                });

            Assert.IsTrue(responseReceived.IsAvailable(0.Seconds()));
        }

        protected override void ConfigureSubscriptions(SubscriptionBusServiceConfigurator configurator)
        {
            _machine = new TestStateMachine();
            _repository = new InMemorySagaRepository<Instance>();

            configurator.StateMachineSaga(_machine, _repository);
        }

        TestStateMachine _machine;
        InMemorySagaRepository<Instance> _repository;

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

                Initially(
                    When(Started)
                        .Respond(new StartupComplete())
                        .TransitionTo(Running));
            }

            public State Running { get; private set; }
            public Event<Start> Started { get; private set; }
        }


        class Start :
            CorrelatedBy<Guid>
        {
            public Start()
            {
                CorrelationId = CombGuid.Generate();
            }

            public Guid CorrelationId { get; set; }
        }


        class StartupComplete
        {
        }
    }
}