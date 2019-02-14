// Copyright 2007-2019 Chris Patterson, Dru Sellers, Travis Smith, et. al.
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
namespace MassTransit.ActiveMqTransport
{
    using System;
    using GreenPipes;


    static class ActiveMqEndpointDefinitionExtensions
    {
        /// <summary>
        /// We may want to have a builder/endpoint context that could store things like management endpoint, etc. to configure
        /// filters and add configuration interfaces for things like concurrency limit and prefetch count
        /// </summary>
        /// <param name="configurator"></param>
        /// <param name="definition"></param>
        /// <param name="configure">The callback to invoke after the definition configuration has been applied</param>
        internal static void Apply(this IActiveMqReceiveEndpointConfigurator configurator, IEndpointDefinition definition,
            Action<IActiveMqReceiveEndpointConfigurator> configure = null)
        {
            if (definition.IsTemporary)
            {
                configurator.AutoDelete = true;
                configurator.Durable = false;
            }

            if (definition.ConcurrentMessageLimit.HasValue)
            {
                configurator.UseConcurrencyLimit(definition.ConcurrentMessageLimit.Value);
            }

            definition.Configure(configurator);

            configure?.Invoke(configurator);
        }
    }
}
