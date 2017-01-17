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
namespace MassTransit.Topology
{
    using System;
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.Linq;
    using Configuration;
    using GreenPipes;
    using Metadata;
    using Util;


    public class PublishTopology :
        IPublishTopologyConfigurator,
        IPublishTopologyConfigurationObserver
    {
        readonly IList<IMessagePublishTopologyConvention> _conventions;
        readonly IEntityNameFormatter _entityNameFormatter;
        readonly object _lock = new object();
        readonly ConcurrentDictionary<Type, IMessagePublishTopologyConfigurator> _messageSpecifications;
        readonly PublishTopologyConfigurationObservable _observers;

        public PublishTopology(IEntityNameFormatter entityNameFormatter)
        {
            if (entityNameFormatter == null)
                throw new ArgumentNullException(nameof(entityNameFormatter));

            _entityNameFormatter = entityNameFormatter;

            _messageSpecifications = new ConcurrentDictionary<Type, IMessagePublishTopologyConfigurator>();

            _observers = new PublishTopologyConfigurationObservable();

            _conventions = new List<IMessagePublishTopologyConvention>();
            _observers.Connect(this);
        }

        void IPublishTopologyConfigurationObserver.MessageTopologyCreated<T>(IMessagePublishTopologyConfigurator<T> configurator)
        {
            ApplyConventionsToMessageTopology(configurator);
        }

        public IEntityNameFormatter EntityNameFormatter => _entityNameFormatter;

        public IMessagePublishTopologyConfigurator<T> GetMessageTopology<T>()
            where T : class
        {
            if (TypeMetadataCache<T>.IsValidMessageType == false)
                throw new MessageException(typeof(T), $"The specified type is not a valid message type: {TypeMetadataCache<T>.ShortName}");

            var specification = _messageSpecifications.GetOrAdd(typeof(T), CreateMessageTopology<T>);

            return specification.GetMessageTopology<T>();
        }

        public ConnectHandle Connect(IPublishTopologyConfigurationObserver observer)
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

        protected virtual IMessagePublishTopologyConfigurator CreateMessageTopology<T>(Type type)
            where T : class
        {
            var messageTopology = new MessagePublishTopology<T>(new MessageEntityNameFormatter<T>(_entityNameFormatter));

            var connector = new ImplementedMessageTypeConnector<T>(this, messageTopology);

            ImplementedMessageTypeCache<T>.EnumerateImplementedTypes(connector);

            OnMessageTopologyCreated(messageTopology);

            return messageTopology;
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
                conventions = _conventions.ToArray();

            foreach (var convention in conventions)
            {
                IMessagePublishTopologyConvention<T> messagePublishTopologyConvention;
                if (convention.TryGetMessagePublishTopologyConvention(out messagePublishTopologyConvention))
                    messageTopology.AddConvention(messagePublishTopologyConvention);
            }
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
    }
}