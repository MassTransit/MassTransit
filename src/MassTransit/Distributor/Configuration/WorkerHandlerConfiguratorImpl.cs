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
namespace MassTransit.Distributor.Configuration
{
    using System;
    using System.Collections.Generic;
    using Builders;
    using Configurators;
    using Connectors;
    using MassTransit.Pipeline;
    using SubscriptionConfigurators;

    public class WorkerHandlerConfiguratorImpl<TMessage> :
        SubscriptionConfiguratorImpl<WorkerHandlerConfigurator<TMessage>>,
        WorkerHandlerConfigurator<TMessage>,
        WorkerBuilderConfigurator
        where TMessage : class
    {
        readonly HandlerSelector<TMessage> _handler;

        public WorkerHandlerConfiguratorImpl(Action<IConsumeContext<TMessage>, TMessage> handler)
        {
            _handler = x => context => handler(context, context.Message);
        }

        public WorkerHandlerConfiguratorImpl(Action<TMessage> handler)
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
            var configurator = new WorkerHandlerConnector<TMessage>(_handler, ReferenceFactory);

            builder.Add(configurator);
        }
    }
}