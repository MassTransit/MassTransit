// Copyright 2007-2016 Chris Patterson, Dru Sellers, Travis Smith, et. al.
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
namespace MassTransit.BusConfigurators
{
    using System.Collections.Generic;
    using Builders;
    using GreenPipes;
    using Monitoring.Performance;


    public class PerformanceCounterBusFactorySpecification :
        IBusFactorySpecification
    {
        readonly ICounterFactory _factory;

        public PerformanceCounterBusFactorySpecification(ICounterFactory factory)
        {
            _factory = factory;
        }

        public IEnumerable<ValidationResult> Validate()
        {
            if (_factory == null)
                yield return this.Failure("factory", "ICounterFactory cannot be null");
        }

        public void Apply(IBusBuilder builder)
        {
            var observer = new PerformanceCounterBusObserver(_factory);

            builder.ConnectBusObserver(observer);
        }
    }
}