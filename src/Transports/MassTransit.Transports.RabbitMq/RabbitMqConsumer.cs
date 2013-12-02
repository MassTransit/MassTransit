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
    using System.Collections.Generic;
    using RabbitMQ.Client;
    using RabbitMQ.Client.Events;
    using RabbitMQ.Util;


    public class RabbitMqConsumer :
        ConnectionBinding<RabbitMqConnection>
    {
        readonly IRabbitMqEndpointAddress _address;
        readonly object _lock = new object();
        IModel _channel;
        QueueingBasicConsumer _consumer;
        bool _purgeOnBind;
        readonly string _consumerTag;

        public RabbitMqConsumer(IRabbitMqEndpointAddress address, bool purgeOnBind)
        {
            _address = address;
            _purgeOnBind = purgeOnBind;
            _consumerTag = NewId.NextGuid().ToString();
        }

        public void Bind(RabbitMqConnection connection)
        {
            IModel channel = null;
            try
            {
                channel = connection.Connection.CreateModel();

                BindQueue(connection, channel);

                PurgeIfRequested(channel);

                channel.BasicQos(0, _address.PrefetchCount, false);

                var consumer = new QueueingBasicConsumer(channel);
                channel.BasicConsume(_address.Name, false, _consumerTag, consumer);

                lock (_lock)
                {
                    _channel = channel;
                    _consumer = consumer;
                }
            }
            catch (Exception ex)
            {
                channel.Cleanup(500, ex.Message);

                throw new InvalidConnectionException(_address.Uri, "Invalid connection to host", ex);
            }
        }

        public void Unbind(RabbitMqConnection connection)
        {
            lock (_lock)
            {
                _consumer = null;

                _channel.Cleanup(200, "Unbind Consumer");
                _channel = null;
            }
        }

        void PurgeIfRequested(IModel channel)
        {
            if (_purgeOnBind)
            {
                channel.QueuePurge(_address.Name);
                _purgeOnBind = false;
            }
        }

        void BindQueue(RabbitMqConnection connection, IModel channel)
        {
            connection.DeclareExchange(channel, _address.Name, _address.Durable, _address.AutoDelete);

            connection.BindQueue(channel, _address.Name, _address.Durable, _address.Exclusive, _address.AutoDelete, _address.QueueArguments());
        }

        public BasicDeliverEventArgs Get(TimeSpan timeout)
        {
            SharedQueue<BasicDeliverEventArgs> queue;
            lock (_lock)
            {
                if (_consumer == null)
                    throw new InvalidConnectionException(_address.Uri, "No connection to RabbitMQ Host");

                if (!_consumer.IsRunning)
                    throw new InvalidConnectionException(_address.Uri, "Consumer is not running");

                queue = _consumer.Queue;
            }

            BasicDeliverEventArgs result;
            queue.Dequeue((int)timeout.TotalMilliseconds, out result);

            return result;
        }

        public void MessageCompleted(BasicDeliverEventArgs result)
        {
            lock (_lock)
            {
                if(_channel == null)
                    throw new InvalidConnectionException(_address.Uri, "No connection to RabbitMQ Host");

                _channel.BasicAck(result.DeliveryTag, false);
                
            }
        }

        public void MessageFailed(BasicDeliverEventArgs result)
        {
            lock (_lock)
            {
                if (_channel == null)
                    throw new InvalidConnectionException(_address.Uri, "No connection to RabbitMQ Host");

                _channel.BasicPublish(_address.Name, "", result.BasicProperties, result.Body);
                _channel.BasicAck(result.DeliveryTag, false);
            }
        }

        public void MessageSkipped(BasicDeliverEventArgs result)
        {
            lock (_lock)
            {
                if (_channel == null)
                    throw new InvalidConnectionException(_address.Uri, "No connection to RabbitMQ Host");

                _channel.BasicNack(result.DeliveryTag, false, true);
            }
        }
    }
}