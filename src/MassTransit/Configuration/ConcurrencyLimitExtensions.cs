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
namespace GreenPipes
{
    using System;
    using MassTransit;
    using MassTransit.PipeConfigurators;


    public static class ConcurrencyLimitExtensions
    {
        /// <summary>
        /// Specify a concurrency limit for tasks executing through the filter. No more than the specified
        /// number of tasks will be allowed to execute concurrently.
        /// </summary>
        /// <param name="configurator"></param>
        /// <param name="concurrencyLimit">The concurrency limit for the subsequent filters in the pipeline</param>
        /// <param name="managementEndpointConfigurator">A management endpoint configurator to support runtime adjustment</param>
        /// <param name="id">An identifier for the concurrency limit to allow selective adjustment</param>
        public static void UseConcurrencyLimit(this IConsumePipeConfigurator configurator, int concurrencyLimit,
            IManagementEndpointConfigurator managementEndpointConfigurator, string id = null)
        {
            if (configurator == null)
                throw new ArgumentNullException(nameof(configurator));

            if (managementEndpointConfigurator != null)
            {
                configurator.AddPrePipeSpecification(new ConcurrencyLimitPipeSpecification<ConsumeContext>(concurrencyLimit, managementEndpointConfigurator, id));
            }
            else
            {
                configurator.AddPrePipeSpecification(new ConcurrencyLimitPipeSpecification<ConsumeContext>(concurrencyLimit));
            }
        }
    }
}