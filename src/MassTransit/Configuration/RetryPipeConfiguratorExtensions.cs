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
namespace MassTransit
{
    using System;
    using System.Threading;
    using PipeConfigurators;
    using Policies;
    using Saga;


    public static class RetryPipeConfiguratorExtensions
    {
        public static void UseRetry(this IPipeConfigurator<ConsumeContext> configurator, IRetryPolicy retryPolicy)
        {
            if (configurator == null)
                throw new ArgumentNullException(nameof(configurator));

            var pipeBuilderConfigurator = new RetryPipeSpecification(retryPolicy);

            configurator.AddPipeSpecification(pipeBuilderConfigurator);
        }

        public static void UseRetry<T>(this IPipeConfigurator<ConsumeContext<T>> configurator, IRetryPolicy retryPolicy)
            where T : class
        {
            if (configurator == null)
                throw new ArgumentNullException(nameof(configurator));

            var pipeBuilderConfigurator = new RetryPipeSpecification<T>(retryPolicy);

            configurator.AddPipeSpecification(pipeBuilderConfigurator);
        }

        public static void UseRetry<TConsumer>(this IPipeConfigurator<ConsumerConsumeContext<TConsumer>> configurator, IRetryPolicy retryPolicy)
            where TConsumer : class
        {
            if (configurator == null)
                throw new ArgumentNullException(nameof(configurator));

            var pipeBuilderConfigurator = new RetryConsumerPipeSpecification<TConsumer>(retryPolicy);

            configurator.AddPipeSpecification(pipeBuilderConfigurator);
        }

        public static void UseRetry<TSaga>(this IPipeConfigurator<SagaConsumeContext<TSaga>> configurator, IRetryPolicy retryPolicy)
            where TSaga : class, ISaga
        {
            if (configurator == null)
                throw new ArgumentNullException(nameof(configurator));

            var pipeBuilderConfigurator = new RetrySagaPipeSpecification<TSaga>(retryPolicy);

            configurator.AddPipeSpecification(pipeBuilderConfigurator);
        }

        /// <summary>
        /// Retry the subsequent filter pipe using the specified retry policy
        /// </summary>
        /// <param name="configurator">The pipe configurator</param>
        /// <param name="retryPolicy">The retry policy</param>
        /// <param name="cancellationToken">The cancellation token to end the retry operations</param>
        public static void UseRetry(this IPipeConfigurator<ConsumeContext> configurator, IRetryPolicy retryPolicy, CancellationToken cancellationToken)
        {
            if (configurator == null)
                throw new ArgumentNullException(nameof(configurator));
            if (retryPolicy == null)
                throw new ArgumentNullException(nameof(retryPolicy));

            retryPolicy = new CancelRetryPolicy(retryPolicy, cancellationToken);

            var pipeBuilderConfigurator = new RetryPipeSpecification(retryPolicy);

            configurator.AddPipeSpecification(pipeBuilderConfigurator);
        }
    }
}