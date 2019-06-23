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
    using Context;
    using Contexts;
    using Events;
    using GreenPipes;
    using GreenPipes.Agents;
    using Topology;


    /// <summary>
    /// A filter that uses the model context to create a basic consumer and connect it to the model
    /// </summary>
    public class RabbitMqConsumerFilter :
        Supervisor,
        IFilter<ModelContext>
    {
        readonly RabbitMqReceiveEndpointContext _receiveEndpointContext;

        public RabbitMqConsumerFilter(RabbitMqReceiveEndpointContext receiveEndpointContext)
        {
            _receiveEndpointContext = receiveEndpointContext;
        }

        void IProbeSite.Probe(ProbeContext context)
        {
        }

        async Task IFilter<ModelContext>.Send(ModelContext context, IPipe<ModelContext> next)
        {
            var receiveSettings = context.GetPayload<ReceiveSettings>();

            var inputAddress = receiveSettings.GetInputAddress(context.ConnectionContext.HostAddress);

            var consumer = new RabbitMqBasicConsumer(context, inputAddress, _receiveEndpointContext);

            await context.BasicConsume(receiveSettings.QueueName, false, _receiveEndpointContext.ExclusiveConsumer, receiveSettings.ConsumeArguments, consumer)
                .ConfigureAwait(false);

            await consumer.Ready.ConfigureAwait(false);

            Add(consumer);

            await _receiveEndpointContext.TransportObservers.Ready(new ReceiveTransportReadyEvent(inputAddress)).ConfigureAwait(false);

            try
            {
                await consumer.Completed.ConfigureAwait(false);
            }
            finally
            {
                RabbitMqDeliveryMetrics metrics = consumer;
                await _receiveEndpointContext.TransportObservers.Completed(new ReceiveTransportCompletedEvent(inputAddress, metrics)).ConfigureAwait(false);

                LogContext.Debug?.Log("Consumer completed {ConsumerTag}: {DeliveryCount} received, {ConcurrentDeliveryCount} concurrent", metrics.ConsumerTag,
                    metrics.DeliveryCount, metrics.ConcurrentDeliveryCount);
            }
        }
    }
}
