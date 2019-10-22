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
    using Context;
    using Contexts;
    using Events;
    using GreenPipes;
    using GreenPipes.Agents;
    using Hosting;
    using Pipeline;


    public class HttpConsumerFilter :
        Supervisor,
        IFilter<HttpHostContext>
    {
        readonly HttpHostSettings _hostSettings;
        readonly ReceiveSettings _receiveSettings;
        readonly HttpReceiveEndpointContext _context;

        public HttpConsumerFilter(HttpHostSettings hostSettings, ReceiveSettings receiveSettings,
            HttpReceiveEndpointContext context)
        {
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

            var consumer = new HttpConsumer(_context);

            context.RegisterEndpointHandler(_receiveSettings.PathMatch, consumer);

            await consumer.Ready.ConfigureAwait(false);

            Add(consumer);

            await _context.TransportObservers.Ready(new ReceiveTransportReadyEvent(inputAddress)).ConfigureAwait(false);

            try
            {
                await consumer.Completed.ConfigureAwait(false);
            }
            finally
            {
                HttpConsumerMetrics metrics = consumer;
                await _context.TransportObservers.Completed(new ReceiveTransportCompletedEvent(inputAddress, metrics)).ConfigureAwait(false);

                LogContext.Debug?.Log("Consumer completed: {DeliveryCount} received, {ConcurrentDeliveryCount} concurrent", metrics.DeliveryCount,
                    metrics.ConcurrentDeliveryCount);
            }
        }
    }
}
