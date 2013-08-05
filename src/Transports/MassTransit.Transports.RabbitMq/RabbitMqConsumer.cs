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
            IModel channel = null;
            try
            {
                channel = connection.Connection.CreateModel();

                BindQueue(channel);

                PurgeIfRequested(channel);

                channel.BasicQos(0, _address.PrefetchCount, false);

                var consumer = new QueueingBasicConsumer(channel);
                channel.BasicConsume(_address.Name, false, consumer);

                _channel = channel;
                _consumer = consumer;
            }
            catch (Exception ex)
            {
                if (channel != null)
                {
                    try
                    {
                        channel.Close(500, ex.Message);
                    }
                    catch
                    {
                    }
                    channel.Dispose();
                }

                throw new InvalidConnectionException(_address.Uri, "Invalid connection to host", ex);
            }
        }

        public void Unbind(RabbitMqConnection connection)
        {
            if (_channel != null)
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
        }

        void PurgeIfRequested(IModel channel)
        {
            if (_purgeOnBind)
            {
                channel.QueuePurge(_address.Name);
                _purgeOnBind = false;
            }
        }

        void BindQueue(IModel channel)
        {
            string queue = channel.QueueDeclare(_address.Name, _address.Durable, _address.Exclusive, _address.AutoDelete, _address.QueueArguments());
            channel.ExchangeDeclare(_address.Name, ExchangeType.Fanout, _address.Durable, _address.AutoDelete, null);
            channel.QueueBind(queue, _address.Name, "");
        }

        public BasicDeliverEventArgs Get(TimeSpan timeout)
        {
            if (_consumer == null)
                throw new InvalidConnectionException(_address.Uri, "No connection to RabbitMQ Host");

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