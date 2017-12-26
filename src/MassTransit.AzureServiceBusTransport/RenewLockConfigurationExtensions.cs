// Copyright 2007-2017 Chris Patterson, Dru Sellers, Travis Smith, et. al.
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
    using AzureServiceBusTransport.Configuration.Specifications;
    using GreenPipes;


    public static class RenewLockConfigurationExtensions
    {
        /// <summary>
        /// Use the automatic lock renewal filter with Azure Service Bus, so that longer running consumers can run without losing the
        /// message lock.
        /// </summary>
        /// <param name="configurator"></param>
        /// <param name="delay">The delay before the lock is renewed (should be reasonably less than the lock time).</param>
        public static void UseRenewLock(this IPipeConfigurator<ConsumeContext> configurator, TimeSpan? delay = default(TimeSpan?))
        {
            var specification = new RenewLockSpecification(delay);

            configurator.AddPipeSpecification(specification);
        }
    }
}