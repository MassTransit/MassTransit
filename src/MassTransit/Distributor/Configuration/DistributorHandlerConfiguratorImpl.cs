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

    public class DistributorHandlerConfiguratorImpl<T> :
        DistributorHandlerConfigurator<T>,
        DistributorBuilderConfigurator
        where T : class
    {
        Func<IWorkerSelectionStrategy<T>> _selector;

        public DistributorHandlerConfiguratorImpl()
        {
            _selector = () => new DefaultWorkerSelectionStrategy<T>();
        }

        public DistributorHandlerConfigurator<T> UseWorkerSelector(Func<IWorkerSelectionStrategy<T>> selector)
        {
            _selector = selector;

            return this;
        }

        public DistributorHandlerConfigurator<T> UseWorkerSelector<TSelector>()
            where TSelector : IWorkerSelectionStrategy<T>, new()
        {
            _selector = () => new TSelector();

            return this;
        }
    }
}