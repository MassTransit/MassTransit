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
    using System.Text;
    using GreenPipes;
    using Metadata;
    using Observers;
    using Util;


    public class ConsumeTopology :
        IConsumeTopologyConfigurator,
        IConsumeTopologyConfigurationObserver
    {
        readonly IList<IMessageConsumeTopologyConvention> _conventions;
        readonly object _lock = new object();
        readonly ConcurrentDictionary<Type, IMessageConsumeTopologyConfigurator> _messageTypes;
        readonly ConsumeTopologyConfigurationObservable _observers;

        protected ConsumeTopology()
        {
            _messageTypes = new ConcurrentDictionary<Type, IMessageConsumeTopologyConfigurator>();

            _observers = new ConsumeTopologyConfigurationObservable();

            _conventions = new List<IMessageConsumeTopologyConvention>();
            _observers.Connect(this);
        }

        void IConsumeTopologyConfigurationObserver.MessageTopologyCreated<T>(IMessageConsumeTopologyConfigurator<T> messageTopology)
        {
            ApplyConventionsToMessageTopology(messageTopology);
        }

        IMessageConsumeTopology<T> IConsumeTopology.GetMessageTopology<T>()
        {
            return GetMessageTopology<T>();
        }

        public virtual string CreateTemporaryQueueName(string tag)
        {
            if (string.IsNullOrWhiteSpace(tag))
                tag = "endpoint";

            var host = HostMetadataCache.Host;

            var sb = new StringBuilder(host.MachineName.Length + host.ProcessName.Length + tag.Length + 35);

            foreach (var c in host.MachineName)
                if (char.IsLetterOrDigit(c) || c == '_')
                    sb.Append(c);

            sb.Append('_');
            foreach (var c in host.ProcessName)
                if (char.IsLetterOrDigit(c) || c == '_')
                    sb.Append(c);

            sb.Append('_');
            sb.Append(tag);
            sb.Append('_');
            sb.Append(NewId.Next().ToString(FormatUtil.Formatter));

            return sb.ToString();
        }

        IMessageConsumeTopologyConfigurator<T> IConsumeTopologyConfigurator.GetMessageTopology<T>()
        {
            return GetMessageTopology<T>();
        }

        public ConnectHandle ConnectConsumeTopologyConfigurationObserver(IConsumeTopologyConfigurationObserver observer)
        {
            return _observers.Connect(observer);
        }

        public void AddConvention(IConsumeTopologyConvention convention)
        {
            lock (_lock)
            {
                _conventions.Add(convention);
            }
        }

        public virtual IEnumerable<ValidationResult> Validate()
        {
            return _messageTypes.Values.SelectMany(x => x.Validate());
        }

        void IConsumeTopologyConfigurator.AddMessageConsumeTopology<T>(IMessageConsumeTopology<T> topology)
        {
            IMessageConsumeTopologyConfigurator<T> messageConfiguration = GetMessageTopology<T>();

            messageConfiguration.Add(topology);
        }

        protected IMessageConsumeTopologyConfigurator<T> GetMessageTopology<T>()
            where T : class
        {
            if (TypeMetadataCache<T>.IsValidMessageType == false)
                throw new ArgumentException(TypeMetadataCache<T>.InvalidMessageTypeReason, nameof(T));

            var specification = _messageTypes.GetOrAdd(typeof(T), CreateMessageTopology<T>);

            return specification as IMessageConsumeTopologyConfigurator<T>;
        }

        protected bool All(Func<IMessageConsumeTopologyConfigurator, bool> callback)
        {
            IMessageConsumeTopologyConfigurator[] configurators;
            lock (_lock)
            {
                configurators = _messageTypes.Values.ToArray();
            }

            if (configurators.Length == 0)
                return true;

            if (configurators.Length == 1)
                return callback(configurators[0]);

            return configurators.All(callback);
        }

        protected IEnumerable<TResult> SelectMany<T, TResult>(Func<T, IEnumerable<TResult>> selector)
            where T : class
        {
            IMessageConsumeTopologyConfigurator[] configurators;
            lock (_lock)
            {
                configurators = _messageTypes.Values.ToArray();
            }

            if (configurators.Length == 0)
                return Enumerable.Empty<TResult>();

            if (configurators.Length == 1)
                return selector(configurators[0] as T);

            return configurators.Cast<T>().SelectMany(selector);
        }

        protected void ForEach<T>(Action<T> callback)
            where T : class
        {
            IMessageConsumeTopologyConfigurator[] configurators;
            lock (_lock)
            {
                configurators = _messageTypes.Values.ToArray();
            }

            switch (configurators.Length)
            {
                case 0:
                    break;
                case 1:
                    callback(configurators[0] as T);
                    break;
                default:
                    foreach (var configurator in configurators.Cast<T>())
                        callback(configurator);

                    break;
            }
        }

        protected virtual IMessageConsumeTopologyConfigurator CreateMessageTopology<T>(Type type)
            where T : class
        {
            var messageTopology = new MessageConsumeTopology<T>();

            OnMessageTopologyCreated(messageTopology);
            return messageTopology;
        }

        protected void OnMessageTopologyCreated<T>(IMessageConsumeTopologyConfigurator<T> messageTopology)
            where T : class
        {
            _observers.MessageTopologyCreated(messageTopology);
        }

        void ApplyConventionsToMessageTopology<T>(IMessageConsumeTopologyConfigurator<T> messageTopology)
            where T : class
        {
            IMessageConsumeTopologyConvention[] conventions;
            lock (_lock)
            {
                conventions = _conventions.ToArray();
            }

            foreach (var convention in conventions)
                if (convention.TryGetMessageConsumeTopologyConvention(out IMessageConsumeTopologyConvention<T> messageConsumeTopologyConvention))
                    messageTopology.AddConvention(messageConsumeTopologyConvention);
        }
    }
}
