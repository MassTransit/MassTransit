// Copyright 2007-2014 Chris Patterson, Dru Sellers, Travis Smith, et. al.
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
namespace MassTransit.SubscriptionConnectors
{
    using System;


    public class MessageInterfaceType
    {
        readonly Lazy<MessageConnectorFactory> _messageConnectorFactory;
        readonly Lazy<SubscriptionConnectorFactory> _messageSubscriptionConnectorFactory;
        readonly Lazy<SubscriptionConnectorFactory> _subscriptionConnectorFactory;

        public MessageInterfaceType(Type interfaceType, Type messageType, Type consumerType)
        {
            InterfaceType = interfaceType;
            MessageType = messageType;

            _messageConnectorFactory = new Lazy<MessageConnectorFactory>(() => (MessageConnectorFactory)
                Activator.CreateInstance(typeof(MessageConnectorFactory<,>).MakeGenericType(consumerType,
                    messageType)));

            _subscriptionConnectorFactory = new Lazy<SubscriptionConnectorFactory>(() => (SubscriptionConnectorFactory)
                Activator.CreateInstance(typeof(ContextSubscriptionConnectorFactory<,>).MakeGenericType(consumerType,
                    messageType)));

            _messageSubscriptionConnectorFactory =
                new Lazy<SubscriptionConnectorFactory>(() => (SubscriptionConnectorFactory)
                    Activator.CreateInstance(typeof(MessageSubscriptionConnectorFactory<,>).MakeGenericType(
                        consumerType, messageType)));
        }

        public Type InterfaceType { get; private set; }
        public Type MessageType { get; private set; }

        public ConsumerMessageConnector GetConsumerConnector()
        {
            return _messageConnectorFactory.Value.CreateConsumerConnector();
        }

        public ConsumerMessageConnector GetConsumerContextConnector()
        {
            return _subscriptionConnectorFactory.Value.CreateSubscriptionConnector();
        }

        public InstanceMessageConnector GetInstanceContextConnector()
        {
            return _subscriptionConnectorFactory.Value.CreateInstanceConnector();
        }

        public ConsumerMessageConnector GetConsumerMessageConnector()
        {
            return _messageSubscriptionConnectorFactory.Value.CreateSubscriptionConnector();
        }

        public InstanceMessageConnector GetInstanceMessageConnector()
        {
            return _messageSubscriptionConnectorFactory.Value.CreateInstanceConnector();
        }

        public InstanceMessageConnector GetInstanceConnector()
        {
            return _messageConnectorFactory.Value.CreateInstanceConnector();
        }
    }
}