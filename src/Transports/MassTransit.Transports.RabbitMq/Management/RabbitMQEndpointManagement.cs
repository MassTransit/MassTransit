// Copyright 2007-2011 Chris Patterson, Dru Sellers, Travis Smith, et. al.
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
namespace MassTransit.Transports.RabbitMq.Management
{
    using System.Collections;
    using System.Collections.Generic;
    using RabbitMQ.Client;

    public class RabbitMqEndpointManagement :
        IRabbitMqEndpointManagement
    {
        readonly bool _owned;
        IConnection _connection;
        bool _disposed;

        public RabbitMqEndpointManagement(IRabbitMqEndpointAddress address)
            : this(address, address.ConnectionFactory.CreateConnection())
        {
            _owned = true;
        }

        public RabbitMqEndpointManagement(IRabbitMqEndpointAddress address, IConnection connection)
        {
            _connection = connection;
        }

        public void BindQueue(string queueName, string exchangeName, string exchangeType, string routingKey,
                              IDictionary<string,object> queueArguments)
        {
            using (IModel model = _connection.CreateModel())
            {
                string queue = model.QueueDeclare(queueName, true, false, false, queueArguments);
                model.ExchangeDeclare(exchangeName, exchangeType, true);

                model.QueueBind(queue, exchangeName, routingKey);

                model.Close(200, "ok");
            }
        }

        public void UnbindQueue(string queueName, string exchangeName, string routingKey)
        {
            using (IModel model = _connection.CreateModel())
            {
                model.QueueUnbind(queueName, exchangeName, routingKey, null);

                model.Close(200, "ok");
            }
        }

        public void BindExchange(string destination, string source, string exchangeType, string routingKey)
        {
            using (IModel model = _connection.CreateModel())
            {
                model.ExchangeDeclare(destination, exchangeType, true, false, null);
                model.ExchangeDeclare(source, exchangeType, true, false, null);

                model.ExchangeBind(destination, source, routingKey);

                model.Close(200, "ok");
            }
        }

        public void UnbindExchange(string destination, string source, string routingKey)
        {
            using (IModel model = _connection.CreateModel())
            {
                model.ExchangeUnbind(destination, source, routingKey, null);

                model.Close(200, "ok");
            }
        }

        public void Purge(string queueName)
        {
            using (IModel model = _connection.CreateModel())
            {
                try
                {
                    model.QueueDeclarePassive(queueName);
                    model.QueuePurge(queueName);
                }
                catch
                {
                }

                model.Close(200, "purged queue");
            }
        }

        public void Dispose()
        {
            if (_disposed)
                return;

            if (_owned)
                _connection.Cleanup();
            _connection = null;

            _disposed = true;
        }
    }
}