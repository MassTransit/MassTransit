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
namespace MassTransit.HttpTransport.Transport
{
    using System.Threading.Tasks;
    using Contexts;
    using Events;
    using GreenPipes;
    using GreenPipes.Agents;
    using Hosting;
    using Logging;
    using Pipeline;
    using Topology;


    public class HttpConsumerFilter :
        Supervisor,
        IFilter<HttpHostContext>
    {
        static readonly ILog _log = Logger.Get<HttpConsumerFilter>();
        readonly HttpHostSettings _hostSettings;
        readonly IReceiveObserver _receiveObserver;
        readonly IPipe<ReceiveContext> _receivePipe;
        readonly ReceiveSettings _receiveSettings;
        readonly HttpReceiveEndpointContext _context;
        readonly IReceiveTransportObserver _transportObserver;

        public HttpConsumerFilter(IPipe<ReceiveContext> receivePipe, IReceiveObserver receiveObserver, IReceiveTransportObserver transportObserver,
            HttpHostSettings hostSettings, ReceiveSettings receiveSettings, HttpReceiveEndpointContext context)
        {
            _receivePipe = receivePipe;
            _receiveObserver = receiveObserver;
            _transportObserver = transportObserver;
            _hostSettings = hostSettings;
            _receiveSettings = receiveSettings;
            _context = context;
        }

        public void Probe(ProbeContext context)
        {
            context.CreateFilterScope("consumer");
        }

        public async Task Send(HttpHostContext context, IPipe<HttpHostContext> next)
        {
            var inputAddress = context.HostSettings.GetInputAddress();

            var consumer = new HttpConsumer(_receiveObserver, _hostSettings, _receivePipe, _context);

            context.RegisterEndpointHandler(_receiveSettings.PathMatch, consumer);

            await consumer.Ready.ConfigureAwait(false);

            Add(consumer);

            await _transportObserver.Ready(new ReceiveTransportReadyEvent(inputAddress)).ConfigureAwait(false);

            try
            {
                await consumer.Completed.ConfigureAwait(false);
            }
            finally
            {
                HttpConsumerMetrics metrics = consumer;
                await _transportObserver.Completed(new ReceiveTransportCompletedEvent(inputAddress, metrics)).ConfigureAwait(false);

                if (_log.IsDebugEnabled)
                    _log.DebugFormat("Consumer {0} received, {1} concurrent", metrics.DeliveryCount, metrics.ConcurrentDeliveryCount);
            }
        }
    }
}