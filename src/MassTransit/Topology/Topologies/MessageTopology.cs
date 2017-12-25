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
    using Configuration;
    using EntityNameFormatters;
    using GreenPipes;
    using Observers;
    using Util;


    public class MessageTopology :
        IMessageTopologyConfigurator
    {
        readonly ConcurrentDictionary<Type, IMessageTypeTopologyConfigurator> _messageTypes;
        readonly MessageTopologyConfigurationObservable _observers;

        public MessageTopology(IEntityNameFormatter entityNameFormatter)
        {
            if (entityNameFormatter == null)
                throw new ArgumentNullException(nameof(entityNameFormatter));

            EntityNameFormatter = entityNameFormatter;

            _messageTypes = new ConcurrentDictionary<Type, IMessageTypeTopologyConfigurator>();
            _observers = new MessageTopologyConfigurationObservable();
        }

        public IEntityNameFormatter EntityNameFormatter { get; }

        IMessageTopologyConfigurator<T> IMessageTopologyConfigurator.GetMessageTopology<T>()
        {
            return GetMessageTopology<T>();
        }

        public ConnectHandle Connect(IMessageTopologyConfigurationObserver observer)
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
            var messageTopology = new MessageTopology<T>(new MessageEntityNameFormatter<T>(EntityNameFormatter));

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
        readonly Lazy<string> _entityName;

        public MessageTopology(IMessageEntityNameFormatter<TMessage> entityNameFormatter)
        {
            EntityNameFormatter = entityNameFormatter;

            _entityName = new Lazy<string>(EntityNameFormatter.FormatEntityName);
        }

        public IMessageEntityNameFormatter<TMessage> EntityNameFormatter { get; private set; }

        public string EntityName => _entityName.Value;

        public void SetEntityNameFormatter(IMessageEntityNameFormatter<TMessage> entityNameFormatter)
        {
            if (entityNameFormatter == null)
                throw new ArgumentNullException(nameof(entityNameFormatter));

            EntityNameFormatter = entityNameFormatter;
        }

        public void SetEntityName(string entityName)
        {
            EntityNameFormatter = new StaticEntityNameFormatter<TMessage>(entityName);
        }
    }
}