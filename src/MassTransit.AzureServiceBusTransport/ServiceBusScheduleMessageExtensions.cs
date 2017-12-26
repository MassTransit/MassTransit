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
    using AzureServiceBusTransport.Configuration.Specifications;
    using GreenPipes;


    public static class ServiceBusScheduleMessageExtensions
    {
        /// <summary>
        /// Uses the Enqueue time of Service Bus messages to schedule future delivery of messages instead
        /// of using Quartz. A natively supported feature that is highly reliable.
        /// </summary>
        /// <param name="configurator"></param>
        public static void UseServiceBusMessageScheduler(this IPipeConfigurator<ConsumeContext> configurator)
        {
            if (configurator == null)
                throw new ArgumentNullException(nameof(configurator));

            var pipeBuilderConfigurator = new ServiceBusMessageSchedulerSpecification();

            configurator.AddPipeSpecification(pipeBuilderConfigurator);
        }
    }
}