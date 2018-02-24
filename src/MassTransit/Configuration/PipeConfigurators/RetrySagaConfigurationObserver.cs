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
namespace MassTransit.PipeConfigurators
{
    using System;
    using System.Collections.Generic;
    using Context;
    using GreenPipes.Configurators;
    using SagaConfigurators;


    public class RetrySagaConfigurationObserver :
        ISagaConfigurationObserver
    {
        readonly IReceiveEndpointConfigurator _configurator;
        readonly Action<IRetryConfigurator> _configure;
        readonly HashSet<Tuple<Type, Type>> _messageTypes;
        readonly HashSet<Type> _sagaTypes;

        public RetrySagaConfigurationObserver(IReceiveEndpointConfigurator configurator, Action<IRetryConfigurator> configure)
        {
            _configurator = configurator;
            _configure = configure;
            _sagaTypes = new HashSet<Type>();
            _messageTypes = new HashSet<Tuple<Type, Type>>();
        }

        void ISagaConfigurationObserver.SagaConfigured<TSaga>(ISagaConfigurator<TSaga> configurator)
        {
            _sagaTypes.Add(typeof(TSaga));
        }

        void ISagaConfigurationObserver.SagaMessageConfigured<TSaga, TMessage>(ISagaMessageConfigurator<TSaga, TMessage> configurator)
        {
            Tuple<Type, Type> key = Tuple.Create(typeof(TSaga), typeof(TMessage));
            if (_messageTypes.Contains(key))
                return;

            _messageTypes.Add(key);

            var specification = new ConsumeContextRetryPipeSpecification<ConsumeContext<TMessage>,
                RetryConsumeContext<TMessage>>(x => new RetryConsumeContext<TMessage>(x));

            _configure?.Invoke(specification);

            _configurator.AddPipeSpecification(specification);
        }
    }
}