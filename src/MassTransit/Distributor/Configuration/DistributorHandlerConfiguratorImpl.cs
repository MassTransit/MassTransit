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
    using SubscriptionConfigurators;

    public class DistributorHandlerConfiguratorImpl<TMessage> :
        SubscriptionConfiguratorImpl<DistributorHandlerConfigurator<TMessage>>,
        DistributorHandlerConfigurator<TMessage>,
        DistributorBuilderConfigurator
        where TMessage : class
    {
        Func<IWorkerSelectorFactory> _workerSelectorFactory;

        public DistributorHandlerConfiguratorImpl()
        {
            _workerSelectorFactory = () => new LeastBusyWorkerSelectorFactory();
        }

        public IEnumerable<ValidationResult> Validate()
        {
            if (_workerSelectorFactory == null)
                yield return this.Failure("Selector", "must not be null");
        }

        public void Configure(DistributorBuilder builder)
        {
            var workerSelector = _workerSelectorFactory().GetSelector<TMessage>();

            var configurator = new DistributorHandlerConnector<TMessage>(workerSelector, ReferenceFactory);

            builder.Add(configurator);
        }

        public DistributorHandlerConfigurator<TMessage> UseWorkerSelector(
            Func<IWorkerSelectorFactory> workerSelectorFactory)
        {
            _workerSelectorFactory = workerSelectorFactory;

            return this;
        }

        public DistributorHandlerConfigurator<TMessage> UseWorkerSelector<TSelector>()
            where TSelector : IWorkerSelectorFactory, new()
        {
            _workerSelectorFactory = () => new TSelector();

            return this;
        }
    }
}