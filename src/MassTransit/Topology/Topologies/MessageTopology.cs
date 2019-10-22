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
namespace MassTransit.Topology.Topologies
{
    using System;
    using System.Collections.Concurrent;
    using System.Linq.Expressions;
    using System.Reflection;
    using EntityNameFormatters;
    using GreenPipes;
    using GreenPipes.Internals.Extensions;
    using Metadata;
    using Observers;
    using Util;


    public class MessageTopology :
        IMessageTopologyConfigurator
    {
        readonly ConcurrentDictionary<Type, IMessageTypeTopologyConfigurator> _messageTypes;
        readonly MessageTopologyConfigurationObservable _observers;

        public MessageTopology(IEntityNameFormatter entityNameFormatter)
        {
            EntityNameFormatter = entityNameFormatter ?? throw new ArgumentNullException(nameof(entityNameFormatter));

            _messageTypes = new ConcurrentDictionary<Type, IMessageTypeTopologyConfigurator>();
            _observers = new MessageTopologyConfigurationObservable();
        }

        public IEntityNameFormatter EntityNameFormatter { get; private set; }

        public void SetEntityNameFormatter(IEntityNameFormatter entityNameFormatter)
        {
            EntityNameFormatter = entityNameFormatter ?? throw new ArgumentNullException("The entity name formatter cannot be null");
        }

        IMessageTopologyConfigurator<T> IMessageTopologyConfigurator.GetMessageTopology<T>()
        {
            return GetMessageTopology<T>();
        }

        public ConnectHandle ConnectMessageTopologyConfigurationObserver(IMessageTopologyConfigurationObserver observer)
        {
            return _observers.Connect(observer);
        }

        IMessageTopology<T> IMessageTopology.GetMessageTopology<T>()
        {
            return GetMessageTopology<T>();
        }

        IMessageTopologyConfigurator<T> GetMessageTopology<T>()
            where T : class
        {
            if (TypeMetadataCache<T>.IsValidMessageType == false)
                throw new ArgumentException(TypeMetadataCache<T>.InvalidMessageTypeReason, nameof(T));

            var specification = _messageTypes.GetOrAdd(typeof(T), CreateMessageTopology<T>);

            return specification as IMessageTopologyConfigurator<T>;
        }

        protected virtual IMessageTypeTopologyConfigurator CreateMessageTopology<T>(Type type)
            where T : class
        {
            var messageTopology = new MessageTopology<T>(new MessageEntityNameFormatter<T>(EntityNameFormatter), _observers);

            OnMessageTopologyCreated(messageTopology);

            return messageTopology;
        }

        void OnMessageTopologyCreated<T>(IMessageTopologyConfigurator<T> messageTopology)
            where T : class
        {
            _observers.MessageTopologyCreated(messageTopology);
        }
    }


    public class MessageTopology<TMessage> :
        IMessageTopologyConfigurator<TMessage>
        where TMessage : class
    {
        readonly MessageTopologyConfigurationObservable _observers;
        readonly Lazy<string> _entityName;
        readonly ConcurrentDictionary<string, IMessagePropertyTopologyConfigurator<TMessage>> _properties;

        public MessageTopology(IMessageEntityNameFormatter<TMessage> entityNameFormatter, MessageTopologyConfigurationObservable observers)
        {
            _observers = observers;
            EntityNameFormatter = entityNameFormatter;

            _properties = new ConcurrentDictionary<string, IMessagePropertyTopologyConfigurator<TMessage>>(StringComparer.OrdinalIgnoreCase);

            _entityName = new Lazy<string>(() => EntityNameFormatter.FormatEntityName());
        }

        public IMessageEntityNameFormatter<TMessage> EntityNameFormatter { get; private set; }

        public string EntityName => _entityName.Value;

        public void SetEntityNameFormatter(IMessageEntityNameFormatter<TMessage> entityNameFormatter)
        {
            if (_entityName.IsValueCreated)
                throw new ConfigurationException(
                    $"The message type {TypeMetadataCache<TMessage>.ShortName} entity name was already evaluated: {_entityName.Value}");

            EntityNameFormatter = entityNameFormatter ?? throw new ArgumentNullException(nameof(entityNameFormatter));
        }

        public void SetEntityName(string entityName)
        {
            SetEntityNameFormatter(new StaticEntityNameFormatter<TMessage>(entityName));
        }

        public void CorrelateBy(Expression<Func<TMessage, Guid>> propertyExpression)
        {
            var propertyTopology = GetPropertyTopology<Guid>(propertyExpression.GetPropertyInfo());

            propertyTopology.IsCorrelationId = true;
        }

        public IMessagePropertyTopologyConfigurator<TMessage, T> GetProperty<T>(Expression<Func<TMessage, T>> propertyExpression)
        {
            return GetPropertyTopology<T>(propertyExpression.GetPropertyInfo());
        }

        IMessagePropertyTopology<TMessage, T> IMessageTopology<TMessage>.GetProperty<T>(PropertyInfo propertyInfo)
        {
            return GetPropertyTopology<T>(propertyInfo);
        }

        IMessagePropertyTopologyConfigurator<TMessage, T> GetPropertyTopology<T>(PropertyInfo propertyInfo)
        {
            var specification = _properties.GetOrAdd(propertyInfo.Name, _ => CreatePropertyTopology<T>(propertyInfo));

            return specification as IMessagePropertyTopologyConfigurator<TMessage, T>;
        }

        protected virtual IMessagePropertyTopologyConfigurator<TMessage> CreatePropertyTopology<T>(PropertyInfo propertyInfo)
        {
            var messageTopology = new MessagePropertyTopology<TMessage, T>(propertyInfo);

            OnMessagePropertyTopologyCreated(messageTopology);

            return messageTopology;
        }

        void OnMessagePropertyTopologyCreated<T>(IMessagePropertyTopologyConfigurator<TMessage, T> propertyTopology)
        {
            _observers.MessagePropertyTopologyCreated(propertyTopology);
        }
    }
}
