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
namespace MassTransit.Context
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.IO;
    using System.Runtime.Serialization;
    using Logging;
    using Serialization;
    using Util;

    public class ReceiveContext :
        MessageContext,
        IReceiveContext
    {
        static readonly ILog _log = Logger.Get(typeof(ReceiveContext));
        readonly IList<IPublished> _published;
        readonly IList<IReceived> _received;
        readonly IList<ISent> _sent;
        readonly bool _transactional;
        Stream _bodyStream;
        Stopwatch _timer;
        IMessageTypeConverter _typeConverter;
        readonly IList<Action> _faultActions;

        ReceiveContext()
        {
            Id = NewId.NextGuid();
            
            _faultActions = new List<Action>();
            _timer = Stopwatch.StartNew();
            _sent = new List<ISent>();
            _published = new List<IPublished>();
            _received = new List<IReceived>();
        }

        ReceiveContext(Stream bodyStream, bool transactional)
            : this()
        {
            _bodyStream = bodyStream;
            _transactional = transactional;
        }

        /// <summary>
        /// The endpoint from which the message was received
        /// </summary>
        public IEndpoint Endpoint { get; private set; }

        public IReceiveContext BaseContext
        {
            get { return this; }
        }

        /// <summary>
        /// The bus on which the message was received
        /// </summary>
        public IServiceBus Bus { get; private set; }


        public void SetBus(IServiceBus bus)
        {
            Bus = bus;
        }

        public void SetEndpoint(IEndpoint endpoint)
        {
            Endpoint = endpoint;
        }

        public void SetBodyStream(Stream stream)
        {
            if (stream == null)
                throw new ArgumentNullException("stream");
            _bodyStream = stream;
        }

        public void CopyBodyTo(Stream stream)
        {
            _bodyStream.Seek(0, SeekOrigin.Begin);
            _bodyStream.CopyTo(stream);
        }

        public Stream BodyStream
        {
            get
            {
                _bodyStream.Seek(0, SeekOrigin.Begin);
                return _bodyStream;
            }
        }

        public void SetMessageTypeConverter(IMessageTypeConverter serializer)
        {
            _typeConverter = serializer;
        }

        public void NotifyFault(Action faultAction)
        {
            _faultActions.Add(faultAction);
        }

        public void NotifySend(ISendContext context, IEndpointAddress address)
        {
            _sent.Add(new Sent(context, address, _timer.ElapsedMilliseconds));
        }

        public void NotifySend<T>(ISendContext<T> sendContext, IEndpointAddress address)
            where T : class
        {
            _sent.Add(new Sent<T>(sendContext, address, _timer.ElapsedMilliseconds));
        }

        public void NotifyPublish<T>(IPublishContext<T> publishContext)
            where T : class
        {
            _published.Add(new Published<T>(publishContext));
        }

        public void NotifyConsume<T>(IConsumeContext<T> consumeContext, string consumerType, string correlationId)
            where T : class
        {
            if (Endpoint != null)
                Endpoint.Address.LogReceived(consumeContext.MessageId, typeof(T).ToMessageName());

            _received.Add(new Received<T>(consumeContext, consumerType, correlationId, _timer.ElapsedMilliseconds));
        }

        public IEnumerable<ISent> Sent
        {
            get { return _sent; }
        }

        public IEnumerable<IReceived> Received
        {
            get { return _received; }
        }

        public Guid Id { get; private set; }

        public bool IsTransactional
        {
            get { return _transactional; }
        }

        public void ExecuteFaultActions(IEnumerable<Action> faultActions)
        {
            try
            {
                foreach (var callback in faultActions)
                    callback();
            }
            catch (Exception ex)
            {
                _log.Error("Failed to execute pending fault", ex);
            }
        }

        public IEnumerable<Action> GetFaultActions()
        {
            return _faultActions;
        }

        public bool IsContextAvailable(Type messageType)
        {
            return _typeConverter.Contains(messageType);
        }

        public bool TryGetContext<T>(out IConsumeContext<T> context)
            where T : class
        {
            try
            {
                T message;
                if (_typeConverter != null && _typeConverter.TryConvert(out message))
                {
                    context = new ConsumeContext<T>(this, message);
                    return true;
                }

                context = null;
                return false;
            }
            catch (Exception ex)
            {
                var exception = new SerializationException("Failed to deserialize the message", ex);

                throw exception;
            }
        }

        /// <summary>
        /// Respond to the current inbound message with either a send to the ResponseAddress or a
        /// Publish on the bus that received the message
        /// </summary>
        /// <typeparam name="T">Message type</typeparam>
        /// <param name="message">The message to send/publish</param>
        /// <param name="contextCallback">The action to setup the context on the outbound message</param>
        public void Respond<T>(T message, [NotNull] Action<ISendContext<T>> contextCallback) where T : class
        {
            if (contextCallback == null)
                throw new ArgumentNullException("contextCallback");
            if (ResponseAddress != null)
            {
                Bus.GetEndpoint(ResponseAddress).Send(message, context =>
                    {
                        context.SetSourceAddress(Bus.Endpoint.Address.Uri);
                        context.SetRequestId(RequestId);
                        contextCallback(context);
                    });
            }
            else
            {
                Bus.Publish(message, context =>
                    {
                        context.SetRequestId(RequestId);
                        contextCallback(context);
                    });
            }
        }

        /// <summary>
        /// Create a new <see cref="ReceiveContext"/> from the incoming 
        /// stream; the stream should contain the MassTransit <see cref="Envelope"/>
        /// which in turn contains both payload and meta-data/out-of-band data.
        /// </summary>
        /// <param name="bodyStream">Body stream to create receive context from</param>
        /// <param name="transactional">True if the transport is transactional and will roll back failed messages </param>
        /// <returns>The receive context</returns>
        [NotNull]
        public static ReceiveContext FromBodyStream(Stream bodyStream, bool transactional)
        {
            return new ReceiveContext(bodyStream, transactional);
        }

        [NotNull]
        public static ReceiveContext FromBodyStream(Stream bodyStream)
        {
            return new ReceiveContext(bodyStream, false);
        }

        /// <summary>
        /// Create a new empty receive context
        /// </summary>
        /// <returns></returns>
        [NotNull]
        public static ReceiveContext Empty()
        {
            return new ReceiveContext(null, false);
        }
    }
}