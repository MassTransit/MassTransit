// Copyright 2007-2015 Chris Patterson, Dru Sellers, Travis Smith, et. al.
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
    using System.Threading.Tasks;
    using Automatonymous;
    using GreenPipes;
    using GreenPipes.Introspection;
    using MassTransit.Saga;
    using MassTransit.Testing;
    using NUnit.Framework;
    using TestFramework;


    [TestFixture]
    public class Publishing_a_message_from_a_saga_state_machine :
        InMemoryTestFixture
    {
        protected override void ConfigureInMemoryReceiveEndpoint(IInMemoryReceiveEndpointConfigurator configurator)
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
            public State CurrentState { get; set; }
            public Guid CorrelationId { get; set; }
        }


        class TestStateMachine :
            MassTransitStateMachine<Instance>
        {
            public TestStateMachine()
            {
                InstanceState(x => x.CurrentState);

                Initially(
                    When(Started)
                        .Publish(context => new StartupComplete
                        {
                            TransactionId = context.Data.CorrelationId
                        })
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
                CorrelationId = NewId.NextGuid();
            }

            public Guid CorrelationId { get; private set; }
        }


        class StartupComplete
        {
            public Guid TransactionId { get; set; }
        }


        [Test]
        public async Task Should_receive_the_published_message()
        {
            Task<ConsumeContext<StartupComplete>> messageReceived = ConnectPublishHandler<StartupComplete>();

            var message = new Start();

            await InputQueueSendEndpoint.Send(message);

            ConsumeContext<StartupComplete> received = await messageReceived;

            Assert.AreEqual(message.CorrelationId, received.Message.TransactionId);

            Assert.IsTrue(received.InitiatorId.HasValue, "The initiator should be copied from the CorrelationId");

            Assert.AreEqual(received.InitiatorId.Value, message.CorrelationId, "The initiator should be the saga CorrelationId");

            Assert.AreEqual(received.SourceAddress, InputQueueAddress, "The published message should have the input queue source address");

            Guid? saga =
                await _repository.ShouldContainSaga(x => x.CorrelationId == message.CorrelationId && Equals(x.CurrentState, _machine.Running), TestTimeout);

            Assert.IsTrue(saga.HasValue);
        }

        [Test, Explicit]
        public void Should_return_a_wonderful_breakdown_of_the_guts_inside_it()
        {
            ProbeResult result = Bus.GetProbeResult();
            
            Console.WriteLine(result.ToJsonString());
        }
    }
}