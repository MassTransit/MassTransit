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
namespace MassTransit.SagaSpecifications
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using GreenPipes;
    using Metadata;
    using Saga;
    using SagaConfigurators;
    using Util;


    public class SagaSpecification<TSaga> :
        ISagaSpecification<TSaga>
        where TSaga : class, ISaga
    {
        readonly ConnectHandle[] _handles;
        readonly IReadOnlyDictionary<Type, ISagaMessageSpecification<TSaga>> _messageTypes;
        readonly SagaConfigurationObservable _observers;

        public SagaSpecification(IEnumerable<ISagaMessageSpecification<TSaga>> messageSpecifications)
        {
            _messageTypes = messageSpecifications.ToDictionary(x => x.MessageType);

            _observers = new SagaConfigurationObservable();
            _handles = _messageTypes.Values.Select(x => x.ConnectSagaConfigurationObserver(_observers)).ToArray();
        }

        void ISagaConfigurator<TSaga>.ConfigureMessage<T>(Action<ISagaMessageConfigurator<T>> configure)
        {
            Message(configure);
        }

        public void Message<T>(Action<ISagaMessageConfigurator<T>> configure)
            where T : class
        {
            if (configure == null)
                throw new ArgumentNullException(nameof(configure));

            ISagaMessageSpecification<TSaga, T> specification = GetMessageSpecification<T>();

            configure(specification);
        }

        public void SagaMessage<T>(Action<ISagaMessageConfigurator<TSaga, T>> configure)
            where T : class
        {
            if (configure == null)
                throw new ArgumentNullException(nameof(configure));

            ISagaMessageSpecification<TSaga, T> specification = GetMessageSpecification<T>();

            configure(specification);
        }

        public ISagaMessageSpecification<TSaga, T> GetMessageSpecification<T>() where T : class
        {
            ISagaMessageSpecification<TSaga> specification;
            if (!_messageTypes.TryGetValue(typeof(T), out specification))
            {
                throw new ArgumentException($"MessageType {TypeMetadataCache<T>.ShortName} is not consumed by {TypeMetadataCache<TSaga>.ShortName}");
            }

            return specification.GetMessageSpecification<T>();
        }

        public IEnumerable<ValidationResult> Validate()
        {
            _observers.All(observer =>
            {
                observer.SagaConfigured(this);
                return true;
            });

            return _messageTypes.Values.SelectMany(x => x.Validate());
        }

        public void AddPipeSpecification(IPipeSpecification<SagaConsumeContext<TSaga>> specification)
        {
            foreach (ISagaMessageSpecification<TSaga> messageSpecification in _messageTypes.Values)
            {
                messageSpecification.AddPipeSpecification(specification);
            }
        }

        public ConnectHandle ConnectSagaConfigurationObserver(ISagaConfigurationObserver observer)
        {
            return _observers.Connect(observer);
        }
    }
}