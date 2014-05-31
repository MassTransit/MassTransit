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
namespace MassTransit.Transports.Msmq
{
    using System;
    using System.Messaging;
    using Context;
    using Logging;
    using Magnum;

    public abstract class OutboundMsmqTransport :
        IOutboundTransport
    {
        static readonly ILog _log = Logger.Get(typeof (OutboundMsmqTransport));
        readonly IMsmqEndpointAddress _address;
        readonly ConnectionHandler<MessageQueueConnection> _connectionHandler;
        bool _disposed;

        protected OutboundMsmqTransport(IMsmqEndpointAddress address,
                                        ConnectionHandler<MessageQueueConnection> connectionHandler)
        {
            _address = address;
            _connectionHandler = connectionHandler;
        }

        public IEndpointAddress Address
        {
            get { return _address; }
        }

        public void Send(ISendContext context)
        {
            using (var message = new Message())
            {
                if (!string.IsNullOrEmpty(context.MessageType))
                    message.Label = context.MessageType.Length > 249 ? context.MessageType.Substring(0, 249) : context.MessageType;

                message.Recoverable = _address.IsRecoverable && context.DeliveryMode == DeliveryMode.Persistent;
                message.UseDeadLetterQueue = true; // in case lack of permission message will be redirected to dead letter

                if (context.ExpirationTime.HasValue)
                {
                    DateTime value = context.ExpirationTime.Value;
                    message.TimeToBeReceived = value.Kind == DateTimeKind.Utc ? value - SystemUtil.UtcNow : value - SystemUtil.Now;
                }

                context.SerializeTo(message.BodyStream);

                var headers = new TransportMessageHeaders();

                if (!string.IsNullOrEmpty(context.ContentType))
                    headers.Add("Content-Type", context.ContentType);
                if (!string.IsNullOrEmpty(context.OriginalMessageId))
                    headers.Add("Original-Message-Id", context.OriginalMessageId);
                
                message.Extension = headers.GetBytes();

                try
                {
                    _connectionHandler.Use(connection => SendMessage(connection.Queue, message));

                    _address.LogSent(message.Id, context.MessageType);

                }
                catch (MessageQueueException ex)
                {
                    HandleOutboundMessageQueueException(ex);
                }
            }
        }

        public void Dispose()
        {
            Dispose(true);
        }

        protected virtual void SendMessage(MessageQueue queue, Message message)
        {
            queue.Send(message, MessageQueueTransactionType.None);
        }

        void Dispose(bool disposing)
        {
            if (_disposed) return;
            if (disposing)
            {
                _connectionHandler.Dispose();
            }

            _disposed = true;
        }

        void HandleOutboundMessageQueueException(MessageQueueException ex)
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
                    throw new InvalidConnectionException(_address.Uri, "The message queue was not found or is improperly named", ex);

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
                        "The message queue handle is stale or no longer valid due to a restart of the message queuing service", ex);


                default:
                    throw new InvalidConnectionException(_address.Uri, "There was a problem communicating with the message queue", ex);
            }
        }
    }
}