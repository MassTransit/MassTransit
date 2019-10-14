namespace MassTransit.Tests.Configuration
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Automatonymous;
    using GreenPipes;
    using MassTransit.Saga;
    using NUnit.Framework;
    using SagaConfigurators;
    using TestFramework.Messages;
    using Util;


    public class SagaConfigurationObserver_Specs
    {
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

                    e.Saga(new InMemorySagaRepository<MySaga>(), x =>
                    {
                        x.Message<PingMessage>(m => m.UseExecute(context => Console.WriteLine("Hello")));
                        x.Message<PongMessage>(m => m.UseExecute(context => Console.WriteLine("Hello")));
                        x.SagaMessage<PingMessage>(m => m.UseExecute(context =>
                        {
                        }));

                        x.UseExecuteAsync(context => TaskUtil.Completed);
                    });
                });
            });

            Assert.That(observer.SagaTypes.Contains(typeof(MySaga)));
            Assert.That(observer.MessageTypes.Contains(Tuple.Create(typeof(MySaga), typeof(PingMessage))));
            Assert.That(observer.MessageTypes.Contains(Tuple.Create(typeof(MySaga), typeof(PongMessage))));
        }


        class MySaga :
            InitiatedBy<PingMessage>,
            Orchestrates<PongMessage>,
            ISaga
        {
            public Task Consume(ConsumeContext<PingMessage> context)
            {
                CorrelationId = context.Message.CorrelationId;

                return TaskUtil.Completed;
            }

            public Guid CorrelationId { get; set; }

            public Task Consume(ConsumeContext<PongMessage> context)
            {
                return TaskUtil.Completed;
            }
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
