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
namespace MassTransit.RabbitMqTransport.Pipeline
{
    using System.Threading.Tasks;
    using Contexts;
    using Events;
    using GreenPipes;
    using GreenPipes.Agents;
    using Logging;
    using Topology;
    using Transports;


    /// <summary>
    /// A filter that uses the model context to create a basic consumer and connect it to the model
    /// </summary>
    public class RabbitMqConsumerFilter :
        Supervisor,
        IFilter<ModelContext>
    {
        static readonly ILog _log = Logger.Get<RabbitMqConsumerFilter>();
        readonly IDeadLetterTransport _deadLetterTransport;
        readonly IErrorTransport _errorTransport;
        readonly IReceiveObserver _receiveObserver;
        readonly IPipe<ReceiveContext> _receivePipe;
        readonly RabbitMqReceiveEndpointContext _receiveEndpointContext;
        readonly IReceiveTransportObserver _transportObserver;

        public RabbitMqConsumerFilter(IPipe<ReceiveContext> receivePipe, IReceiveObserver receiveObserver, IReceiveTransportObserver transportObserver,
            RabbitMqReceiveEndpointContext receiveEndpointContext, IDeadLetterTransport deadLetterTransport, IErrorTransport errorTransport)
        {
            _receivePipe = receivePipe;
            _receiveObserver = receiveObserver;
            _transportObserver = transportObserver;
            _receiveEndpointContext = receiveEndpointContext;
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

            var consumer = new RabbitMqBasicConsumer(context, inputAddress, _receivePipe, _receiveObserver, _receiveEndpointContext, _deadLetterTransport, _errorTransport);

            await context.BasicConsume(receiveSettings.QueueName, false, consumer).ConfigureAwait(false);

            await consumer.Ready.ConfigureAwait(false);

            Add(consumer);

            await _transportObserver.Ready(new ReceiveTransportReadyEvent(inputAddress)).ConfigureAwait(false);

            try
            {
                await consumer.Completed.ConfigureAwait(false);
            }
            finally
            {
                RabbitMqDeliveryMetrics metrics = consumer;
                await _transportObserver.Completed(new ReceiveTransportCompletedEvent(inputAddress, metrics)).ConfigureAwait(false);

                if (_log.IsDebugEnabled)
                    _log.DebugFormat("Consumer completed {0}: {1} received, {2} concurrent", metrics.ConsumerTag, metrics.DeliveryCount,
                        metrics.ConcurrentDeliveryCount);
            }
        }
    }
}