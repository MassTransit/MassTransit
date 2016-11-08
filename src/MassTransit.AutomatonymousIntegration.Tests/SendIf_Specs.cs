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
namespace MassTransit.AutomatonymousIntegration.Tests
{
    using System;
    using System.Threading.Tasks;
    using Automatonymous;
    using GreenPipes;
    using NUnit.Framework;
    using Saga;
    using TestFramework;


    [TestFixture]
    public class Sending_a_message_from_a_state_machine_if :
        InMemoryTestFixture
    {
        [Test]
        public async Task Should_receive_the_published_message()
        {
            var message = new Start();

            await InputQueueSendEndpoint.Send(message);

            ConsumeContext<StartupComplete> received = await _handled;

            Guid? saga =
                await _repository.ShouldContainSaga(x => x.CorrelationId == message.CorrelationId && Equals(x.CurrentState, _machine.Running), TestTimeout);

            Assert.IsTrue(saga.HasValue);
            Assert.IsNotNull(received);
        }

        [Test]
        public async Task Should_not_receive_message()
        {
            var message = new Start();

            await InputQueueSendEndpoint.Send(message);

            Task.WaitAll(new [] { _notHandled }, 2000);

            Guid? saga =
                await _repository.ShouldContainSaga(x => x.CorrelationId == message.CorrelationId && Equals(x.CurrentState, _machine.Running), TestTimeout);

            Assert.IsTrue(saga.HasValue);

            Assert.AreEqual(TaskStatus.WaitingForActivation, _notHandled.Status);

        }

        [Test]
        public void Should_return_a_wonderful_breakdown_of_the_guts_inside_it()
        {
            var result = Bus.GetProbeResult();

            Console.WriteLine(result.ToJsonString());
        }

        protected override void ConfigureBus(IInMemoryBusFactoryConfigurator configurator)
        {
            base.ConfigureBus(configurator);

            configurator.ReceiveEndpoint("observer", e =>
            {
                _handlerAddress = e.InputAddress;

                _handled = Handled<StartupComplete>(e);
                _notHandled = Handled<NeverSent>(e);
            });
        }

        protected override void ConfigureInputQueueEndpoint(IInMemoryReceiveEndpointConfigurator configurator)
        {
            _machine = new TestStateMachine(_handlerAddress);
            _repository = new InMemorySagaRepository<Instance>();

            configurator.StateMachineSaga(_machine, _repository);
        }

        TestStateMachine _machine;
        InMemorySagaRepository<Instance> _repository;

        Task<ConsumeContext<StartupComplete>> _handled;
        Task<ConsumeContext<NeverSent>> _notHandled;
        Uri _handlerAddress;


        class Instance :
            SagaStateMachineInstance
        {
            public State CurrentState { get; set; }
            public Guid CorrelationId { get; set; }
        }


        class TestStateMachine :
            MassTransitStateMachine<Instance>
        {
            public TestStateMachine(Uri sendAddress)
            {
                InstanceState(x => x.CurrentState);

                Initially(
                    When(Started)
                        .SendIf(sendAddress, context => new StartupComplete
                        {
                            TransactionId = context.Data.CorrelationId
                        }, context => true)
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

        class NeverSent { }
    }
}