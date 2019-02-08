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
namespace MassTransit.Topology.Topologies
{
    using System;
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.Linq;
    using GreenPipes;
    using Metadata;
    using Observers;
    using Util;


    public class PublishTopology :
        IPublishTopologyConfigurator,
        IPublishTopologyConfigurationObserver
    {
        readonly IList<IMessagePublishTopologyConvention> _conventions;
        readonly object _lock = new object();
        readonly ConcurrentDictionary<Type, IMessageTypeFactory> _messageTypeFactoryCache;
        readonly ConcurrentDictionary<Type, IMessagePublishTopologyConfigurator> _messageTypes;
        readonly PublishTopologyConfigurationObservable _observers;

        public PublishTopology()
        {
            _messageTypes = new ConcurrentDictionary<Type, IMessagePublishTopologyConfigurator>();

            _observers = new PublishTopologyConfigurationObservable();
            _messageTypeFactoryCache = new ConcurrentDictionary<Type, IMessageTypeFactory>();

            _conventions = new List<IMessagePublishTopologyConvention>();

            _observers.Connect(this);
        }

        void IPublishTopologyConfigurationObserver.MessageTopologyCreated<T>(IMessagePublishTopologyConfigurator<T> configurator)
        {
            ApplyConventionsToMessageTopology(configurator);
        }

        IMessagePublishTopology<T> IPublishTopology.GetMessageTopology<T>()
        {
            return GetMessageTopology<T>();
        }

        IMessagePublishTopologyConfigurator<T> IPublishTopologyConfigurator.GetMessageTopology<T>()
        {
            return GetMessageTopology<T>();
        }

        public bool TryGetPublishAddress(Type messageType, Uri baseAddress, out Uri publishAddress)
        {
            return _messageTypes.GetOrAdd(messageType, CreateMessageType)
                .TryGetPublishAddress(baseAddress, out publishAddress);
        }

        public ConnectHandle ConnectPublishTopologyConfigurationObserver(IPublishTopologyConfigurationObserver observer)
        {
            return _observers.Connect(observer);
        }

        public void AddConvention(IPublishTopologyConvention convention)
        {
            lock (_lock)
            {
                _conventions.Add(convention);
            }
        }

        void IPublishTopologyConfigurator.AddMessagePublishTopology<T>(IMessagePublishTopology<T> topology)
        {
            IMessagePublishTopologyConfigurator<T> messageConfiguration = GetMessageTopology<T>();

            messageConfiguration.Add(topology);
        }

        public virtual IEnumerable<ValidationResult> Validate()
        {
            return _messageTypes.Values.SelectMany(x => x.Validate());
        }

        protected virtual IMessagePublishTopologyConfigurator CreateMessageTopology<T>(Type type)
            where T : class
        {
            var messageTopology = new MessagePublishTopology<T>();

            var connector = new ImplementedMessageTypeConnector<T>(this, messageTopology);

            ImplementedMessageTypeCache<T>.EnumerateImplementedTypes(connector);

            OnMessageTopologyCreated(messageTopology);

            return messageTopology;
        }

        protected IMessagePublishTopologyConfigurator<T> GetMessageTopology<T>()
            where T : class
        {
            if (TypeMetadataCache<T>.IsValidMessageType == false)
                throw new ArgumentException(TypeMetadataCache<T>.InvalidMessageTypeReason, nameof(T));

            var topology = _messageTypes.GetOrAdd(typeof(T), CreateMessageTopology<T>);

            return topology as IMessagePublishTopologyConfigurator<T>;
        }

        protected void OnMessageTopologyCreated<T>(IMessagePublishTopologyConfigurator<T> messageTopology)
            where T : class
        {
            _observers.MessageTopologyCreated(messageTopology);
        }

        void ApplyConventionsToMessageTopology<T>(IMessagePublishTopologyConfigurator<T> messageTopology)
            where T : class
        {
            IMessagePublishTopologyConvention[] conventions;
            lock (_lock)
            {
                conventions = _conventions.ToArray();
            }

            foreach (var convention in conventions)
                if (convention.TryGetMessagePublishTopologyConvention(out IMessagePublishTopologyConvention<T> messagePublishTopologyConvention))
                    messageTopology.AddConvention(messagePublishTopologyConvention);
        }

        IMessagePublishTopologyConfigurator CreateMessageType(Type messageType)
        {
            return GetOrAddByMessageType(messageType).CreateMessageType();
        }

        IMessageTypeFactory GetOrAddByMessageType(Type type)
        {
            return _messageTypeFactoryCache.GetOrAdd(type,
                _ => (IMessageTypeFactory)Activator.CreateInstance(typeof(MessageTypeFactory<>).MakeGenericType(type), this));
        }


        class ImplementedMessageTypeConnector<TMessage> :
            IImplementedMessageType
            where TMessage : class
        {
            readonly MessagePublishTopology<TMessage> _messagePublishTopologyConfigurator;
            readonly IPublishTopologyConfigurator _publishTopology;

            public ImplementedMessageTypeConnector(IPublishTopologyConfigurator publishTopology,
                MessagePublishTopology<TMessage> messagePublishTopologyConfigurator)
            {
                _publishTopology = publishTopology;
                _messagePublishTopologyConfigurator = messagePublishTopologyConfigurator;
            }

            public void ImplementsMessageType<T>(bool direct)
                where T : class
            {
                IMessagePublishTopologyConfigurator<T> messageTopology = _publishTopology.GetMessageTopology<T>();

                _messagePublishTopologyConfigurator.AddImplementedMessageConfigurator(messageTopology);
            }
        }


        interface IMessageTypeFactory
        {
            IMessagePublishTopologyConfigurator CreateMessageType();
        }


        class MessageTypeFactory<T> :
            IMessageTypeFactory
            where T : class
        {
            readonly IPublishTopologyConfigurator _configurator;

            public MessageTypeFactory(IPublishTopologyConfigurator configurator)
            {
                _configurator = configurator;
            }

            public IMessagePublishTopologyConfigurator CreateMessageType()
            {
                return _configurator.GetMessageTopology<T>();
            }
        }
    }
}