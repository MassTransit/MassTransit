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
namespace MassTransit.AzureServiceBusTransport.Configuration
{
    using System;
    using Microsoft.ServiceBus.Messaging;


    public class ReceiveEndpointSettings :
        ReceiveSettings
    {
        public ReceiveEndpointSettings(string queuePath)
        {
            QueueDescription = new QueueDescription(queuePath);
            QueueDescription.EnableBatchedOperations = true;
            QueueDescription.MaxDeliveryCount = 5;
            QueueDescription.DefaultMessageTimeToLive = TimeSpan.FromDays(365);
            QueueDescription.LockDuration = TimeSpan.FromMinutes(5);
            QueueDescription.EnableDeadLetteringOnMessageExpiration = true;

            MaxConcurrentCalls = Math.Max(Environment.ProcessorCount, 8);
            PrefetchCount = Math.Max(MaxConcurrentCalls, 32);
        }

        public int PrefetchCount { get; set; }
        public int MaxConcurrentCalls { get; set; }
        public QueueDescription QueueDescription { get; }
        public TimeSpan AutoRenewTimeout { get; set; }
    }
}