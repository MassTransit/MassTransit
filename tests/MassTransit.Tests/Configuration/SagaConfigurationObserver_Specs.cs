namespace MassTransit.Tests.Configuration
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using MassTransit.Saga;
    using NUnit.Framework;
    using TestFramework.Messages;


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

                        x.UseExecuteAsync(context => Task.CompletedTask);
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

                return Task.CompletedTask;
            }

            public Guid CorrelationId { get; set; }

            public Task Consume(ConsumeContext<PongMessage> context)
            {
                return Task.CompletedTask;
            }
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
