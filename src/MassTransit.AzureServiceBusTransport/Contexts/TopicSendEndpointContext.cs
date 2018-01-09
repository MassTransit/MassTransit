// Copyright 2007-2018 Chris Patterson, Dru Sellers, Travis Smith, et. al.
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
namespace MassTransit.AzureServiceBusTransport.Contexts
{
    using System;
    using System.Threading.Tasks;
    using GreenPipes;
    using Microsoft.ServiceBus.Messaging;


    public class TopicSendEndpointContext :
        BasePipeContext,
        SendEndpointContext
    {
        readonly TopicClient _client;

        public TopicSendEndpointContext(TopicClient client)
        {
            _client = client;
        }

        public string EntityPath => _client.Path;

        public Task Send(BrokeredMessage message)
        {
            return _client.SendAsync(message);
        }

        public Task<long> ScheduleSend(BrokeredMessage message, DateTime scheduleEnqueueTimeUtc)
        {
            return _client.ScheduleMessageAsync(message, scheduleEnqueueTimeUtc);
        }

        public Task CancelScheduledSend(long sequenceNumber)
        {
            return _client.CancelScheduledMessageAsync(sequenceNumber);
        }
    }
}