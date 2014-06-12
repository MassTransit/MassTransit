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
    using System.Collections.Concurrent;
    using System.Threading;
    using System.Threading.Tasks;
    using RabbitMQ.Client;
    using RabbitMQ.Client.Events;


    public class RabbitMQBasicConsumer :
        IBasicConsumer
    {
        readonly Uri _inputAddress;
        readonly IModel _model;
        readonly ConcurrentDictionary<ulong, Task> _pending;
        readonly Func<ReceiveContext, Task> _taskFactory;
        string _consumerTag;

        int _current;
        int _max;


        public RabbitMQBasicConsumer(IModel model, Uri inputAddress, Func<ReceiveContext, Task> taskFactory)
        {
            _model = model;
            _taskFactory = taskFactory;
            _inputAddress = inputAddress;
            _pending = new ConcurrentDictionary<ulong, Task>();
        }

        public void HandleBasicConsumeOk(string consumerTag)
        {
            _consumerTag = consumerTag;
        }

        public void HandleBasicCancelOk(string consumerTag)
        {
            Console.WriteLine("CancelOk: {0}", consumerTag);
        }

        public void HandleBasicCancel(string consumerTag)
        {
            Console.WriteLine("Cancel: {0}", consumerTag);
        }

        public void HandleModelShutdown(IModel model, ShutdownEventArgs reason)
        {
            Console.WriteLine("Model shutdown: {0}", reason);
            Console.WriteLine("Max consumers: {0}", _max);
        }

        public async void HandleBasicDeliver(string consumerTag, ulong deliveryTag, bool redelivered, string exchange,
            string routingKey,
            IBasicProperties properties, byte[] body)
        {
            int current = Interlocked.Increment(ref _current);
            while (current > _max)
                Interlocked.CompareExchange(ref _max, current, _max);

            try
            {
                var context = new RabbitMqReceiveContext(exchange, routingKey, _consumerTag, _inputAddress, deliveryTag,
                    body, redelivered, properties);

                await _taskFactory(context);

                _model.BasicAck(deliveryTag, false);
            }
            catch (Exception)
            {
                _model.BasicNack(deliveryTag, false, true);
            }
            finally
            {
                Interlocked.Decrement(ref _current);
            }
        }

        public IModel Model
        {
            get { return _model; }
        }

        public event ConsumerCancelledEventHandler ConsumerCancelled;
    }
}