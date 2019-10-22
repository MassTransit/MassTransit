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
namespace MassTransit.ConsumerSpecifications
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using ConsumeConfigurators;
    using GreenPipes;
    using Metadata;
    using Util;


    public class ConsumerSpecification<TConsumer> :
        IConsumerSpecification<TConsumer>
        where TConsumer : class
    {
        readonly ConnectHandle[] _handles;
        readonly IReadOnlyDictionary<Type, IConsumerMessageSpecification<TConsumer>> _messageTypes;
        readonly ConsumerConfigurationObservable _observers;

        public ConsumerSpecification(IEnumerable<IConsumerMessageSpecification<TConsumer>> messageSpecifications)
        {
            _messageTypes = messageSpecifications.ToDictionary(x => x.MessageType);

            _observers = new ConsumerConfigurationObservable();
            _handles = _messageTypes.Values.Select(x => x.ConnectConsumerConfigurationObserver(_observers)).ToArray();
        }

        public void Message<T>(Action<IConsumerMessageConfigurator<T>> configure)
            where T : class
        {
            if (configure == null)
                throw new ArgumentNullException(nameof(configure));

            IConsumerMessageSpecification<TConsumer, T> specification = GetMessageSpecification<T>();

            configure(specification);
        }

        public void ConsumerMessage<T>(Action<IConsumerMessageConfigurator<TConsumer, T>> configure)
            where T : class
        {
            if (configure == null)
                throw new ArgumentNullException(nameof(configure));

            IConsumerMessageSpecification<TConsumer, T> specification = GetMessageSpecification<T>();

            configure(specification);
        }

        public IConsumerMessageSpecification<TConsumer, T> GetMessageSpecification<T>() where T : class
        {
            if (!_messageTypes.TryGetValue(typeof(T), out IConsumerMessageSpecification<TConsumer> specification))
            {
                throw new ArgumentException($"MessageType {TypeMetadataCache<T>.ShortName} is not consumed by {TypeMetadataCache<TConsumer>.ShortName}");
            }

            return specification.GetMessageSpecification<T>();
        }

        public IEnumerable<ValidationResult> Validate()
        {
            _observers.All(observer =>
            {
                observer.ConsumerConfigured(this);
                return true;
            });

            return _messageTypes.Values.SelectMany(x => x.Validate());
        }

        public void AddPipeSpecification(IPipeSpecification<ConsumerConsumeContext<TConsumer>> specification)
        {
            foreach (IConsumerMessageSpecification<TConsumer> messageSpecification in _messageTypes.Values)
            {
                messageSpecification.AddPipeSpecification(specification);
            }
        }

        public ConnectHandle ConnectConsumerConfigurationObserver(IConsumerConfigurationObserver observer)
        {
            return _observers.Connect(observer);
        }
    }
}