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
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;
    using Magnum.Reflection;
    using Saga;
    using Saga.SubscriptionConnectors;


    public class SagaInterfaceType
    {
        readonly Lazy<MessageConnectorFactory> _messageConnectorFactory;
        readonly Lazy<SubscriptionConnectorFactory> _messageSubscriptionConnectorFactory;
        readonly Lazy<SubscriptionConnectorFactory> _subscriptionConnectorFactory;

        public SagaInterfaceType(Type interfaceType, Type messageType, Type consumerType)
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


    public class SagaMetadataCache<TSaga> :
        ISagaMetadataCache<TSaga>
        where TSaga : class
    {
        readonly MessageInterfaceType[] _consumerTypes;
        readonly MessageInterfaceType[] _contextConsumerTypes;
        readonly MessageInterfaceType[] _messageConsumerTypes;

        SagaMetadataCache()
        {
            _consumerTypes = GetConsumerMessageTypes().ToArray();
            _messageConsumerTypes = GetMessageConsumerTypes().ToArray();
            _contextConsumerTypes = GetContextConsumerTypes().ToArray();
        }

        public static MessageInterfaceType[] ConsumerTypes
        {
            get { return InstanceCache.Cached.Value.ConsumerTypes; }
        }

        public static MessageInterfaceType[] MessageConsumerTypes
        {
            get { return InstanceCache.Cached.Value.MessageConsumerTypes; }
        }

        public static MessageInterfaceType[] ContextConsumerTypes
        {
            get { return InstanceCache.Cached.Value.ContextConsumerTypes; }
        }

        MessageInterfaceType[] ISagaMetadataCache<TSaga>.ConsumerTypes
        {
            get { return _consumerTypes; }
        }

        MessageInterfaceType[] ISagaMetadataCache<TSaga>.MessageConsumerTypes
        {
            get { return _messageConsumerTypes; }
        }

        MessageInterfaceType[] ISagaMetadataCache<TSaga>.ContextConsumerTypes
        {
            get { return _contextConsumerTypes; }
        }


        static IEnumerable<MessageInterfaceType> GetInitiatingTypes()
        {
            return typeof(TSaga).GetInterfaces()
    .Where(x => x.IsGenericType)
    .Where(x => x.GetGenericTypeDefinition() == typeof(InitiatedBy<>))
    .Select(x => new { InterfaceType = x, MessageType = x.GetGenericArguments()[0] })
    .Where(x => x.MessageType.IsValueType == false)
    .Select(x => FastActivator.Create(typeof(InitiatedBySagaSubscriptionConnector<,>),
        new[] { typeof(TSaga), x.MessageType }, _args))
    .Cast<SagaSubscriptionConnector>();

        }

        static IEnumerable<MessageInterfaceType> GetConsumerMessageTypes()
        {
            return typeof(TSaga).GetInterfaces()
                .Where(x => x.IsGenericType)
                .Where(x => x.GetGenericTypeDefinition() == typeof(IConsumer<>))
                .Select(x => new MessageInterfaceType(x, x.GetGenericArguments()[0], typeof(TSaga)))
                .Where(x => x.MessageType.IsValueType == false && x.MessageType != typeof(string));
        }

        static IEnumerable<MessageInterfaceType> GetMessageConsumerTypes()
        {
            return typeof(TSaga).GetInterfaces()
                .Where(x => x.IsGenericType)
                .Where(x => x.GetGenericTypeDefinition() == typeof(IMessageConsumer<>))
                .Select(x => new MessageInterfaceType(x, x.GetGenericArguments()[0], typeof(TSaga)))
                .Where(x => x.MessageType.IsValueType == false)
                .Where(IsNotContextType);
        }

        static IEnumerable<MessageInterfaceType> GetContextConsumerTypes()
        {
            return typeof(TSaga).GetInterfaces()
                .Where(x => x.IsGenericType)
                .Where(x => x.GetGenericTypeDefinition() == typeof(IMessageConsumer<>))
                .Select(x => new MessageInterfaceType(x, x.GetGenericArguments()[0], typeof(TSaga)))
                .Where(x => x.MessageType.IsValueType == false)
                .Where(type => !IsNotContextType(type));
        }

        static bool IsNotContextType(MessageInterfaceType x)
        {
            return !(x.MessageType.IsGenericType &&
                x.MessageType.GetGenericTypeDefinition() == typeof(IConsumeContext<>));
        }


        static class InstanceCache
        {
            internal static readonly Lazy<IConsumerMetadataCache<TSaga>> Cached = new Lazy<IConsumerMetadataCache<TSaga>>(
                () => new ConsumerMetadataCache<TSaga>(), LazyThreadSafetyMode.PublicationOnly);
        }
    }
}