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
namespace MassTransit.Transports.RabbitMq
{
    using System;
    using System.Threading.Tasks;
    using Logging;
    using Pipeline;
    using RabbitMQ.Client;


    public class ModelConsumerFilter :
        IFilter<ModelContext>
    {
        readonly ILog _log = Logger.Get<ModelConsumerFilter>();

        readonly IPipe<ReceiveContext> _pipe;
        readonly ReceiveConsumerSettings _settings;

        public ModelConsumerFilter(IPipe<ReceiveContext> pipe, ReceiveConsumerSettings settings)
        {
            _pipe = pipe;
            _settings = settings;
        }

        public async Task Send(ModelContext context, IPipe<ModelContext> next)
        {
            var taskCompletion = new TaskCompletionSource<bool>();

            var registration = context.CancellationToken.Register(() => taskCompletion.TrySetResult(false));

            context.Model.BasicQos(0, _settings.PrefetchCount, false);

            context.Model.ModelShutdown += (m, args) => taskCompletion.TrySetResult(true);

            var inputAddress = new Uri("rabbitmq://localhost/speed/input");

            QueueDeclareOk queueOk = context.Model.QueueDeclare(_settings.QueueName, _settings.Durable, _settings.Exclusive,
                _settings.AutoDelete, _settings.QueueArguments);

            string queueName = queueOk.QueueName;

            if (_settings.PurgeOnReceive)
                context.Model.QueuePurge(_settings.QueueName);

            if (!string.IsNullOrWhiteSpace(_settings.ExchangeName) || string.IsNullOrWhiteSpace(_settings.QueueName))
            {
                string exchangeName = _settings.ExchangeName ?? queueName;

                context.Model.ExchangeDeclare(exchangeName, _settings.ExchangeType, _settings.Durable, _settings.AutoDelete,
                    _settings.ExchangeArguments);

                context.Model.QueueBind(queueName, exchangeName, "");
            }


            var consumer = new RabbitMqBasicConsumer(context.Model, inputAddress, _pipe);

            consumer.ConsumerCancelled += (_, args) => taskCompletion.TrySetResult(true);

            context.Model.BasicConsume(queueName, false, consumer);

            await taskCompletion.Task;

            registration.Dispose();
        }

        public bool Inspect(IPipeInspector inspector)
        {
            return inspector.Inspect(this, (x, _) => _pipe.Inspect(x));
        }
    }
}