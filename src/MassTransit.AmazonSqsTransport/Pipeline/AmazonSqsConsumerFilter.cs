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
namespace MassTransit.AmazonSqsTransport.Pipeline
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Contexts;
    using Events;
    using GreenPipes;
    using GreenPipes.Agents;
    using Logging;
    using Topology;
    using Transports;
    using Transports.Metrics;


    /// <summary>
    /// A filter that uses the model context to create a basic consumer and connect it to the model
    /// </summary>
    public class AmazonSqsConsumerFilter :
        Supervisor,
        IFilter<ModelContext>
    {
        static readonly ILog _log = Logger.Get<AmazonSqsConsumerFilter>();
        readonly IDeadLetterTransport _deadLetterTransport;
        readonly IErrorTransport _errorTransport;
        readonly IReceiveObserver _receiveObserver;
        readonly IPipe<ReceiveContext> _receivePipe;
        readonly AmazonSqsReceiveEndpointContext _context;
        readonly IReceiveTransportObserver _transportObserver;

        public AmazonSqsConsumerFilter(IPipe<ReceiveContext> receivePipe, IReceiveObserver receiveObserver, IReceiveTransportObserver transportObserver,
            AmazonSqsReceiveEndpointContext context, IDeadLetterTransport deadLetterTransport, IErrorTransport errorTransport)
        {
            _receivePipe = receivePipe;
            _receiveObserver = receiveObserver;
            _transportObserver = transportObserver;
            _context = context;
            _deadLetterTransport = deadLetterTransport;
            _errorTransport = errorTransport;
        }

        void IProbeSite.Probe(ProbeContext context)
        {
        }

        async Task IFilter<ModelContext>.Send(ModelContext context, IPipe<ModelContext> next)
        {
            var receiveSettings = context.GetPayload<ReceiveSettings>();

            var inputAddress = receiveSettings.GetInputAddress(context.ConnectionContext.HostSettings.HostAddress);

            var queueUrl = await context.GetQueue(receiveSettings.EntityName).ConfigureAwait(false);

            var consumer = new AmazonSqsBasicConsumer(context, queueUrl, inputAddress, _receivePipe, _receiveObserver, _context, _deadLetterTransport, _errorTransport);

            await context.BasicConsume(queueUrl, receiveSettings.PrefetchCount, consumer).ConfigureAwait(false);

            await consumer.Ready.ConfigureAwait(false);

            Add(consumer);

            await _transportObserver.Ready(new ReceiveTransportReadyEvent(inputAddress)).ConfigureAwait(false);

            try
            {
                await consumer.Completed.ConfigureAwait(false);
            }
            finally
            {
                DeliveryMetrics metrics = consumer;
                await _transportObserver.Completed(new ReceiveTransportCompletedEvent(inputAddress, metrics)).ConfigureAwait(false);

                if (_log.IsDebugEnabled)
                    _log.DebugFormat("Consumer completed: {0} received, {0} concurrent", metrics.DeliveryCount, metrics.ConcurrentDeliveryCount);
            }
        }
    }
}
