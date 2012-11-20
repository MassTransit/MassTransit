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
    using System.Collections;
    using System.IO;
    using Magnum;
    using PublisherConfirm;
    using RabbitMQ.Client;
    using RabbitMQ.Client.Exceptions;

    public class OutboundRabbitMqTransport :
        IOutboundTransport
    {
        readonly IRabbitMqEndpointAddress _address;
        readonly bool _bindToQueue;
        readonly ConnectionHandler<RabbitMqConnection> _connectionHandler;
        readonly IPublisherConfirmSettings _publisherConfirmSettings;
        RabbitMqProducer _producer;

        public OutboundRabbitMqTransport(IRabbitMqEndpointAddress address,
            ConnectionHandler<RabbitMqConnection> connectionHandler, 
            IPublisherConfirmSettings publisherConfirmSettings,
            bool bindToQueue)
        {
            _address = address;
            _connectionHandler = connectionHandler;
            _bindToQueue = bindToQueue;
            _publisherConfirmSettings = publisherConfirmSettings;
        }

        public IEndpointAddress Address
        {
            get { return _address; }
        }

        public void Send(ISendContext context)
        {
            AddProducerBinding();

            _connectionHandler.Use(connection =>
                {
                    try
                    {
                        IBasicProperties properties = _producer.CreateProperties();

                        properties.SetPersistent(true);
                        properties.MessageId = context.MessageId ?? properties.MessageId ?? NewId.Next().ToString();

                        if (context.ExpirationTime.HasValue)
                        {
                            DateTime value = context.ExpirationTime.Value;
                            properties.Expiration = (value.Kind == DateTimeKind.Utc ? value - SystemUtil.UtcNow : value - SystemUtil.Now).ToString();
                        }

                        properties.Headers = new Hashtable { { "Content-Type", context.ContentType } };

                        if (context.Headers[PublisherConfirmSettings.ClientMessageId] != null)
                        {
                            properties.Headers.Add(PublisherConfirmSettings.ClientMessageId, context.Headers[PublisherConfirmSettings.ClientMessageId]);
                        }

                        using (var body = new MemoryStream())
                        {
                            context.SerializeTo(body);
                            
                            _producer.Publish(_address.Name, properties, body.ToArray());

                            _address.LogSent(context.MessageId ?? "", context.MessageType);
                        }
                    }
                    catch (AlreadyClosedException ex)
                    {
                        throw new InvalidConnectionException(_address.Uri, "Connection was already closed", ex);
                    }
                    catch (EndOfStreamException ex)
                    {
                        throw new InvalidConnectionException(_address.Uri, "Connection was closed", ex);
                    }
                    catch (OperationInterruptedException ex)
                    {
                        throw new InvalidConnectionException(_address.Uri, "Operation was interrupted", ex);
                    }
                });
        }

        public void Dispose()
        {
            RemoveProducer();
        }

        void AddProducerBinding()
        {
            if (_producer != null)
                return;

            _producer = new RabbitMqProducer(_address, _publisherConfirmSettings, _bindToQueue);

            _connectionHandler.AddBinding(_producer);
        }

        void RemoveProducer()
        {
            if (_producer != null)
            {
                _connectionHandler.RemoveBinding(_producer);
            }
        }
    }
}