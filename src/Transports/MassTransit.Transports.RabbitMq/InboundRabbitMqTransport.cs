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
    using System.IO;
    using System.Linq;
    using System.Text;
    using Context;
    using Logging;
    using RabbitMQ.Client.Events;
    using RabbitMQ.Client.Exceptions;

    public class InboundRabbitMqTransport :
        IInboundTransport
    {
        static readonly ILog _log = Logger.Get(typeof(InboundRabbitMqTransport));

        readonly IRabbitMqEndpointAddress _address;
        readonly ConnectionHandler<RabbitMqConnection> _connectionHandler;
        readonly IMessageNameFormatter _messageNameFormatter;
        readonly bool _purgeExistingMessages;
        RabbitMqConsumer _consumer;
        bool _disposed;
        RabbitMqPublisher _publisher;

        public InboundRabbitMqTransport(IRabbitMqEndpointAddress address,
            ConnectionHandler<RabbitMqConnection> connectionHandler,
            bool purgeExistingMessages,
            IMessageNameFormatter messageNameFormatter)
        {
            _address = address;
            _connectionHandler = connectionHandler;
            _purgeExistingMessages = purgeExistingMessages;
            _messageNameFormatter = messageNameFormatter;
        }

        public IMessageNameFormatter MessageNameFormatter
        {
            get { return _messageNameFormatter; }
        }

        public IEndpointAddress Address
        {
            get { return _address; }
        }

        public void Receive(Func<IReceiveContext, Action<IReceiveContext>> lookupSinkChain, TimeSpan timeout)
        {
            AddConsumerBinding();

            _connectionHandler.Use(connection =>
                {
                    BasicDeliverEventArgs result = null;
                    try
                    {
                        result = _consumer.Get(timeout);
                        if (result == null)
                            return;

                        using (var body = new MemoryStream(result.Body, false))
                        {
                            ReceiveContext context = ReceiveContext.FromBodyStream(body, true);
                            context.SetMessageId(result.BasicProperties.MessageId ?? result.DeliveryTag.ToString());
                            result.BasicProperties.MessageId = context.MessageId;
                            context.SetInputAddress(_address);

                            if (result.BasicProperties.IsHeadersPresent())
                            {
                                object value;
                                if (result.BasicProperties.Headers.TryGetValue("Content-Type", out value))
                                {
                                    var contentType = value as byte[];
                                    if (contentType != null)
                                    {
                                        context.SetContentType(Encoding.UTF8.GetString(contentType));
                                    }
                                }
                            }

                            Action<IReceiveContext> receive = lookupSinkChain(context);
                            if (receive == null)
                            {
                                Address.LogSkipped(result.BasicProperties.MessageId);

                                _consumer.MessageSkipped(result);
                            }
                            else
                            {
                                receive(context);

                                _consumer.MessageCompleted(result);
                            }
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
                    catch (Exception ex)
                    {
                        _log.Error("Failed to consume message from endpoint", ex);

                        if (result != null)
                            _consumer.MessageFailed(result);

                        throw;
                    }
                });
        }

        public void Dispose()
        {
            Dispose(true);
        }

        public IEnumerable<Type> BindExchangesForPublisher(Type messageType, IMessageNameFormatter messageNameFormatter)
        {
            AddPublisherBinding();

            IList<Type> messageTypes = new List<Type>();
            _connectionHandler.Use(connection =>
                {
                    MessageName messageName = messageNameFormatter.GetMessageName(messageType);

                    bool temporary = IsTemporaryMessageType(messageType);

                    _publisher.ExchangeDeclare(messageName.ToString(), temporary);

                    messageTypes.Add(messageType);

                    foreach (Type type in messageType.GetMessageTypes().Skip(1))
                    {
                        MessageName interfaceName = messageNameFormatter.GetMessageName(type);

                        bool isTemporary = IsTemporaryMessageType(type);

                        _publisher.ExchangeBind(interfaceName.ToString(), messageName.ToString(), isTemporary, temporary);
                        messageTypes.Add(type);
                    }
                });

            return messageTypes;
        }

        static bool IsTemporaryMessageType(Type messageType)
        {
            return (!messageType.IsPublic && messageType.IsClass)
                   || (messageType.IsGenericType
                       && messageType.GetGenericArguments().Any(x => IsTemporaryMessageType(x)));
        }

        public void BindSubscriberExchange(IRabbitMqEndpointAddress address, string exchangeName, bool temporary)
        {
            AddPublisherBinding();
            _connectionHandler.Use(connection =>
                {
                    _publisher.ExchangeBind(address.Name, exchangeName, false, temporary);
                });
        }

        public void UnbindSubscriberExchange(string exchangeName)
        {
            AddPublisherBinding();
            _connectionHandler.Use(connection =>
            {
                _publisher.ExchangeUnbind(_address.Name, exchangeName);
            });            
        }

        void AddConsumerBinding()
        {
            if (_consumer != null)
                return;

            _consumer = new RabbitMqConsumer(_address, _purgeExistingMessages);

            _connectionHandler.AddBinding(_consumer);
        }

        void AddPublisherBinding()
        {
            if (_publisher != null)
                return;

            _publisher = new RabbitMqPublisher(_address);

            _connectionHandler.AddBinding(_publisher);
        }

        void RemoveConsumerBinding()
        {
            if (_consumer != null)
                _connectionHandler.RemoveBinding(_consumer);
        }

        void RemovePublisherBinding()
        {
            if (_publisher != null)
                _connectionHandler.RemoveBinding(_publisher);
        }

        void Dispose(bool disposing)
        {
            if (_disposed)
                return;
            if (disposing)
            {
                RemoveConsumerBinding();
                RemovePublisherBinding();
            }

            _disposed = true;
        }
    }
}