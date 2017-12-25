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
namespace MassTransit.Tests.Configuration
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using ConsumeConfigurators;
    using GreenPipes;
    using MassTransit.Configuration;
    using NUnit.Framework;
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
                        x.Message<PingMessage>(m => m.UseConsoleLog(context => Task.FromResult("Hello")));
                        x.Message<PongMessage>(m => m.UseConsoleLog(context => Task.FromResult("Hello")));
                        x.ConsumerMessage<PingMessage>(m => m.UseExecute(context => {}));

                        x.UseExecuteAsync(context => TaskUtil.Completed);
                    });
                });
            });

            Assert.That(observer.ConsumerTypes.Contains(typeof(MyConsumer)));
            Assert.That(observer.MessageTypes.Contains(Tuple.Create(typeof(MyConsumer), typeof(PingMessage))));
            Assert.That(observer.MessageTypes.Contains(Tuple.Create(typeof(MyConsumer), typeof(PongMessage))));
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