// Copyright 2007-2014 Chris Patterson, Dru Sellers, Travis Smith, et. al.
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
namespace MassTransit.Transports
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Reflection;
    using System.Runtime.Serialization;
    using Context;
    using Exceptions;
    using Logging;
    using Magnum.Reflection;
    using Serialization;
    using Util;


    /// <summary>
    /// See <see cref="IEndpoint"/> for docs.
    /// </summary>
    [DebuggerDisplay("{Address}")]
    public class Endpoint :
        IEndpoint
    {
        static readonly ILog _log = Logger.Get(typeof(Endpoint));
        readonly IEndpointAddress _address;
        readonly IMessageSerializer _serializer;
        readonly ISupportedMessageSerializers _supportedSerializers;
        readonly IInboundMessageTracker _tracker;
        bool _disposed;
        string _disposedMessage;
        IOutboundTransport _errorTransport;
        IDuplexTransport _transport;

        public Endpoint([NotNull] IEndpointAddress address,
            [NotNull] IMessageSerializer serializer,
            [NotNull] IDuplexTransport transport,
            [NotNull] IOutboundTransport errorTransport,
            [NotNull] IInboundMessageTracker messageTracker,
            [NotNull] ISupportedMessageSerializers supportedSerializers)
        {
            if (address == null)
                throw new ArgumentNullException("address");
            if (serializer == null)
                throw new ArgumentNullException("serializer");
            if (transport == null)
                throw new ArgumentNullException("transport");
            if (errorTransport == null)
                throw new ArgumentNullException("errorTransport");
            if (messageTracker == null)
                throw new ArgumentNullException("messageTracker");
            if (supportedSerializers == null)
                throw new ArgumentNullException("supportedSerializers");

            _address = address;
            _errorTransport = errorTransport;
            _serializer = serializer;
            _tracker = messageTracker;
            _supportedSerializers = supportedSerializers;
            _transport = transport;

            _disposedMessage = string.Format("The endpoint has already been disposed: {0}", _address);
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
                throw new SendException(typeof(T), _address.Uri, "An exception was thrown during Send", ex);
            }
        }

        public void Send<T>(T message)
            where T : class
        {
            ISendContext<T> context = ContextStorage.CreateSendContext(message);

            Send(context);
        }

        public void Send<T>(T message, Action<ISendContext<T>> contextCallback)
            where T : class
        {
            ISendContext<T> context = ContextStorage.CreateSendContext(message);

            contextCallback(context);

            Send(context);
        }

        public void Send(object message)
        {
            if (message == null)
                throw new ArgumentNullException("message");

            EndpointObjectSenderCache.Instance[message.GetType()].Send(this, message);
        }

        public void Send(object message, Type messageType)
        {
            if (message == null)
                throw new ArgumentNullException("message");
            if (messageType == null)
                throw new ArgumentNullException("messageType");

            EndpointObjectSenderCache.Instance[messageType].Send(this, message);
        }

        public void Send(object message, Action<ISendContext> contextCallback)
        {
            if (message == null)
                throw new ArgumentNullException("message");
            if (contextCallback == null)
                throw new ArgumentNullException("contextCallback");

            Type messageType = message.GetType();

            EndpointObjectSenderCache.Instance[messageType].Send(this, message, contextCallback);
        }

        public void Send(object message, Type messageType, Action<ISendContext> contextCallback)
        {
            if (message == null)
                throw new ArgumentNullException("message");
            if (messageType == null)
                throw new ArgumentNullException("messageType");
            if (contextCallback == null)
                throw new ArgumentNullException("contextCallback");

            EndpointObjectSenderCache.Instance[messageType].Send(this, message, contextCallback);
        }

        /// <summary>
        /// Sends an interface message, initializing the properties of the interface using the anonymous
        /// object specified
        /// </summary>
        /// <typeparam name="T">The interface type to send</typeparam>
        /// <param name="endpoint">The destination endpoint</param>
        /// <param name="values">The property values to initialize on the interface</param>
        public void Send<T>(object values)
            where T : class
        {
            var message = InterfaceImplementationExtensions.InitializeProxy<T>(values);

            Send(message, x => { });
        }

        /// <summary>
        /// Sends an interface message, initializing the properties of the interface using the anonymous
        /// object specified
        /// </summary>
        /// <typeparam name="T">The interface type to send</typeparam>
        /// <param name="endpoint">The destination endpoint</param>
        /// <param name="values">The property values to initialize on the interface</param>
        /// <param name="contextCallback">A callback method to modify the send context for the message</param>
        public void Send<T>(object values, Action<ISendContext<T>> contextCallback)
            where T : class
        {
            var message = InterfaceImplementationExtensions.InitializeProxy<T>(values);

            Send(message, contextCallback);
        }

        public void Dispose()
        {
            Dispose(true);
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
                    string acceptMessageId = acceptContext.OriginalMessageId ?? acceptContext.MessageId;
                    IEnumerable<Action> faultActions;
                    if (_tracker.IsRetryLimitExceeded(acceptMessageId, out retryException, out faultActions))
                    {
                        if (_log.IsErrorEnabled)
                        {
                            _log.ErrorFormat("Message retry limit exceeded {0}:{1}", Address,
                                acceptMessageId);
                        }

                        failedMessageException = retryException;

                        acceptContext.ExecuteFaultActions(faultActions);

                        return x => MoveMessageToErrorTransport(x, retryException);
                    }

                    if (acceptContext.MessageId != acceptMessageId)
                    {
                        if (_log.IsErrorEnabled)
                        {
                            _log.DebugFormat("Message {0} original message id {1}", acceptContext.MessageId,
                                acceptContext.OriginalMessageId);
                        }
                    }

                    Action<IReceiveContext> receive;
                    try
                    {
                        acceptContext.SetEndpoint(this);

                        IMessageSerializer serializer;
                        if (!_supportedSerializers.TryGetSerializer(acceptContext.ContentType, out serializer))
                        {
                            throw new SerializationException(
                                string.Format("The content type could not be deserialized: {0}",
                                    acceptContext.ContentType));
                        }

                        serializer.Deserialize(acceptContext);

                        receive = receiver(acceptContext);
                        if (receive == null)
                        {
                            Address.LogSkipped(acceptMessageId);

                            if (_tracker.IncrementRetryCount(acceptMessageId))
                                return x => MoveMessageToErrorTransport(x);

                            return null;
                        }
                    }
                    catch (SerializationException sex)
                    {
                        if (_log.IsErrorEnabled)
                            _log.Error("Unrecognized message " + Address + ":" + acceptMessageId, sex);

                        _tracker.IncrementRetryCount(acceptMessageId, sex);
                        return x => MoveMessageToErrorTransport(x, sex);
                    }
                    catch (Exception ex)
                    {
                        if (_log.IsErrorEnabled)
                            _log.Error("An exception was thrown preparing the message consumers", ex);

                        if (_tracker.IncrementRetryCount(acceptMessageId, ex))
                        {
                            if (!_tracker.IsRetryEnabled)
                            {
                                acceptContext.ExecuteFaultActions(acceptContext.GetFaultActions());
                                return x => MoveMessageToErrorTransport(x, ex);
                            }
                        }
                        return null;
                    }

                    return receiveContext =>
                    {
                        string receiveMessageId = receiveContext.OriginalMessageId ?? receiveContext.MessageId;
                        try
                        {
                            receive(receiveContext);

                            successfulMessageId = receiveMessageId;
                        }
                        catch (Exception ex)
                        {
                            if (_log.IsErrorEnabled)
                                _log.Error("An exception was thrown by a message consumer", ex);

                            faultActions = receiveContext.GetFaultActions();
                            if (_tracker.IncrementRetryCount(receiveMessageId, ex, faultActions))
                            {
                                if (!_tracker.IsRetryEnabled)
                                {
                                    receiveContext.ExecuteFaultActions(faultActions);
                                    MoveMessageToErrorTransport(receiveContext, ex);

                                    return;
                                }
                            }

                            if (!receiveContext.IsTransactional)
                            {
                                SaveMessageToInboundTransport(receiveContext);
                                return;
                            }

                            throw;
                        }
                    };
                }, timeout);

                if (failedMessageException != null)
                {
                    if (_log.IsErrorEnabled)
                        _log.ErrorFormat("Throwing Original Exception: {0}", failedMessageException.GetType());

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

        void Dispose(bool disposing)
        {
            if (_disposed)
                return;
            if (disposing)
            {
                _transport.Dispose();
                _transport = null;

                _errorTransport.Dispose();
                _errorTransport = null;
            }

            _disposed = true;
        }

        void MoveMessageToErrorTransport(IReceiveContext context, Exception exception = null)
        {
            var moveContext = new MoveMessageSendContext(context);

            if (exception != null)
            {
                moveContext.SetHeader("MT-Fault-Message", exception.Message);
                moveContext.SetHeader("MT-Fault-StackTrace", exception.StackTrace);
            }

            moveContext.SetHeader("MT-Error-Host", Environment.MachineName);
            moveContext.SetHeader("MT-Error-Process", Process.GetCurrentProcess().ProcessName);

            Assembly entryAssembly = Assembly.GetEntryAssembly();
            if (entryAssembly != null)
            {
                AssemblyName assemblyName = entryAssembly.GetName();
                moveContext.SetHeader("MT-Error-Assembly", assemblyName.Name);
            }

            _errorTransport.Send(moveContext);

            string messageId = context.OriginalMessageId ?? context.MessageId;
            _tracker.MessageWasMovedToErrorQueue(messageId);

            Address.LogMoved(_errorTransport.Address, context.MessageId, "");
        }

        void SaveMessageToInboundTransport(IReceiveContext context)
        {
            var moveContext = new MoveMessageSendContext(context);

            _transport.Send(moveContext);

            Address.LogReQueued(_transport.Address, context.MessageId, "");
        }
    }
}