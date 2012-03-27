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
namespace MassTransit.Transports.RabbitMq
{
    using System;
    using Logging;
    using Management;
    using RabbitMQ.Client;

    public class RabbitMqConsumer :
        ConnectionBinding<RabbitMqConnection>
    {
        static readonly ILog _log = Logger.Get(typeof (RabbitMqConsumer));
        readonly IRabbitMqEndpointAddress _address;
        IModel _channel;
        bool _purgeOnBind;

        public RabbitMqConsumer(IRabbitMqEndpointAddress address, bool purgeOnBind)
        {
            _address = address;
            _purgeOnBind = purgeOnBind;
        }

        public void Bind(RabbitMqConnection connection)
        {
            using (var management = new RabbitMqEndpointManagement(_address, connection.Connection))
            {
                management.BindQueue(_address.Name, _address.Name, ExchangeType.Fanout, "", _address.QueueArguments());

                if (_purgeOnBind)
                {
                    management.Purge(_address.Name);
                    _purgeOnBind = false;
                }
            }

            _channel = connection.Connection.CreateModel();
            _channel.BasicQos(0, 1, false);
        }

        public void Unbind(RabbitMqConnection connection)
        {
            try
            {
                _channel.Close(200, "unbind consumer");
            }
            catch (Exception ex)
            {
                _log.Error("Failed to close channel: " + _address, ex);
            }
            finally
            {
                _channel.Dispose();
                _channel = null;
            }
        }

        public BasicGetResult Get()
        {
            return _channel.BasicGet(_address.Name, false);
        }

        public void MessageCompleted(BasicGetResult result)
        {
            _channel.BasicAck(result.DeliveryTag, false);
        }

        public void MessageFailed(BasicGetResult result)
        {
            _channel.BasicPublish(_address.Name, "", result.BasicProperties, result.Body);
            _channel.BasicAck(result.DeliveryTag, false);
        }

        public void MessageSkipped(BasicGetResult result)
        {
            _channel.BasicNack(result.DeliveryTag, false, true);
        }
    }
}