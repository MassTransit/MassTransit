// Copyright 2007-2015 Chris Patterson, Dru Sellers, Travis Smith, et. al.
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
    using PipeConfigurators;
    using Saga;


    public static class RateLimitExtensions
    {
        /// <summary>
        /// Specify a rate limit for message processing, so that only the specified number of messages are allowed
        /// per interval.
        /// </summary>
        /// <param name="configurator"></param>
        /// <param name="rateLimit">The number of messages allowed per interval</param>
        /// <param name="interval">The reset interval for each set of messages</param>
        public static void UseRateLimit(this IPipeConfigurator<ConsumeContext> configurator, int rateLimit, TimeSpan? interval = default(TimeSpan?))
        {
            ConfigureRateLimit(configurator, rateLimit, interval);
        }

        /// <summary>
        /// Specify a rate limit for message processing, so that only the specified number of messages are allowed
        /// per interval.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="configurator"></param>
        /// <param name="rateLimit">The number of messages allowed per interval</param>
        /// <param name="interval">The reset interval for each set of messages</param>
        public static void UseRateLimit<T>(this IPipeConfigurator<ConsumeContext<T>> configurator, int rateLimit, TimeSpan? interval = default(TimeSpan?))
            where T : class
        {
            ConfigureRateLimit(configurator, rateLimit, interval);
        }

        /// <summary>
        /// Specify a rate limit for message processing, so that only the specified number of messages are allowed
        /// per interval.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="configurator"></param>
        /// <param name="rateLimit">The number of messages allowed per interval</param>
        /// <param name="interval">The reset interval for each set of messages</param>
        public static void UseRateLimit<T>(this IPipeConfigurator<SagaConsumeContext<T>> configurator, int rateLimit, TimeSpan? interval = default(TimeSpan?))
            where T : class, ISaga
        {
            ConfigureRateLimit(configurator, rateLimit, interval);
        }

        /// <summary>
        /// Specify a rate limit for message processing, so that only the specified number of messages are allowed
        /// per interval.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="configurator"></param>
        /// <param name="rateLimit">The number of messages allowed per interval</param>
        /// <param name="interval">The reset interval for each set of messages</param>
        public static void UseRateLimit<T>(this IPipeConfigurator<ConsumerConsumeContext<T>> configurator, int rateLimit,
            TimeSpan? interval = default(TimeSpan?))
            where T : class
        {
            ConfigureRateLimit(configurator, rateLimit, interval);
        }

        static void ConfigureRateLimit<T>(IPipeConfigurator<T> configurator, int rateLimit, TimeSpan? interval)
            where T : class, PipeContext
        {
            if (configurator == null)
                throw new ArgumentNullException(nameof(configurator));

            var specification = new RateLimitPipeSpecification<T>(rateLimit, interval ?? TimeSpan.FromSeconds(1));

            configurator.AddPipeSpecification(specification);
        }
    }
}