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

using MassTransit.Util;

namespace MassTransit.Transports
{
    using System;
    using System.Diagnostics;
    using System.Runtime.Serialization;
    using Context;
    using Exceptions;
    using Logging;
    using Serialization;

	/// <summary>
	/// See <see cref="IEndpoint"/> for docs.
	/// </summary>
    [DebuggerDisplay("{Address}")]
    public class Endpoint :
        IEndpoint
    {
        static readonly ILog _log = Logger.Get(typeof (Endpoint));
        readonly IEndpointAddress _address;
        readonly IMessageSerializer _serializer;
        readonly MessageRetryTracker _tracker;
        bool _disposed;
        string _disposedMessage;
        IOutboundTransport _errorTransport;
        IDuplexTransport _transport;

        public Endpoint([NotNull] IEndpointAddress address,
			[NotNull] IMessageSerializer serializer,
			[NotNull] IDuplexTransport transport,
			[NotNull] IOutboundTransport errorTransport)
        {
        	if (address == null) throw new ArgumentNullException("address");
        	if (serializer == null) throw new ArgumentNullException("serializer");
        	if (transport == null) throw new ArgumentNullException("transport");
        	if (errorTransport == null) throw new ArgumentNullException("errorTransport");
        	_address = address;
            _transport = transport;
            _errorTransport = errorTransport;
            _serializer = serializer;

            _tracker = new MessageRetryTracker(5);

            SetDisposedMessage();
        }

        public IOutboundTransport ErrorTransport
        {
            get { return _errorTransport; }
        }

        public IMessageSerializer Serializer
        {
            get { return _serializer; }
        }

        public IEndpointAddress Address
        {
            get { return _address; }
        }

        public IInboundTransport InboundTransport
        {
            get { return _transport.InboundTransport; }
        }

        public IOutboundTransport OutboundTransport
        {
            get { return _transport.OutboundTransport; }
        }

        public void Send<T>(ISendContext<T> context)
            where T : class
        {
            if (_disposed)
                throw new ObjectDisposedException(_disposedMessage);

            try
            {
                context.SetDestinationAddress(Address.Uri);
                context.SetBodyWriter(stream => _serializer.Serialize(stream, context));

                _transport.Send(context);

                context.NotifySend(_address);
            }
            catch (Exception ex)
            {
                throw new SendException(typeof (T), _address.Uri, "An exception was thrown during Send", ex);
            }
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        public void Receive(Func<IReceiveContext, Action<IReceiveContext>> receiver, TimeSpan timeout)
        {
            if (_disposed)
                throw new ObjectDisposedException(_disposedMessage);

            string successfulMessageId = null;

            try
            {
                Exception failedMessageException = null;

                _transport.Receive(acceptContext =>
                    {
                        failedMessageException = null;

                        if (successfulMessageId != null)
                        {
                            _log.DebugFormat("Received Successfully: {0}", successfulMessageId);

                            _tracker.MessageWasReceivedSuccessfully(successfulMessageId);
                            successfulMessageId = null;
                        }

                        Exception retryException;
                        if (_tracker.IsRetryLimitExceeded(acceptContext.MessageId, out retryException))
                        {
                            if (_log.IsErrorEnabled)
                                _log.ErrorFormat("Message retry limit exceeded {0}:{1}", Address,
                                    acceptContext.MessageId);

                            failedMessageException = retryException;

                            return MoveMessageToErrorTransport;
                        }

                        Action<IReceiveContext> receive;
                        try
                        {
                            acceptContext.SetEndpoint(this);
                            _serializer.Deserialize(acceptContext);

                            receive = receiver(acceptContext);
                            if (receive == null)
                            {
                                Address.LogSkipped(acceptContext.MessageId);

                                _tracker.IncrementRetryCount(acceptContext.MessageId, null);
                                return null;
                            }
                        }
                        catch (SerializationException sex)
                        {
                            if (_log.IsErrorEnabled)
                                _log.Error("Unrecognized message " + Address + ":" + acceptContext.MessageId, sex);

                            _tracker.IncrementRetryCount(acceptContext.MessageId, sex);
                            return MoveMessageToErrorTransport;
                        }
                        catch (Exception ex)
                        {
                            if (_log.IsErrorEnabled)
                                _log.Error("An exception was thrown preparing the message consumers", ex);

                            _tracker.IncrementRetryCount(acceptContext.MessageId, ex);
                            return null;
                        }

                        return receiveContext =>
                            {
                                bool receivedSuccessfully;

                                try
                                {
                                    receive(receiveContext);

                                    //                                    _tracker.MessageWasReceivedSuccessfully(receiveContext.MessageId);
                                    receivedSuccessfully = true;
                                }
                                catch (MessageNotConsumedException ex)
                                {
                                    receivedSuccessfully = false;

                                    _tracker.MessageWasReceivedSuccessfully(receiveContext.MessageId);
                                    MoveMessageToErrorTransport(receiveContext);
                                }
                                catch (Exception ex)
                                {
                                    receivedSuccessfully = false;

                                    if (_log.IsErrorEnabled)
                                        _log.Error("An exception was thrown by a message consumer", ex);

                                    _tracker.IncrementRetryCount(receiveContext.MessageId, ex);
                                    MoveMessageToErrorTransport(receiveContext);
                                }

                                if (receivedSuccessfully)
                                {
                                    successfulMessageId = receiveContext.MessageId;
                                }
                            };
                    }, timeout);

                if (failedMessageException != null)
                {
                    _log.DebugFormat("Throwing Original Exception: {0}", failedMessageException.GetType());

                    throw failedMessageException;
                }
            }
            catch (Exception ex)
            {
                if (successfulMessageId != null)
                {
                    _log.DebugFormat("Increment Retry Count: {0}", successfulMessageId);

                    _tracker.IncrementRetryCount(successfulMessageId, ex);
                    successfulMessageId = null;
                }
                throw;
            }
            finally
            {
                if (successfulMessageId != null)
                {
                    _log.DebugFormat("Received Successfully: {0}", successfulMessageId);

                    _tracker.MessageWasReceivedSuccessfully(successfulMessageId);
                    successfulMessageId = null;
                }
            }
        }

        void SetDisposedMessage()
        {
            _disposedMessage = "The endpoint has already been disposed: " + _address;
        }

        void Dispose(bool disposing)
        {
            if (_disposed) return;
            if (disposing)
            {
                _transport.Dispose();
                _transport = null;

                _errorTransport.Dispose();
                _errorTransport = null;
            }

            _disposed = true;
        }

        void MoveMessageToErrorTransport(IReceiveContext context)
        {
            var moveContext = new MoveMessageSendContext(context);

            _errorTransport.Send(moveContext);

            Address.LogMoved(_errorTransport.Address, context.MessageId, "");
        }

        ~Endpoint()
        {
            Dispose(false);
        }
    }
}