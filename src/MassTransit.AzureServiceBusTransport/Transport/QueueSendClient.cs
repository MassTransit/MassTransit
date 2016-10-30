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
    using Microsoft.ServiceBus.Messaging;


    public class QueueSendClient :
        ISendClient
    {
        readonly QueueClient _queueClient;

        public QueueSendClient(QueueClient queueClient)
        {
            _queueClient = queueClient;
        }

        public string Path => _queueClient.Path;

        public Task Send(BrokeredMessage message)
        {
            return _queueClient.SendAsync(message);
        }

        public Task Close()
        {
            return _queueClient.CloseAsync();
        }

        public Task<long> ScheduleSend(BrokeredMessage message, DateTime scheduleEnqueueTimeUtc)
        {
            return _queueClient.ScheduleMessageAsync(message, scheduleEnqueueTimeUtc);
        }

        public Task CancelScheduledSend(long sequenceNumber)
        {
            return _queueClient.CancelScheduledMessageAsync(sequenceNumber);
        }
    }
}