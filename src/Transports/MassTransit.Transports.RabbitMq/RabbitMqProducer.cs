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
    using Management;
    using PublisherConfirm;
    using RabbitMQ.Client;
    using RabbitMQ.Client.Events;

    public class RabbitMqProducer : ConnectionBinding<RabbitMqConnection>
    {
        readonly IPublisherConfirmSettings _publisherConfirmSettings;
        readonly IRabbitMqEndpointAddress _address;
        readonly bool _bindToQueue;
        IModel _channel;

        public RabbitMqProducer(IRabbitMqEndpointAddress address, IPublisherConfirmSettings publisherConfirmSettings, bool bindToQueue)
        {
            _address = address;
            _bindToQueue = bindToQueue;
            _publisherConfirmSettings = publisherConfirmSettings;
        }

        public IBasicProperties CreateProperties()
        {
            if (_channel == null)
                throw new InvalidConnectionException(_address.Uri, "Channel should not be null");

            return _channel.CreateBasicProperties();
        }

        public void Publish(string exchangeName, IBasicProperties properties, byte[] body)
        {
            if (_channel == null)
                throw new InvalidConnectionException(_address.Uri, "Channel should not be null");

            if (_publisherConfirmSettings.UsePublisherConfirms)
            {
                _publisherConfirmSettings.RegisterMessageAction(_channel.NextPublishSeqNo, properties.CorrelationId);
            }

            _channel.BasicPublish(exchangeName, "", properties, body);
        }

        public void Bind(RabbitMqConnection connection)
        {
            _channel = connection.Connection.CreateModel();

            if (_publisherConfirmSettings.UsePublisherConfirms)
            {
                _channel.ConfirmSelect();
                _channel.BasicAcks += OnBasicAcks;
                _channel.BasicNacks += OnBasicNacks;
            }

            _channel.ExchangeDeclare(_address.Name, ExchangeType.Fanout, true);

            if (_bindToQueue)
            {
                using (var management = new RabbitMqEndpointManagement(_address, connection.Connection))
                {
                    management.BindQueue(_address.Name, _address.Name, ExchangeType.Fanout, "",
                        _address.QueueArguments());
                }
            }
        }

        public void Unbind(RabbitMqConnection connection)
        {
            if (_channel != null)
            {
                if (_channel.IsOpen)
                    _channel.Close(200, "producer unbind");

                if(_publisherConfirmSettings.UsePublisherConfirms)
                {
                    _channel.BasicAcks -= OnBasicAcks;
                    _channel.BasicNacks -= OnBasicNacks;
                }
                _channel.Dispose();
                _channel = null;
            }
        }

        private void OnBasicNacks(IModel model, BasicNackEventArgs args)
        {
            _publisherConfirmSettings.Acktion(args.DeliveryTag, args.Multiple);

        }

        private void OnBasicAcks(IModel model, BasicAckEventArgs args)
        {

            _publisherConfirmSettings.Acktion(args.DeliveryTag, args.Multiple);
        }
    }
}