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
    using System.Threading;
    using System.Threading.Tasks;
    using AzureServiceBusTransport.Contexts;
    using GreenPipes;
    using Logging;
    using Microsoft.Azure.WebJobs;
    using Microsoft.ServiceBus.Messaging;


    public class CollectorEventDataSendEndpointContext :
        BasePipeContext,
        EventDataSendEndpointContext
    {
        readonly CancellationToken _cancellationToken;
        readonly IAsyncCollector<EventData> _collector;
        readonly ILog _log;

        public CollectorEventDataSendEndpointContext(string path, ILog log, IAsyncCollector<EventData> collector, CancellationToken cancellationToken)
        {
            _collector = collector;
            _log = log;
            _cancellationToken = cancellationToken;
            EntityPath = path;
        }

        public string EntityPath { get; }

        public Task Send(EventData message)
        {
            return _collector.AddAsync(message, _cancellationToken);
        }
    }
}