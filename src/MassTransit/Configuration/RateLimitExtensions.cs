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


    public static class RateLimitExtensions
    {
        /// <summary>
        /// Specify a rate limit for message processing, so that only the specified number of messages are allowed
        /// per interval.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="configurator"></param>
        /// <param name="rateLimit">The number of messages allowed per interval</param>
        public static void UseRateLimit<T>(this IPipeConfigurator<T> configurator, int rateLimit)
            where T : class, PipeContext
        {
            if (configurator == null)
                throw new ArgumentNullException("configurator");

            var specification = new RateLimitPipeSpecification<T>(rateLimit, TimeSpan.FromSeconds(1));

            configurator.AddPipeSpecification(specification);
        }

        /// <summary>
        /// Specify a rate limit for message processing, so that only the specified number of messages are allowed
        /// per interval.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="configurator"></param>
        /// <param name="rateLimit">The number of messages allowed per interval</param>
        /// <param name="interval">The reset interval for each set of messages</param>
        public static void UseRateLimit<T>(this IPipeConfigurator<T> configurator, int rateLimit, TimeSpan interval)
            where T : class, PipeContext
        {
            if (configurator == null)
                throw new ArgumentNullException("configurator");

            var specification = new RateLimitPipeSpecification<T>(rateLimit, interval);

            configurator.AddPipeSpecification(specification);
        }
    }
}