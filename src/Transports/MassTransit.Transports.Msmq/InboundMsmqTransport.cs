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
namespace MassTransit.Transports.Msmq
{
    using System;
    using System.Messaging;
    using Context;
    using Exceptions;
    using Logging;
    using Magnum.Extensions;

    public abstract class InboundMsmqTransport :
        IInboundTransport
    {
        static readonly ILog _log = Logger.Get(typeof(InboundMsmqTransport));
        readonly IMsmqEndpointAddress _address;
        readonly ConnectionHandler<MessageQueueConnection> _connectionHandler;
        bool _disposed;

        protected InboundMsmqTransport(IMsmqEndpointAddress address,
            ConnectionHandler<MessageQueueConnection> connectionHandler)
        {
            _address = address;
            _connectionHandler = connectionHandler;
        }

        public IEndpointAddress Address
        {
            get { return _address; }
        }

        public virtual void Receive(Func<IReceiveContext, Action<IReceiveContext>> callback, TimeSpan timeout)
        {
            EnumerateQueue(callback, timeout);
        }

        public void Dispose()
        {
            Dispose(true);
        }

        protected void EnumerateQueue(Func<IReceiveContext, Action<IReceiveContext>> receiver, TimeSpan timeout)
        {
            if (_disposed)
                throw new ObjectDisposedException("The transport has been disposed: '{0}'".FormatWith(Address));

            _connectionHandler.Use(connection =>
                {
                    try
                    {
                        using (MessageEnumerator enumerator = connection.Queue.GetMessageEnumerator2())
                        {
                            while (enumerator.MoveNext(timeout))
                            {
                                if (enumerator.Current == null)
                                {
                                    if (_log.IsDebugEnabled)
                                        _log.DebugFormat("Current message was null while enumerating endpoint");

                                    continue;
                                }

                                Message peekMessage = enumerator.Current;
                                using (peekMessage)
                                {
                                    IReceiveContext context = ReceiveContext.FromBodyStream(peekMessage.BodyStream,
                                        _address.IsTransactional);
                                    context.SetMessageId(peekMessage.Id);
                                    context.SetInputAddress(_address);

                                    byte[] extension = peekMessage.Extension;
                                    if (extension.Length > 0)
                                    {
                                        TransportMessageHeaders headers = TransportMessageHeaders.Create(extension);

                                        context.SetContentType(headers["Content-Type"]);
                                        context.SetOriginalMessageId(headers["Original-Message-Id"]);
                                    }

                                    Action<IReceiveContext> receive = receiver(context);
                                    if (receive == null)
                                    {
                                        continue;
                                    }

                                    ReceiveMessage(enumerator, timeout, message =>
                                        {
                                            if (message == null)
                                                throw new TransportException(Address.Uri,
                                                    "Unable to remove message from queue: " + context.MessageId);

                                            if (message.Id != context.MessageId)
                                                throw new TransportException(Address.Uri,
                                                    string.Format(
                                                        "Received message does not match current message: ({0} != {1})",
                                                        message.Id, context.MessageId));

                                            receive(context);
                                        });
                                }
                            }
                        }
                    }
                    catch (MessageQueueException ex)
                    {
                        HandleInboundMessageQueueException(ex);
                    }
                });
        }

        protected virtual void ReceiveMessage(MessageEnumerator enumerator, TimeSpan timeout,
            Action<Message> receiveAction)
        {
            using (Message message = enumerator.RemoveCurrent(timeout, MessageQueueTransactionType.None))
            {
                receiveAction(message);
            }
        }

        protected void HandleInboundMessageQueueException(MessageQueueException ex)
        {
            switch (ex.MessageQueueErrorCode)
            {
                case MessageQueueErrorCode.IOTimeout:
                    break;

                case MessageQueueErrorCode.ServiceNotAvailable:
                    throw new InvalidConnectionException(_address.Uri,
                        "The message queuing service is not available, pausing for timeout period", ex);

                case MessageQueueErrorCode.QueueNotAvailable:
                case MessageQueueErrorCode.AccessDenied:
                case MessageQueueErrorCode.QueueDeleted:
                    throw new InvalidConnectionException(_address.Uri, "The message queue was not available", ex);

                case MessageQueueErrorCode.QueueNotFound:
                case MessageQueueErrorCode.IllegalFormatName:
                case MessageQueueErrorCode.MachineNotFound:
                    throw new InvalidConnectionException(_address.Uri,
                        "The message queue was not found or is improperly named", ex);

                case MessageQueueErrorCode.MessageAlreadyReceived:
                    // we are competing with another consumer, no reason to report an error since
                    // the message has already been handled.
                    if (_log.IsDebugEnabled)
                        _log.Debug(
                            "The message was removed from the queue before it could be received. This could be the result of another service reading from the same queue.");
                    break;

                case MessageQueueErrorCode.InvalidHandle:
                case MessageQueueErrorCode.StaleHandle:
                    throw new InvalidConnectionException(_address.Uri,
                        "The message queue handle is stale or no longer valid due to a restart of the message queuing service",
                        ex);


                default:
                    throw new InvalidConnectionException(_address.Uri,
                        "There was a problem communicating with the message queue", ex);
            }
        }

        void Dispose(bool disposing)
        {
            if (_disposed)
                return;
            if (disposing)
            {
                _connectionHandler.Dispose();
            }

            _disposed = true;
        }
    }
}