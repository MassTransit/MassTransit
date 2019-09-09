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
    using MassTransit.Saga;
    using NUnit.Framework;
    using Shouldly;
    using TestFramework;


    [TestFixture]
    public class Responding_from_within_a_saga :
        InMemoryTestFixture
    {
        [Test]
        public async Task Should_receive_the_response_message()
        {
            StartupComplete complete = await _client.Request(new Start(), TestCancellationToken);
        }

        [Test]
        public async Task Should_start_and_report_status()
        {
            var start = new Start();

            StartupComplete complete = await _client.Request(start, TestCancellationToken);

            var status = await _statusClient.Request(new StatusRequested(start.CorrelationId), TestCancellationToken);

            status.Status.ShouldBe(_machine.Running.Name);
        }

        [Test]
        public void Should_fault_on_a_missing_instance()
        {
            Assert.That(
                async () => await _statusClient.Request(new StatusRequested(NewId.NextGuid()), TestCancellationToken), 
                Throws.TypeOf<RequestFaultException>());
        }

        [OneTimeSetUp]
        public void Setup()
        {
            _client = new MessageRequestClient<Start, StartupComplete>(Bus, InputQueueAddress, TestTimeout);
            _statusClient = new MessageRequestClient<StatusRequested, StatusReport>(Bus, InputQueueAddress, TestTimeout);
        }

        protected override void ConfigureInMemoryReceiveEndpoint(IInMemoryReceiveEndpointConfigurator configurator)
        {
            _machine = new TestStateMachine();
            _repository = new InMemorySagaRepository<Instance>();

            configurator.StateMachineSaga(_machine, _repository);
        }

        TestStateMachine _machine;
        InMemorySagaRepository<Instance> _repository;
        IRequestClient<Start, StartupComplete> _client;
        IRequestClient<StatusRequested, StatusReport> _statusClient;


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
        }


        class TestStateMachine :
            MassTransitStateMachine<Instance>
        {
            public TestStateMachine()
            {
                Event(() => Requested, x => x.OnMissingInstance(m => m.Fault()));
                Initially(
                    When(Started)
                        .Respond(new StartupComplete())
                        .TransitionTo(Running));

                DuringAny(
                    When(Requested)
                        .Respond(context => new StatusReport(context.Instance.CorrelationId, context.Instance.CurrentState.Name)));
            }

            public State Running { get; private set; }
            public Event<Start> Started { get; private set; }
            public Event<StatusRequested> Requested { get; private set; }
        }


        public class Start :
            CorrelatedBy<Guid>
        {
            public Start()
            {
                CorrelationId = NewId.NextGuid();
            }

            public Guid CorrelationId { get; private set; }
        }

        public class StatusRequested :
            CorrelatedBy<Guid>
        {
            public StatusRequested(Guid correlationId)
            {
                CorrelationId = correlationId;
            }

            public Guid CorrelationId { get; private set; }
        }


        class StatusReport :
            CorrelatedBy<Guid>
        {
            public StatusReport(Guid correlationId, string status)
            {
                CorrelationId = correlationId;
                Status = status;
            }

            public Guid CorrelationId { get; private set; }
            public string Status { get; private set; }
        }

        class StartupComplete
        {
        }
    }
}