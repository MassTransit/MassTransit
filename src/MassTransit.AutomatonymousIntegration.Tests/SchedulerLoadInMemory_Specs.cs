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
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Automatonymous;
    using NUnit.Framework;
    using Saga;


    [TestFixture]
    public class When_processing_a_lot_of_saga_instances :
        StateMachineTestFixture
    {
        [Test]
        public async Task Should_remove_the_saga_once_completed()
        {
            var tasks = new List<Task<ConsumeContext<Stopped>>>();

            for (var i = 0; i < 20; i++)
            {
                var correlationId = NewId.NextGuid();

                tasks.Add(ConnectPublishHandler<Stopped>(context => context.Message.CorrelationId == correlationId));

                await InputQueueSendEndpoint.Send(new Start {CorrelationId = correlationId});
            }

            await Task.WhenAll(tasks.ToArray());

            await Task.Delay(1000);

            Assert.AreEqual(0, _repository.Count);
        }

        protected override void ConfigureInMemoryReceiveEndpoint(IInMemoryReceiveEndpointConfigurator configurator)
        {
            configurator.StateMachineSaga(_machine, _repository);
        }

        readonly TestStateMachine _machine;
        readonly InMemorySagaRepository<Instance> _repository;

        public When_processing_a_lot_of_saga_instances()
        {
            _machine = new TestStateMachine();
            _repository = new InMemorySagaRepository<Instance>();
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

            public Guid? ScheduleTokenId { get; set; }
            public Guid CorrelationId { get; set; }
        }


        class TestStateMachine :
            MassTransitStateMachine<Instance>
        {
            public TestStateMachine()
            {
                Schedule(() => StopSchedule, x => x.ScheduleTokenId, x => x.Delay = TimeSpan.FromMilliseconds(100));

                Initially(
                    When(Started)
                        .Schedule(StopSchedule, context => new Stop {CorrelationId = context.Instance.CorrelationId})
                        .Then(context => Console.WriteLine($"Started: {context.Instance.CorrelationId}"))
                        .TransitionTo(Running));

                During(Running,
                    When(Stopped)
                        .Publish(context => new Stopped {CorrelationId = context.Instance.CorrelationId})
                        .Then(context => Console.WriteLine($"Stopped: {context.Instance.CorrelationId}"))
                        .Finalize());

                SetCompletedWhenFinalized();
            }

            public Schedule<Instance, Stop> StopSchedule { get; private set; }

            public State Running { get; private set; }
            public Event<Start> Started { get; private set; }
            public Event<Stop> Stopped { get; private set; }
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


        public class Stopped :
            CorrelatedBy<Guid>
        {
            public Guid CorrelationId { get; set; }
        }
    }
}