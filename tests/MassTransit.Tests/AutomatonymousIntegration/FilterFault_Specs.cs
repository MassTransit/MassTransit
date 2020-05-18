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
    using System.Threading.Tasks;
    using Automatonymous;
    using GreenPipes;
    using MassTransit.Saga;
    using MassTransit.Testing;
    using NUnit.Framework;
    using TestFramework;


    [TestFixture]
    public class When_an_exception_is_filtered :
        InMemoryTestFixture
    {
        [Test]
        public async Task Should_be_able_to_observe_its_own_event_fault()
        {
            Task<ConsumeContext<Fault<Start>>> faulted = ConnectPublishHandler<Fault<Start>>();

            var message = new Initialize();
            await InputQueueSendEndpoint.Send(message);

            Guid? saga = await _repository.ShouldContainSaga(x => x.CorrelationId == message.CorrelationId
                && GetCurrentState(x) == _machine.WaitingToStart, TimeSpan.FromSeconds(8));
            Assert.IsTrue(saga.HasValue);

            await InputQueueSendEndpoint.Send(new Start(message.CorrelationId));

            await faulted;

            saga = await _repository.ShouldContainSaga(x => x.CorrelationId == message.CorrelationId
                && GetCurrentState(x) == _machine.WaitingToStart && x.StartAttempts == 1, TimeSpan.FromSeconds(8));
            Assert.IsTrue(saga.HasValue);

        }

        protected override void ConfigureInMemoryReceiveEndpoint(IInMemoryReceiveEndpointConfigurator configurator)
        {
            _machine = new TestStateMachine();
            _repository = new InMemorySagaRepository<Instance>();

            configurator.UseRetry(x =>
            {
                x.Ignore<NotSupportedException>();
                x.Immediate(2);
            });

            configurator.StateMachineSaga(_machine, _repository);
        }

        State GetCurrentState(Instance state)
        {
            return _machine.GetState(state).Result;
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

            public int StartAttempts { get; set; }
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
                        .Then(context =>
                        {
                            context.Instance.StartAttempts++;
                            throw new NotSupportedException("This is expected, but nonetheless exceptional");
                        })
                        .TransitionTo(Running),
                    When(Initialized)
                        .TransitionTo(WaitingToStart));

                During(WaitingToStart,
                    When(Started)
                        .Then(instance =>
                        {
                            instance.Instance.StartAttempts++;
                            throw new NotSupportedException("This is expected, but nonetheless exceptional");
                        })
                        .TransitionTo(Running));

                During(Running,
                    When(Stopped)
                        .TransitionTo(Complete));
            }

            public State WaitingToStart { get; private set; }
            public State Running { get; private set; }
            public State Complete { get; private set; }

            public Event<Start> Started { get; private set; }
            public Event<Initialize> Initialized { get; private set; }
            public Event<Stop> Stopped { get; private set; }
        }


        public class Start :
            CorrelatedBy<Guid>
        {
            public Start()
            {
                CorrelationId = NewId.NextGuid();
            }

            public Start(Guid correlationId)
            {
                CorrelationId = correlationId;
            }

            public Guid CorrelationId { get; set; }
        }


        public class Stop :
            CorrelatedBy<Guid>
        {
            public Stop()
            {
                CorrelationId = NewId.NextGuid();
            }

            public Stop(Guid correlationId)
            {
                CorrelationId = correlationId;
            }

            public Guid CorrelationId { get; set; }
        }


        public class Initialize :
            CorrelatedBy<Guid>
        {
            public Initialize()
            {
                CorrelationId = NewId.NextGuid();
            }

            public Guid CorrelationId { get; set; }
        }
    }
}