namespace MassTransit.Tests.AutomatonymousIntegration
{
    using System;
    using System.Collections.Generic;
    using Automatonymous;
    using GreenPipes;
    using MassTransit.Saga;
    using NUnit.Framework;
    using SagaConfigurators;
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
                        x.Message<Start>(m => m.UseExecute(context => Console.WriteLine("Hello")));
                        x.Message<Stop>(m => m.UseExecute(context => Console.WriteLine("Hello")));
                        x.SagaMessage<Start>(m => m.UseExecute(context =>
                        {
                        }));

                        x.UseExecuteAsync(context => TaskUtil.Completed);
                    });
                });
            });

            Assert.That(observer.SagaTypes.Contains(typeof(Instance)));
            Assert.That(observer.StateMachineTypes.Contains(typeof(TestStateMachine)));
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
            readonly HashSet<Type> _stateMachineTypes;

            public SagaConfigurationObserver()
            {
                _sagaTypes = new HashSet<Type>();
                _stateMachineTypes = new HashSet<Type>();
                _messageTypes = new HashSet<Tuple<Type, Type>>();
            }

            public HashSet<Type> SagaTypes => _sagaTypes;

            public HashSet<Type> StateMachineTypes => _stateMachineTypes;

            public HashSet<Tuple<Type, Type>> MessageTypes => _messageTypes;

            void ISagaConfigurationObserver.SagaConfigured<TSaga>(ISagaConfigurator<TSaga> configurator)
            {
                _sagaTypes.Add(typeof(TSaga));
            }

            public void StateMachineSagaConfigured<TInstance>(ISagaConfigurator<TInstance> configurator, SagaStateMachine<TInstance> stateMachine)
                where TInstance : class, ISaga, SagaStateMachineInstance
            {
                _stateMachineTypes.Add(stateMachine.GetType());
            }

            void ISagaConfigurationObserver.SagaMessageConfigured<TSaga, TMessage>(ISagaMessageConfigurator<TSaga, TMessage> configurator)
            {
                _messageTypes.Add(Tuple.Create(typeof(TSaga), typeof(TMessage)));
            }
        }
    }
}
