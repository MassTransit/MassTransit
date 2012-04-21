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
namespace MassTransit.Distributor.DistributorConfigurators
{
    using System;
    using System.Collections.Generic;
    using Configurators;
    using SubscriptionConfigurators;

    public abstract class DistributorConfiguratorImpl<T> :
        SubscriptionConfiguratorImpl<T>
        where T : class, SubscriptionConfigurator<T>
    {
        Func<IWorkerSelectorFactory> _workerSelectorFactory;

        protected DistributorConfiguratorImpl()
        {
            UseWorkerSelector<LeastBusyWorkerSelectorFactory>();
        }

        protected IWorkerSelectorFactory WorkerSelectorFactory
        {
            get { return _workerSelectorFactory(); }
        }

        public virtual IEnumerable<ValidationResult> Validate()
        {
            if (_workerSelectorFactory == null)
                yield return
                    new ValidationResultImpl(ValidationResultDisposition.Failure, "Selector", "must not be null");
        }

        public T UseWorkerSelector(
            Func<IWorkerSelectorFactory> workerSelectorFactory)
        {
            _workerSelectorFactory = () =>
                {
                    IWorkerSelectorFactory factory = workerSelectorFactory();

                    _workerSelectorFactory = () => factory;

                    return factory;
                };

            return this as T;
        }

        public T UseWorkerSelector<TSelector>()
            where TSelector : IWorkerSelectorFactory, new()
        {
            UseWorkerSelector(() => new TSelector());

            return this as T;
        }
    }
}