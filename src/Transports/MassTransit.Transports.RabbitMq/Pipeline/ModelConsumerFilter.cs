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
namespace MassTransit.Transports.RabbitMq.Pipeline
{
    using System;
    using System.Threading.Tasks;
    using MassTransit.Pipeline;


    public class ModelConsumerFilter :
        IFilter<ModelContext>
    {
        readonly IPipe<ReceiveContext> _pipe;

        public ModelConsumerFilter(IPipe<ReceiveContext> pipe)
        {
            _pipe = pipe;
        }

        public async Task Send(ModelContext context, IPipe<ModelContext> next)
        {
            var receiveSettings = context.GetPayload<ReceiveSettings>();

            Uri inputAddress = context.ConnectionContext.GetAddress(receiveSettings.QueueName);

            using (var consumer = new RabbitMqBasicConsumer(context.Model, inputAddress, _pipe, context.CancellationToken))
            {
                context.Model.BasicConsume(receiveSettings.QueueName, false, consumer);

                await consumer.CompleteTask;
            }
        }

        public bool Inspect(IPipeInspector inspector)
        {
            return inspector.Inspect(this, (x, _) => _pipe.Inspect(x));
        }
    }
}