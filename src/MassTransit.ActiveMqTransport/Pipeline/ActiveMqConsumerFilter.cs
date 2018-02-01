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
namespace MassTransit.ActiveMqTransport.Pipeline
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
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
    public class ActiveMqConsumerFilter :
        Supervisor,
        IFilter<SessionContext>
    {
        static readonly ILog _log = Logger.Get<ActiveMqConsumerFilter>();
        readonly IDeadLetterTransport _deadLetterTransport;
        readonly IErrorTransport _errorTransport;
        readonly IReceiveObserver _receiveObserver;
        readonly IPipe<ReceiveContext> _receivePipe;
        readonly IActiveMqReceiveEndpointTopology _topology;
        readonly IReceiveTransportObserver _transportObserver;

        public ActiveMqConsumerFilter(IPipe<ReceiveContext> receivePipe, IReceiveObserver receiveObserver, IReceiveTransportObserver transportObserver,
            IActiveMqReceiveEndpointTopology topology, IDeadLetterTransport deadLetterTransport, IErrorTransport errorTransport)
        {
            _receivePipe = receivePipe;
            _receiveObserver = receiveObserver;
            _transportObserver = transportObserver;
            _topology = topology;
            _deadLetterTransport = deadLetterTransport;
            _errorTransport = errorTransport;
        }

        void IProbeSite.Probe(ProbeContext context)
        {
        }

        async Task IFilter<SessionContext>.Send(SessionContext context, IPipe<SessionContext> next)
        {
            var receiveSettings = context.GetPayload<ReceiveSettings>();

            var inputAddress = receiveSettings.GetInputAddress(context.ConnectionContext.HostSettings.HostAddress);

            List<Task<ActiveMqBasicConsumer>> consumers = new List<Task<ActiveMqBasicConsumer>>();

            consumers.Add(CreateConsumer(context, inputAddress, receiveSettings.EntityName, receiveSettings.Selector));

            consumers.AddRange(_topology.BrokerTopology.Consumers.Select(x => CreateConsumer(context, inputAddress, x.Destination.EntityName, x.Selector)));

            var actualConsumers = await Task.WhenAll(consumers).ConfigureAwait(false);

            await _transportObserver.Ready(new ReceiveTransportReadyEvent(inputAddress)).ConfigureAwait(false);

            try
            {
                await Task.WhenAll(actualConsumers.Select(x => x.Completed)).ConfigureAwait(false);
            }
            finally
            {
                var consumerMetrics = actualConsumers.Cast<DeliveryMetrics>().ToArray();

                DeliveryMetrics metrics =
                    new CombinedDeliveryMetrics(consumerMetrics.Sum(x => x.DeliveryCount), consumerMetrics.Max(x => x.ConcurrentDeliveryCount));

                await _transportObserver.Completed(new ReceiveTransportCompletedEvent(inputAddress, metrics)).ConfigureAwait(false);

                if (_log.IsDebugEnabled)
                    _log.DebugFormat("Consumer completed: {0} received, {0} concurrent", metrics.DeliveryCount, metrics.ConcurrentDeliveryCount);
            }
        }

        async Task<ActiveMqBasicConsumer> CreateConsumer(SessionContext context, Uri inputAddress, string entityName, string selector)
        {
            var queue = await context.GetQueue(entityName).ConfigureAwait(false);

            var messageConsumer = await context.CreateMessageConsumer(queue, selector, false).ConfigureAwait(false);

            var consumer = new ActiveMqBasicConsumer(context, messageConsumer, inputAddress, _receivePipe, _receiveObserver, _topology, _deadLetterTransport,
                _errorTransport);

            await consumer.Ready.ConfigureAwait(false);

            Add(consumer);

            return consumer;
        }


        class CombinedDeliveryMetrics :
            DeliveryMetrics
        {
            public CombinedDeliveryMetrics(long deliveryCount, int concurrentDeliveryCount)
            {
                DeliveryCount = deliveryCount;
                ConcurrentDeliveryCount = concurrentDeliveryCount;
            }

            public long DeliveryCount { get; }
            public int ConcurrentDeliveryCount { get; }
        }
    }
}