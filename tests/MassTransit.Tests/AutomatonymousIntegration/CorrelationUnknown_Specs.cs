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
    using Automatonymous;
    using MassTransit.Saga;
    using NUnit.Framework;
    using TestFramework;


    [TestFixture]
    public class When_message_correlation_is_not_configured :
        InMemoryTestFixture
    {
        [Test]
        public void Should_retry_the_status_message()
        {
            TestDelegate invocation = () => Bus.ConnectStateMachineSaga(_machine, _repository);

            Assert.Throws<ConfigurationException>(invocation);
        }

        protected override void ConfigureInMemoryReceiveEndpoint(IInMemoryReceiveEndpointConfigurator configurator)
        {
            _machine = new TestStateMachine();
            _repository = new InMemorySagaRepository<Instance>();
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
        }


        class TestStateMachine :
            MassTransitStateMachine<Instance>
        {
            public TestStateMachine()
            {
                InstanceState(x => x.CurrentState);

                Initially(
                    When(Firsted)
                        .Then(context => Console.WriteLine("Started: {0}", context.Instance.CorrelationId))
                        .TransitionTo(Running));

                During(Running,
                    When(Seconded)
                        .Then(context => Console.WriteLine("Status check!")));
            }

            public State Running { get; private set; }

            public Event<First> Firsted { get; private set; }
            public Event<Second> Seconded { get; private set; }
        }


        class Second
        {
            public string ServiceName { get; set; }
        }


        class First :
            CorrelatedBy<Guid>
        {
            public Guid CorrelationId { get; set; }
        }
    }
}
