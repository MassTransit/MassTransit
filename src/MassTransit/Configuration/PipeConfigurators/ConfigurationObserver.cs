// Copyright 2007-2018 Chris Patterson, Dru Sellers, Travis Smith, et. al.
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
namespace MassTransit.PipeConfigurators
{
    using System;
    using System.Collections.Generic;
    using ConsumeConfigurators;
    using GreenPipes.Util;
    using SagaConfigurators;


    /// <summary>
    /// Combines the separate configuration observers into a single observer that is for each message type, called once, to configure each
    /// message pipeline only once. Only outputs the individual message events for configuring the pipeline.
    /// </summary>
    public class ConfigurationObserver :
        Connectable<IMessageConfigurationObserver>,
        IConsumerConfigurationObserver,
        ISagaConfigurationObserver,
        IHandlerConfigurationObserver
    {
        readonly IConsumePipeConfigurator _configurator;
        readonly HashSet<Type> _messageTypes;

        public ConfigurationObserver(IConsumePipeConfigurator configurator)
        {
            _configurator = configurator;

            _messageTypes = new HashSet<Type>();

            configurator.ConnectConsumerConfigurationObserver(this);
            configurator.ConnectSagaConfigurationObserver(this);
            configurator.ConnectHandlerConfigurationObserver(this);
        }

        void IConsumerConfigurationObserver.ConsumerConfigured<TConsumer>(IConsumerConfigurator<TConsumer> configurator)
        {
        }

        void IConsumerConfigurationObserver.ConsumerMessageConfigured<TConsumer, TMessage>(IConsumerMessageConfigurator<TConsumer, TMessage> configurator)
        {
            NotifyObserver<TMessage>();
        }

        void IHandlerConfigurationObserver.HandlerConfigured<TMessage>(IHandlerConfigurator<TMessage> configurator)
        {
            NotifyObserver<TMessage>();
        }

        void ISagaConfigurationObserver.SagaConfigured<TSaga>(ISagaConfigurator<TSaga> configurator)
        {
        }

        void ISagaConfigurationObserver.SagaMessageConfigured<TSaga, TMessage>(ISagaMessageConfigurator<TSaga, TMessage> configurator)
        {
            NotifyObserver<TMessage>();
        }

        void NotifyObserver<TMessage>()
            where TMessage : class
        {
            if (_messageTypes.Contains(typeof(TMessage)))
                return;

            _messageTypes.Add(typeof(TMessage));

            All(observer =>
            {
                observer.MessageConfigured<TMessage>(_configurator);

                return true;
            });
        }
    }
}