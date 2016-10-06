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
namespace MassTransit.AzureServiceBusTransport
{
    using System.Threading.Tasks;
    using GreenPipes;
    using Microsoft.ServiceBus;
    using Microsoft.ServiceBus.Messaging;
    using Transports;
    using Util;


    /// <summary>
    /// An Azure ServiceBus Host, which caches the messaging factory and namespace manager
    /// </summary>
    public interface IServiceBusHost :
        IBusHost
    {
        ServiceBusHostSettings Settings { get; }

        Task<MessagingFactory> MessagingFactory { get; }

        /// <summary>
        /// Session-based messages with state require the use of a net-tcp style client
        /// </summary>
        Task<MessagingFactory> SessionMessagingFactory { get; }

        Task<NamespaceManager> NamespaceManager { get; }

        Task<NamespaceManager> RootNamespaceManager { get; }

        IMessageNameFormatter MessageNameFormatter { get; }

        /// <summary>
        /// The supervisor for the host, which indicates when it's being stopped
        /// </summary>
        ITaskSupervisor Supervisor { get; }

        string GetQueuePath(QueueDescription queueDescription);

        IRetryPolicy RetryPolicy { get; }
    }
}