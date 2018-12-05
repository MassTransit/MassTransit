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
namespace MassTransit.WebJobs.ServiceBusIntegration.Contexts
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using Azure.ServiceBus.Core.Contexts;
    using GreenPipes;
    using Logging;
    using Microsoft.Azure.ServiceBus;
    using Microsoft.Azure.WebJobs;


    public class CollectorMessageSendEndpointContext :
        BasePipeContext,
        SendEndpointContext
    {
        readonly CancellationToken _cancellationToken;
        readonly ILog _log;
        readonly IAsyncCollector<Message> _collector;

        public CollectorMessageSendEndpointContext(string path, ILog log, IAsyncCollector<Message> collector, CancellationToken cancellationToken)
        {
            _collector = collector;
            _log = log;
            _cancellationToken = cancellationToken;
            EntityPath = path;
        }

        public string EntityPath { get; }

        public Task Send(Message message)
        {
            if (_log.IsDebugEnabled)
                _log.DebugFormat("Sending message: {0}", message.MessageId);

            return _collector.AddAsync(message, _cancellationToken);
        }

        public async Task<long> ScheduleSend(Message message, DateTime scheduleEnqueueTimeUtc)
        {
            if (_log.IsDebugEnabled)
                _log.DebugFormat("Scheduling message: {0} for delivery at {1}", message.MessageId, scheduleEnqueueTimeUtc);

            message.ScheduledEnqueueTimeUtc = scheduleEnqueueTimeUtc;

            await _collector.AddAsync(message, _cancellationToken).ConfigureAwait(false);

            return 0;
        }

        public Task CancelScheduledSend(long sequenceNumber)
        {
            return Task.CompletedTask;
        }

        public Task Close()
        {
            return _collector.FlushAsync(_cancellationToken);
        }
    }
}