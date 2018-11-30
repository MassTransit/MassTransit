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
    using System.Collections.ObjectModel;
    using System.Linq;
    using GreenPipes;
    using Observers;
    using Util;


    public class SendTopology :
        ISendTopologyConfigurator,
        ISendTopologyConfigurationObserver
    {
        readonly IList<IMessageSendTopologyConvention> _conventions;
        readonly object _lock = new object();
        readonly ConcurrentDictionary<Type, IMessageSendTopologyConfigurator> _messageTypes;
        readonly ConcurrentDictionary<Type, Uri> _endpointMapping;
        readonly SendTopologyConfigurationObservable _observers;

        public SendTopology()
        {
            _messageTypes = new ConcurrentDictionary<Type, IMessageSendTopologyConfigurator>();

            _observers = new SendTopologyConfigurationObservable();
            _endpointMapping = new ConcurrentDictionary<Type, Uri>();

            _conventions = new List<IMessageSendTopologyConvention>();
            _observers.Connect(this);
        }

        void ISendTopologyConfigurationObserver.MessageTopologyCreated<T>(IMessageSendTopologyConfigurator<T> messageTopology)
        {
            ApplyConventionsToMessageTopology(messageTopology);
        }

        public IMessageSendTopologyConfigurator<T> GetMessageTopology<T>()
            where T : class
        {
            if (TypeMetadataCache<T>.IsValidMessageType == false)
                throw new ArgumentException(TypeMetadataCache<T>.InvalidMessageTypeReason, nameof(T));

            var specification = _messageTypes.GetOrAdd(typeof(T), CreateMessageTopology<T>);

            return specification as IMessageSendTopologyConfigurator<T>;
        }

        public ConnectHandle Connect(ISendTopologyConfigurationObserver observer)
        {
            return _observers.Connect(observer);
        }

        public void AddConvention(ISendTopologyConvention convention)
        {
            lock (_lock)
            {
                _conventions.Add(convention);
            }
        }

        void ISendTopologyConfigurator.AddMessageSendTopology<T>(IMessageSendTopology<T> topology)
        {
            IMessageSendTopologyConfigurator<T> messageConfiguration = GetMessageTopology<T>();

            messageConfiguration.Add(topology);
        }

        public IReadOnlyDictionary<Type, Uri> EndpointMapping => new ReadOnlyDictionary<Type, Uri>(_endpointMapping);

        public void MapEndpoint(Type type, Uri uri)
        {
            if (!TypeMetadataCache.IsValidMessageType(type))
                throw new ArgumentException($"Message was not a valid type: {TypeMetadataCache.GetShortName(type)}", nameof(type));

            if (_endpointMapping.TryGetValue(type, out _))
                throw new InvalidOperationException("The endpoint convention has already been created and can no longer be modified.");

            _endpointMapping.TryAdd(type, uri);
        }

        public virtual IEnumerable<ValidationResult> Validate()
        {
            return _messageTypes.Values.SelectMany(x => x.Validate());
        }

        protected virtual IMessageSendTopologyConfigurator CreateMessageTopology<T>(Type type)
            where T : class
        {
            var messageTopology = new MessageSendTopology<T>();

            OnMessageTopologyCreated(messageTopology);
            return messageTopology;
        }

        protected void OnMessageTopologyCreated<T>(IMessageSendTopologyConfigurator<T> messageTopology)
            where T : class
        {
            _observers.MessageTopologyCreated(messageTopology);
        }

        void ApplyConventionsToMessageTopology<T>(IMessageSendTopologyConfigurator<T> messageTopology)
            where T : class
        {
            IMessageSendTopologyConvention[] conventions;
            lock (_lock)
            {
                conventions = _conventions.ToArray();
            }

            foreach (var convention in conventions)
            {
                if (convention.TryGetMessageSendTopologyConvention(out IMessageSendTopologyConvention<T> messageSendTopologyConvention))
                    messageTopology.AddConvention(messageSendTopologyConvention);
            }
        }
    }
}
