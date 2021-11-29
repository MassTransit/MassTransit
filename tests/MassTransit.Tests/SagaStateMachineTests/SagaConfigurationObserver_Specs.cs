namespace MassTransit.Tests.SagaStateMachineTests
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using MassTransit.Saga;
    using NUnit.Framework;


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

                        x.UseExecuteAsync(context => Task.CompletedTask);
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
            public SagaConfigurationObserver()
            {
                SagaTypes = new HashSet<Type>();
                StateMachineTypes = new HashSet<Type>();
                MessageTypes = new HashSet<Tuple<Type, Type>>();
            }

            public HashSet<Type> SagaTypes { get; }

            public HashSet<Type> StateMachineTypes { get; }

            public HashSet<Tuple<Type, Type>> MessageTypes { get; }

            void ISagaConfigurationObserver.SagaConfigured<TSaga>(ISagaConfigurator<TSaga> configurator)
            {
                SagaTypes.Add(typeof(TSaga));
            }

            public void StateMachineSagaConfigured<TInstance>(ISagaConfigurator<TInstance> configurator, SagaStateMachine<TInstance> stateMachine)
                where TInstance : class, ISaga, SagaStateMachineInstance
            {
                StateMachineTypes.Add(stateMachine.GetType());
            }

            void ISagaConfigurationObserver.SagaMessageConfigured<TSaga, TMessage>(ISagaMessageConfigurator<TSaga, TMessage> configurator)
            {
                MessageTypes.Add(Tuple.Create(typeof(TSaga), typeof(TMessage)));
            }
        }
    }
}
