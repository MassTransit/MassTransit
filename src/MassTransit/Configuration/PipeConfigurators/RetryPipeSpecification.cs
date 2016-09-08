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
    using Context;
    using GreenPipes;
    using GreenPipes.Filters;
    using Pipeline.Filters;


    public class RetryPipeSpecification :
        IPipeSpecification<ConsumeContext>
    {
        readonly IRetryPolicy _retryPolicy;

        public RetryPipeSpecification(IRetryPolicy retryPolicy)
        {
            _retryPolicy = retryPolicy;
        }

        public void Apply(IPipeBuilder<ConsumeContext> builder)
        {
            var retryPolicy = new ConsumeContextRetryPolicy(_retryPolicy);

            builder.AddFilter(new RetryFilter<ConsumeContext>(retryPolicy));
        }

        public IEnumerable<ValidationResult> Validate()
        {
            if (_retryPolicy == null)
                yield return this.Failure("RetryPolicy", "must not be null");
        }
    }


    public class RetryPipeSpecification<T> :
        IPipeSpecification<ConsumeContext<T>>
        where T : class
    {
        readonly IRetryPolicy _retryPolicy;

        public RetryPipeSpecification(IRetryPolicy retryPolicy)
        {
            _retryPolicy = retryPolicy;
        }

        public void Apply(IPipeBuilder<ConsumeContext<T>> builder)
        {
            var retryPolicy = new ConsumeContextRetryPolicy<ConsumeContext<T>, RetryConsumeContext<T>>(_retryPolicy, x => new RetryConsumeContext<T>(x));

            builder.AddFilter(new RetryFilter<ConsumeContext<T>>(retryPolicy));
        }

        public IEnumerable<ValidationResult> Validate()
        {
            if (_retryPolicy == null)
                yield return this.Failure("RetryPolicy", "must not be null");
        }
    }
}