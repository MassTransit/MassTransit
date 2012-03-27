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
    using System.IO;
    using System.Text;
    using System.Threading;
    using Context;
    using Logging;
    using RabbitMQ.Client;
    using RabbitMQ.Client.Exceptions;

    public class InboundRabbitMqTransport :
        IInboundTransport
    {
        static readonly ILog _log = Logger.Get(typeof (InboundRabbitMqTransport));

        readonly IRabbitMqEndpointAddress _address;
        readonly ConnectionHandler<RabbitMqConnection> _connectionHandler;
        readonly IMessageNameFormatter _messageNameFormatter;
        readonly bool _purgeExistingMessages;
        RabbitMqConsumer _consumer;
        bool _disposed;

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
                    BasicGetResult result = null;
                    try
                    {
                        result = _consumer.Get();
                        if (result == null)
                        {
                            Thread.Sleep(10);
                            return;
                        }

                        using (var body = new MemoryStream(result.Body, false))
                        {
                            ReceiveContext context = ReceiveContext.FromBodyStream(body);
                            context.SetMessageId(result.BasicProperties.MessageId ?? result.DeliveryTag.ToString());
                            result.BasicProperties.MessageId = context.MessageId;
                            context.SetInputAddress(_address);

                            byte[] contentType = result.BasicProperties.IsHeadersPresent()
                                                     ? (byte[]) result.BasicProperties.Headers["Content-Type"]
                                                     : null;
                            if (contentType != null)
                            {
                                context.SetContentType(Encoding.UTF8.GetString(contentType));
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
            GC.SuppressFinalize(this);
        }

        void AddConsumerBinding()
        {
            if (_consumer != null)
                return;

            _consumer = new RabbitMqConsumer(_address, _purgeExistingMessages);

            _connectionHandler.AddBinding(_consumer);
        }

        void RemoveConsumer()
        {
            if (_consumer != null)
            {
                _connectionHandler.RemoveBinding(_consumer);
            }
        }

        void Dispose(bool disposing)
        {
            if (_disposed) return;
            if (disposing)
            {
                RemoveConsumer();
            }

            _disposed = true;
        }

        ~InboundRabbitMqTransport()
        {
            Dispose(false);
        }
    }
}