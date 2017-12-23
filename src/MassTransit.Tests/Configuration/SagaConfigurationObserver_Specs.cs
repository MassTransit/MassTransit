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
    using GreenPipes;
    using MassTransit.Saga;
    using MassTransit.Saga.SubscriptionConfigurators;
    using NUnit.Framework;
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
                        x.Message<PingMessage>(m => m.UseConsoleLog(context => Task.FromResult("Hello")));
                        x.Message<PongMessage>(m => m.UseConsoleLog(context => Task.FromResult("Hello")));
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