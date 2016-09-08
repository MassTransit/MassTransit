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


    public class RetryConsumerPipeSpecification<TConsumer> :
        IPipeSpecification<ConsumerConsumeContext<TConsumer>>
        where TConsumer : class
    {
        readonly IRetryPolicy _retryPolicy;

        public RetryConsumerPipeSpecification(IRetryPolicy retryPolicy)
        {
            _retryPolicy = retryPolicy;
        }

        public void Apply(IPipeBuilder<ConsumerConsumeContext<TConsumer>> builder)
        {
            var retryPolicy = new ConsumeContextRetryPolicy<ConsumerConsumeContext<TConsumer>, RetryConsumerConsumeContext<TConsumer>>(_retryPolicy,
                x => new RetryConsumerConsumeContext<TConsumer>(x));

            builder.AddFilter(new RetryFilter<ConsumerConsumeContext<TConsumer>>(retryPolicy));
        }

        public IEnumerable<ValidationResult> Validate()
        {
            if (_retryPolicy == null)
                yield return this.Failure("RetryPolicy", "must not be null");
        }
    }
}