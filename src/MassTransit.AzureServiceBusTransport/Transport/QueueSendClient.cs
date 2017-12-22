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
namespace MassTransit.AzureServiceBusTransport.Transport
{
    using System;
    using System.Threading.Tasks;
    using GreenPipes;
    using Microsoft.ServiceBus.Messaging;
    using Policies;


    public class QueueSendClient :
        ISendClient
    {
        readonly QueueClient _queueClient;
        readonly IRetryPolicy _retryPolicy;

        public QueueSendClient(QueueClient queueClient)
        {
            _queueClient = queueClient;

            _retryPolicy = Retry.CreatePolicy(x =>
            {
                x.Handle<ServerBusyException>();
                x.Handle<MessagingException>(exception => exception.IsTransient || exception.IsWrappedExceptionTransient());
                x.Handle<MessagingCommunicationException>(exception => exception.IsTransient || exception.IsWrappedExceptionTransient());
                x.Handle<TimeoutException>();

                x.Interval(5, TimeSpan.FromSeconds(10));
            });
        }

        public string Path => _queueClient.Path;

        public async Task Send(BrokeredMessage message)
        {
            try
            {
                await _retryPolicy.Retry(() => _queueClient.SendAsync(message)).ConfigureAwait(false);
            }
            catch (InvalidOperationException exception) when (exception.Message.Contains("been consumed"))
            {
                // this is okay, it means we timed out and upon retry the message was accepted
            }
        }

        public Task Close()
        {
            return _queueClient.CloseAsync();
        }

        public Task<long> ScheduleSend(BrokeredMessage message, DateTime scheduleEnqueueTimeUtc)
        {
            return _retryPolicy.Retry(() => _queueClient.ScheduleMessageAsync(message, scheduleEnqueueTimeUtc));
        }

        public Task CancelScheduledSend(long sequenceNumber)
        {
            return _queueClient.CancelScheduledMessageAsync(sequenceNumber);
        }
    }
}