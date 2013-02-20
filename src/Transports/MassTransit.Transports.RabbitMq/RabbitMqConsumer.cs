// Copyright 2007-2012 Chris Patterson, Dru Sellers, Travis Smith, et. al.
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
    using Logging;
    using RabbitMQ.Client;
    using RabbitMQ.Client.Events;

    public class RabbitMqConsumer :
        ConnectionBinding<RabbitMqConnection>
    {
        static readonly ILog _log = Logger.Get(typeof(RabbitMqConsumer));
        readonly IRabbitMqEndpointAddress _address;
        IModel _channel;
        QueueingBasicConsumer _consumer;
        bool _purgeOnBind;

        public RabbitMqConsumer(IRabbitMqEndpointAddress address, bool purgeOnBind)
        {
            _address = address;
            _purgeOnBind = purgeOnBind;
        }

        public void Bind(RabbitMqConnection connection)
        {
            _channel = connection.Connection.CreateModel();

            BindQueue();

            PurgeIfRequested();

            _channel.BasicQos(0, 10, false);

            _consumer = new QueueingBasicConsumer(_channel);
            _channel.BasicConsume(_address.Name, false, _consumer);
        }

        void PurgeIfRequested()
        {
            if (_purgeOnBind)
            {
                _channel.QueuePurge(_address.Name);
                _purgeOnBind = false;
            }
        }

        void BindQueue()
        {
            string queue = _channel.QueueDeclare(_address.Name, true, false, false, _address.QueueArguments());
            _channel.ExchangeDeclare(_address.Name, ExchangeType.Fanout, true);
            _channel.QueueBind(queue, _address.Name, "");
        }

        public void Unbind(RabbitMqConnection connection)
        {
            try
            {
                _channel.Close(200, "unbind consumer");
                _consumer = null;
            }
            catch (Exception ex)
            {
                _log.Error("Failed to close channel: " + _address, ex);
            }

            try
            {
                _channel.Dispose();
            }
            catch (Exception ex)
            {
                _log.Error("Failed to dispose channel: " + _address, ex);
            }
            finally
            {
                _channel = null;
            }
        }

        public BasicDeliverEventArgs Get(TimeSpan timeout)
        {
            object result;
            _consumer.Queue.Dequeue((int)timeout.TotalMilliseconds, out result);

            return (BasicDeliverEventArgs)result;
        }

        public void MessageCompleted(BasicDeliverEventArgs result)
        {
            _channel.BasicAck(result.DeliveryTag, false);
        }

        public void MessageFailed(BasicDeliverEventArgs result)
        {
            _channel.BasicPublish(_address.Name, "", result.BasicProperties, result.Body);
            _channel.BasicAck(result.DeliveryTag, false);
        }

        public void MessageSkipped(BasicDeliverEventArgs result)
        {
            _channel.BasicNack(result.DeliveryTag, false, true);
        }
    }
}