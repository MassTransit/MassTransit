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
namespace MassTransit.AzureServiceBusTransport.Transport
{
    using System;
    using System.Threading.Tasks;
#if !NETCORE
    using Microsoft.ServiceBus.Messaging;
#else
    using Microsoft.Azure.ServiceBus;
#endif

    public interface ISendClient
    {
        /// <summary>
        /// The path of the messaging entity for this client
        /// </summary>
        string Path { get; }

        /// <summary>
        /// Send the message to the messaging entity
        /// </summary>
        /// <param name="message"></param>
        /// <returns></returns>
        Task Send(BrokeredMessage message);

        /// <summary>
        /// Close the messaging client
        /// </summary>
        /// <returns></returns>
        Task Close();

        /// <summary>
        /// Schedule a send in the future to the messaging entity
        /// </summary>
        /// <param name="message"></param>
        /// <param name="scheduleEnqueueTimeUtc"></param>
        /// <returns></returns>
        Task<long> ScheduleSend(BrokeredMessage message, DateTime scheduleEnqueueTimeUtc);

        /// <summary>
        /// Cancel a previously schedule send on the messaging entity
        /// </summary>
        /// <param name="sequenceNumber"></param>
        /// <returns></returns>
        Task CancelScheduledSend(long sequenceNumber);
    }
}