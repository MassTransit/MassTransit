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
    using Magnum.Extensions;
    using NUnit.Framework;
    using Saga;
    using SubscriptionConfigurators;


    [TestFixture, Explicit("Still figuring out the request syntax")]
    public class Publishing_a_request_from_a_saga_state_machine :
        MassTransitTestFixture
    {
        [Test]
        public void Should_receive_the_published_message()
        {
            var messageReceived = new FutureMessage<RequestAuthorization>();

            Bus.SubscribeHandler<RequestAuthorization>(messageReceived.Set);

            var message = new ScheduleAppointment();
            Bus.Publish(message);

            Assert.IsTrue(messageReceived.IsAvailable(8.Seconds()));
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
                        .PublishRequest((instance, context) => new RequestAuthorization
                            {
                                User = context.Message.User
                            },
                            (instance, message, x) =>
                                {
                                    x.HandleTimeout(30.Seconds(), () => { });
                                    x.Handle<AuthorizationGranted>(m => { });
                                })
                        .TransitionTo(Running));
            }

            public State Running { get; private set; }
            public Event<ScheduleAppointment> Started { get; private set; }
        }


        class ScheduleAppointment :
            CorrelatedBy<Guid>
        {
            public ScheduleAppointment()
            {
                CorrelationId = NewId.NextGuid();
            }

            public string User { get; set; }
            public Guid CorrelationId { get; private set; }
        }


        class RequestAuthorization
        {
            public string User { get; set; }
        }


        class AuthorizationGranted
        {
        }


        class AuthorizationDenied
        {
        }
    }
}