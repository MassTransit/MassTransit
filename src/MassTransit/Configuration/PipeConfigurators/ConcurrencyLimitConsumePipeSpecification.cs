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
namespace MassTransit.PipeConfigurators
{
    using System.Collections.Generic;
    using GreenPipes;
    using Pipeline.Filters.ConcurrencyLimit;


    /// <summary>
    /// Adds a concurrency limit filter to the message pipe.
    /// </summary>
    /// <typeparam name="T">The message type</typeparam>
    public class ConcurrencyLimitConsumePipeSpecification<T> :
        IPipeSpecification<ConsumeContext<T>>
        where T : class
    {
        readonly IConcurrencyLimiter _limiter;

        public ConcurrencyLimitConsumePipeSpecification(IConcurrencyLimiter limiter)
        {
            _limiter = limiter;
        }

        public void Apply(IPipeBuilder<ConsumeContext<T>> builder)
        {
            var filter = new ConsumeConcurrencyLimitFilter<T>(_limiter);

            builder.AddFilter(filter);
        }

        public IEnumerable<ValidationResult> Validate()
        {
            if (_limiter.Limit < 1)
                yield return this.Failure("ConcurrencyLimit", "must be >= 1");
        }
    }
}
