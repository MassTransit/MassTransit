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
namespace MassTransit.WindowsServiceBusTransport
{
    /// <summary>
    /// Configure an Azure Service Bus receive endpoint
    /// </summary>
    public interface IServiceBusReceiveEndpointConfigurator :
        IReceiveEndpointConfigurator,
        IQueueConfigurator
    {
        /// <summary>
        /// Specify the number of messages to prefetch from the queue to the service
        /// </summary>
        /// <value>The limit</value>
        int PrefetchCount { set; }

        /// <summary>
        /// Specify the number of concurrent consumers (separate from prefetch count)
        /// </summary>
        int MaxConcurrentCalls { set; }

        /// <summary>
        /// The host on which the endpoint is being configured
        /// </summary>
        IServiceBusHost Host { get; }

        /// <summary>
        /// If true, subscribes the message type exchanges to the queue
        /// </summary>
        bool SubscribeMessageTopics { set; }
    }
}