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
    using System.Threading;
    using System.Threading.Tasks;
    using Context;
    using Logging;
    using MassTransit.Pipeline;


    public class ModelConsumerFilter :
        IFilter<ModelContext>
    {
        readonly ILog _log = Logger.Get<ModelConsumerFilter>();

        readonly IPipe<ReceiveContext> _pipe;

        public ModelConsumerFilter(IPipe<ReceiveContext> pipe)
        {
            _pipe = pipe;
        }

        public async Task Send(ModelContext context, IPipe<ModelContext> next)
        {
            var taskCompletion = new TaskCompletionSource<bool>();

            CancellationTokenRegistration registration = context.CancellationToken.Register(() => taskCompletion.TrySetResult(false));

            context.Model.ModelShutdown += (m, args) => taskCompletion.TrySetResult(true);

            var inputAddress = new Uri("rabbitmq://localhost/speed/input");


            var consumer = new RabbitMqBasicConsumer(context.Model, inputAddress, _pipe);

            consumer.ConsumerCancelled += (_, args) => taskCompletion.TrySetResult(true);


            ReceiveSettings receiveSettings;
            if (!context.TryGetPayload(out receiveSettings))
                throw new PayloadNotFoundException("The ReceiveSettings were not present");

            context.Model.BasicConsume(receiveSettings.QueueName, false, consumer);

            await taskCompletion.Task;

            registration.Dispose();
        }

        public bool Inspect(IPipeInspector inspector)
        {
            return inspector.Inspect(this, (x, _) => _pipe.Inspect(x));
        }
    }
}