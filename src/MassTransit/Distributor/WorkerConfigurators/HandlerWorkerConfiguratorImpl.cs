// Copyright 2007-2012 Chris Patterson, Dru Sellers, Travis Smith, et. al.
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
namespace MassTransit.Distributor.WorkerConfigurators
{
    using System;
    using System.Collections.Generic;
    using Builders;
    using Configuration;
    using Configurators;
    using MassTransit.Pipeline;
    using SubscriptionConfigurators;
    using WorkerConnectors;

    public class HandlerWorkerConfiguratorImpl<TMessage> :
        SubscriptionConfiguratorImpl<HandlerWorkerConfigurator<TMessage>>,
        HandlerWorkerConfigurator<TMessage>,
        WorkerBuilderConfigurator
        where TMessage : class
    {
        readonly HandlerSelector<TMessage> _handler;

        public HandlerWorkerConfiguratorImpl(Action<IConsumeContext<TMessage>, TMessage> handler)
        {
            _handler = x => context => handler(context, context.Message);
        }

        public HandlerWorkerConfiguratorImpl(Action<TMessage> handler)
        {
            _handler = HandlerSelector.ForHandler(handler);
        }

        public IEnumerable<ValidationResult> Validate()
        {
            if (_handler == null)
                yield return this.Failure("Handler", "must not be null");
        }

        public void Configure(WorkerBuilder builder)
        {
            var configurator = new HandlerWorkerConnector<TMessage>(_handler, ReferenceFactory);

            builder.Add(configurator);
        }
    }
}