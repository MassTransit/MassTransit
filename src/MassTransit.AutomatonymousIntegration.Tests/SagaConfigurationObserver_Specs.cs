// Copyright 2007-2017 Chris Patterson, Dru Sellers, Travis Smith, et. al.
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
    using GreenPipes;
    using NUnit.Framework;
    using Saga;
    using Saga.SubscriptionConfigurators;
    using Util;


    public class SagaConfigurationObserver_Specs
    {
        readonly TestStateMachine _machine;
        readonly InMemorySagaRepository<Instance> _repository;

        public SagaConfigurationObserver_Specs()
        {
            _machine = new TestStateMachine();
            _repository = new InMemorySagaRepository<Instance>();
        }

        [Test]
        public void Should_invoke_the_observers_for_each_consumer_and_message_type()
        {
            var observer = new SagaConfigurationObserver();

            var bus = Bus.Factory.CreateUsingInMemory(cfg =>
            {
                cfg.ConnectSagaConfigurationObserver(observer);

                cfg.ReceiveEndpoint("hello", e =>
                {
                    e.UseRetry(x => x.Immediate(1));

                    e.StateMachineSaga(_machine, _repository, x =>
                    {
                        x.Message<Start>(m => m.UseConsoleLog(context => Task.FromResult("Hello")));
                        x.Message<Stop>(m => m.UseConsoleLog(context => Task.FromResult("Hello")));
                        x.SagaMessage<Start>(m => m.UseExecute(context =>
                        {
                        }));

                        x.UseExecuteAsync(context => TaskUtil.Completed);
                    });
                });
            });

            Assert.That(observer.SagaTypes.Contains(typeof(Instance)));
            Assert.That(observer.MessageTypes.Contains(Tuple.Create(typeof(Instance), typeof(Start))));
            Assert.That(observer.MessageTypes.Contains(Tuple.Create(typeof(Instance), typeof(Stop))));
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
        }


        class TestStateMachine :
            MassTransitStateMachine<Instance>
        {
            public TestStateMachine()
            {
                Initially(
                    When(Started)
                        .TransitionTo(Running));

                During(Running,
                    When(Stopped)
                        .Finalize());

                SetCompletedWhenFinalized();
            }

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


        class SagaConfigurationObserver :
            ISagaConfigurationObserver
        {
            readonly HashSet<Tuple<Type, Type>> _messageTypes;
            readonly HashSet<Type> _sagaTypes;

            public SagaConfigurationObserver()
            {
                _sagaTypes = new HashSet<Type>();
                _messageTypes = new HashSet<Tuple<Type, Type>>();
            }

            public HashSet<Type> SagaTypes => _sagaTypes;

            public HashSet<Tuple<Type, Type>> MessageTypes => _messageTypes;

            void ISagaConfigurationObserver.SagaConfigured<TSaga>(ISagaConfigurator<TSaga> configurator)
            {
                _sagaTypes.Add(typeof(TSaga));
            }

            void ISagaConfigurationObserver.SagaMessageConfigured<TSaga, TMessage>(ISagaMessageConfigurator<TSaga, TMessage> configurator)
            {
                _messageTypes.Add(Tuple.Create(typeof(TSaga), typeof(TMessage)));
            }
        }
    }
}