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


    public static class ConcurrencyLimitExtensions
    {
        /// <summary>
        /// Specify a concurrency limit for tasks executing through the filter. No more than the specified
        /// number of tasks will be allowed to execute concurrently.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="configurator"></param>
        /// <param name="concurrencyLimit">The concurrency limit for the subsequent filters in the pipeline</param>
        public static void UseConcurrencyLimit<T>(this IPipeConfigurator<T> configurator, int concurrencyLimit)
            where T : class, PipeContext
        {
            if (configurator == null)
                throw new ArgumentNullException(nameof(configurator));

            var specification = new ConcurrencyLimitPipeSpecification<T>(concurrencyLimit);

            configurator.AddPipeSpecification(specification);
        }
    }
}