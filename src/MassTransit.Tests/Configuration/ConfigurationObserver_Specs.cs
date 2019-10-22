namespace MassTransit.Tests.Configuration
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using ConsumeConfigurators;
    using GreenPipes;
    using MassTransit.Courier;
    using NUnit.Framework;
    using TestFramework.Courier;
    using TestFramework.Messages;
    using Util;


    public class ConfigurationObserver_Specs
    {
        [Test]
        public void Should_invoke_the_observers_for_each_consumer_and_message_type()
        {
            var observer = new ConsumerConfigurationObserver();

            var bus = Bus.Factory.CreateUsingInMemory(cfg =>
            {
                cfg.ConnectConsumerConfigurationObserver(observer);

                cfg.ReceiveEndpoint("hello", e =>
                {
                    e.UseRetry(x => x.Immediate(1));

                    e.Consumer<MyConsumer>(x =>
                    {
                        x.Message<PingMessage>(m => m.UseExecute(context => Console.WriteLine("Hello")));
                        x.Message<PongMessage>(m => m.UseExecute(context => Console.WriteLine("Hello")));
                        x.ConsumerMessage<PingMessage>(m => m.UseExecute(context =>
                        {
                        }));

                        x.UseExecuteAsync(context => TaskUtil.Completed);
                    });
                });
            });

            Assert.That(observer.ConsumerTypes.Contains(typeof(MyConsumer)));
            Assert.That(observer.MessageTypes.Contains(Tuple.Create(typeof(MyConsumer), typeof(PingMessage))));
            Assert.That(observer.MessageTypes.Contains(Tuple.Create(typeof(MyConsumer), typeof(PongMessage))));
        }

        [Test]
        public void Should_invoke_the_observers_for_object_consumer_and_message_type()
        {
            var observer = new ConsumerConfigurationObserver();

            var bus = Bus.Factory.CreateUsingInMemory(cfg =>
            {
                cfg.ConnectConsumerConfigurationObserver(observer);

                cfg.ReceiveEndpoint("hello", e =>
                {
                    e.UseRetry(x => x.Immediate(1));

                    e.Consumer(typeof(MyConsumer), _ => new MyConsumer());
                });
            });

            Assert.That(observer.ConsumerTypes.Contains(typeof(MyConsumer)));
            Assert.That(observer.MessageTypes.Contains(Tuple.Create(typeof(MyConsumer), typeof(PingMessage))));
            Assert.That(observer.MessageTypes.Contains(Tuple.Create(typeof(MyConsumer), typeof(PongMessage))));
        }

        [Test]
        public void Should_invoke_for_the_handler()
        {
            var observer = new HandlerConfigurationObserver();

            var bus = Bus.Factory.CreateUsingInMemory(cfg =>
            {
                cfg.ConnectHandlerConfigurationObserver(observer);

                cfg.ReceiveEndpoint("hello", e =>
                {
                    e.UseRetry(x => x.Immediate(1));

                    e.Handler<PingMessage>(async context =>
                    {
                    });
                });
            });

            Assert.That(observer.MessageTypes.Contains(typeof(PingMessage)));
        }

        [Test]
        public void Should_invoke_the_observers_for_regular_consumer_and_message_type()
        {
            var observer = new ConsumerConfigurationObserver();

            var bus = Bus.Factory.CreateUsingInMemory(cfg =>
            {
                cfg.ConnectConsumerConfigurationObserver(observer);

                cfg.ReceiveEndpoint("hello", e =>
                {
                    e.UseRetry(x => x.Immediate(1));

                    e.Consumer<MyConsumer>();
                });
            });

            Assert.That(observer.ConsumerTypes.Contains(typeof(MyConsumer)));
            Assert.That(observer.MessageTypes.Contains(Tuple.Create(typeof(MyConsumer), typeof(PingMessage))));
            Assert.That(observer.MessageTypes.Contains(Tuple.Create(typeof(MyConsumer), typeof(PongMessage))));
        }

        [Test]
        public void Should_invoke_the_observers_for_execute_activity_type()
        {
            var observer = new ActivityConfigurationObserver();

            var bus = Bus.Factory.CreateUsingInMemory(cfg =>
            {
                cfg.ConnectActivityConfigurationObserver(observer);

                cfg.ReceiveEndpoint("hello", e =>
                {
                    e.ExecuteActivityHost<SetVariableActivity, SetVariableArguments>();
                });
            });

            Assert.That(observer.ExecuteActivityTypes.Contains((typeof(SetVariableActivity), typeof(SetVariableArguments))));
        }

        [Test]
        public void Should_invoke_the_observers_for_activity_type()
        {
            var observer = new ActivityConfigurationObserver();

            Uri compensateAddress = null;

            var bus = Bus.Factory.CreateUsingInMemory(cfg =>
            {
                cfg.ConnectActivityConfigurationObserver(observer);

                cfg.ReceiveEndpoint("hello", e =>
                {
                    cfg.ReceiveEndpoint("goodbye", ce =>
                    {
                        ce.CompensateActivityHost<TestActivity, TestLog>();

                        e.ExecuteActivityHost<TestActivity, TestArguments>(ce.InputAddress);

                        compensateAddress = ce.InputAddress;
                    });

                });
            });

            Assert.That(observer.ActivityTypes.Contains((typeof(TestActivity), typeof(TestArguments), compensateAddress)));
            Assert.That(observer.CompensateActivityTypes.Contains((typeof(TestActivity), typeof(TestLog))));
        }


        class MyConsumer :
            IConsumer<PingMessage>,
            IConsumer<PongMessage>
        {
            public Task Consume(ConsumeContext<PingMessage> context)
            {
                return TaskUtil.Completed;
            }

            public Task Consume(ConsumeContext<PongMessage> context)
            {
                return TaskUtil.Completed;
            }
        }


        class HandlerConfigurationObserver :
            IHandlerConfigurationObserver
        {
            readonly HashSet<Type> _messageTypes;

            public HashSet<Type> MessageTypes => _messageTypes;

            public HandlerConfigurationObserver()
            {
                _messageTypes = new HashSet<Type>();
            }

            public void HandlerConfigured<TMessage>(IHandlerConfigurator<TMessage> configurator)
                where TMessage : class
            {
                _messageTypes.Add(typeof(TMessage));
            }
        }


        class ActivityConfigurationObserver :
            IActivityConfigurationObserver
        {
            readonly HashSet<(Type activityType, Type argumentType)> _executeActivityTypes;
            readonly HashSet<(Type activityType, Type argumentType, Uri compensateAddress)> _activityTypes;
            readonly HashSet<(Type activityType, Type logType)> _compensateActivityTypes;

            public HashSet<(Type activityType, Type argumentType)> ExecuteActivityTypes => _executeActivityTypes;
            public HashSet<(Type activityType, Type argumentType, Uri compensateAddress)> ActivityTypes => _activityTypes;
            public HashSet<(Type activityType, Type logType)> CompensateActivityTypes => _compensateActivityTypes;

            public ActivityConfigurationObserver()
            {
                _executeActivityTypes = new HashSet<(Type activityType, Type argumentdType)>();
                _activityTypes = new HashSet<(Type activityType, Type argumentType, Uri compensateAddress)>();
                _compensateActivityTypes = new HashSet<(Type activityType, Type logType)>();
            }

            public void ActivityConfigured<TActivity, TArguments>(IExecuteActivityConfigurator<TActivity, TArguments> configurator, Uri compensateAddress)
                where TActivity : class, IExecuteActivity<TArguments>
                where TArguments : class
            {
                _activityTypes.Add((typeof(TActivity), typeof(TArguments), compensateAddress));
            }

            public void ExecuteActivityConfigured<TActivity, TArguments>(IExecuteActivityConfigurator<TActivity, TArguments> configurator)
                where TActivity : class, IExecuteActivity<TArguments>
                where TArguments : class
            {
                _executeActivityTypes.Add((typeof(TActivity), typeof(TArguments)));
            }

            public void CompensateActivityConfigured<TActivity, TLog>(ICompensateActivityConfigurator<TActivity, TLog> configurator)
                where TActivity : class, ICompensateActivity<TLog>
                where TLog : class
            {
                _compensateActivityTypes.Add((typeof(TActivity), typeof(TLog)));
            }
        }


        class ConsumerConfigurationObserver :
            IConsumerConfigurationObserver
        {
            readonly HashSet<Type> _consumerTypes;
            readonly HashSet<Tuple<Type, Type>> _messageTypes;

            public ConsumerConfigurationObserver()
            {
                _consumerTypes = new HashSet<Type>();
                _messageTypes = new HashSet<Tuple<Type, Type>>();
            }

            public HashSet<Type> ConsumerTypes => _consumerTypes;

            public HashSet<Tuple<Type, Type>> MessageTypes => _messageTypes;

            void IConsumerConfigurationObserver.ConsumerConfigured<TConsumer>(IConsumerConfigurator<TConsumer> configurator)
            {
                _consumerTypes.Add(typeof(TConsumer));
            }

            void IConsumerConfigurationObserver.ConsumerMessageConfigured<TConsumer, TMessage>(IConsumerMessageConfigurator<TConsumer, TMessage> configurator)
            {
                _messageTypes.Add(Tuple.Create(typeof(TConsumer), typeof(TMessage)));
            }
        }
    }
}
