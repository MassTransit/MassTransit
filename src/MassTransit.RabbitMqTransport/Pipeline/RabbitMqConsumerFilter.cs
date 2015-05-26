// Copyright 2007-2014 Chris Patterson, Dru Sellers, Travis Smith, et. al.
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
    using System;
    using System.Threading.Tasks;
    using Logging;
    using MassTransit.Pipeline;


    /// <summary>
    /// A filter that uses the model context to create a basic consumer and connect it to the model
    /// </summary>
    public class RabbitMqConsumerFilter :
        IFilter<ModelContext>
    {
        static readonly ILog _log = Logger.Get<RabbitMqConsumerFilter>();
        readonly IPipe<ReceiveContext> _receivePipe;
        readonly INotifyReceiveObserver _receiveObserver;

        public RabbitMqConsumerFilter(IPipe<ReceiveContext> receivePipe, INotifyReceiveObserver receiveObserver)
        {
            _receivePipe = receivePipe;
            _receiveObserver = receiveObserver;
        }

        async Task IFilter<ModelContext>.Send(ModelContext context, IPipe<ModelContext> next)
        {
            var receiveSettings = context.GetPayload<ReceiveSettings>();

            Uri inputAddress = context.ConnectionContext.HostSettings.GetInputAddress(receiveSettings);

            using (var consumer = new RabbitMqBasicConsumer(context.Model, inputAddress, _receivePipe, _receiveObserver, context.CancellationToken))
            {
                context.Model.BasicConsume(receiveSettings.QueueName, false, consumer);

                RabbitMqConsumerMetrics metrics = await consumer.CompleteTask.ConfigureAwait(false);

                if (_log.IsDebugEnabled)
                {
                    _log.InfoFormat("Consumer {0}: {1} received, {2} concurrent", metrics.ConsumerTag, metrics.DeliveryCount,
                        metrics.ConcurrentDeliveryCount);
                }
            }
        }

        public bool Visit(IPipelineVisitor visitor)
        {
            return visitor.Visit(this, x => _receivePipe.Visit(x));
        }
    }
}