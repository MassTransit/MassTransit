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
namespace MassTransit
{
    using System;
    using System.Collections.Generic;
    using Configurators;
    using Distributor;
    using Distributor.Builders;
    using Distributor.Configuration;
    using Saga;

    public class DistributorSagaConfiguratorImpl<TSaga> :
        DistributorSagaConfigurator<TSaga>,
        DistributorBuilderConfigurator
        where TSaga : class, ISaga
    {
        public DistributorSagaConfigurator<TSaga> UseWorkerSelector(Func<IWorkerSelector<TSaga>> selector)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<ValidationResult> Validate()
        {
            throw new NotImplementedException();
        }

        public void Configure(DistributorBuilder builder)
        {
            throw new NotImplementedException();
        }
    }
}