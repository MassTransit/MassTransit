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
namespace MassTransit.WebJobs.ServiceBusIntegration
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using AzureServiceBusTransport.Contexts;
    using GreenPipes;
    using Logging;
    using Microsoft.Azure.WebJobs;
    using Microsoft.ServiceBus.Messaging;


    public class CollectorSendEndpointContext :
        BasePipeContext,
        SendEndpointContext
    {
        readonly CancellationToken _cancellationToken;
        readonly ILog _log;
        readonly IAsyncCollector<BrokeredMessage> _collector;

        public CollectorSendEndpointContext(string path, ILog log, IAsyncCollector<BrokeredMessage> collector, CancellationToken cancellationToken)
        {
            _collector = collector;
            _log = log;
            _cancellationToken = cancellationToken;
            EntityPath = path;
        }

        public string EntityPath { get; }

        public Task Send(BrokeredMessage message)
        {
            if (_log.IsDebugEnabled)
                _log.DebugFormat("Sending message: {0}", message.MessageId);

            return _collector.AddAsync(message, _cancellationToken);
        }

        public async Task<long> ScheduleSend(BrokeredMessage message, DateTime scheduleEnqueueTimeUtc)
        {
            if (_log.IsDebugEnabled)
                _log.DebugFormat("Scheduling message: {0} for delivery at {1}", message.MessageId, scheduleEnqueueTimeUtc);

            message.ScheduledEnqueueTimeUtc = scheduleEnqueueTimeUtc;

            await _collector.AddAsync(message, _cancellationToken).ConfigureAwait(false);

            return 0;
        }

        public Task CancelScheduledSend(long sequenceNumber)
        {
            throw new NotImplementedException("Cannot cancel a scheduled message when using the collector transport");
        }

        public Task Close()
        {
            return _collector.FlushAsync(_cancellationToken);
        }
    }
}