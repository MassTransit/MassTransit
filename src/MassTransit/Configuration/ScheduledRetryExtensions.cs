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
    using Context;
    using GreenPipes;
    using GreenPipes.Configurators;
    using PipeConfigurators;
    using Pipeline.Filters;


    public static class ScheduledRetryExtensions
    {
        /// <summary>
        /// Use the message scheduler to schedule redelivery of a specific message type based upon the retry policy.
        /// </summary>
        /// <param name="configurator"></param>
        /// <param name="retryPolicy"></param>
        public static void UseScheduledRedelivery<T>(this IPipeConfigurator<ConsumeContext<T>> configurator, IRetryPolicy retryPolicy)
            where T : class
        {
            if (configurator == null)
                throw new ArgumentNullException(nameof(configurator));

            var redeliverySpecification = new ScheduleMessageRedeliveryPipeSpecification<T>();

            configurator.AddPipeSpecification(redeliverySpecification);

            var retrySpecification = new RedeliveryRetryPipeSpecification<T>();

            retrySpecification.SetRetryPolicy(exceptionFilter =>
                new ConsumeContextRetryPolicy<ConsumeContext<T>, RetryConsumeContext<T>>(retryPolicy, CancellationToken.None, Factory));

            configurator.AddPipeSpecification(retrySpecification);
        }

        static RetryConsumeContext<T> Factory<T>(ConsumeContext<T> context, IRetryPolicy retryPolicy, RetryContext retryContext)
            where T : class
        {
            return new RetryConsumeContext<T>(context, retryPolicy, retryContext);
        }

        /// <summary>
        /// Use the message scheduler to schedule redelivery of a specific message type based upon the retry policy.
        /// </summary>
        /// <param name="configurator"></param>
        /// <param name="configure"></param>
        public static void UseScheduledRedelivery<T>(this IPipeConfigurator<ConsumeContext<T>> configurator, Action<IRetryConfigurator> configure)
            where T : class
        {
            if (configurator == null)
                throw new ArgumentNullException(nameof(configurator));

            var redeliverySpecification = new ScheduleMessageRedeliveryPipeSpecification<T>();

            configurator.AddPipeSpecification(redeliverySpecification);

            var retrySpecification = new RedeliveryRetryPipeSpecification<T>();

            configure?.Invoke(retrySpecification);

            configurator.AddPipeSpecification(retrySpecification);
        }
    }
}
