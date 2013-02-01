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
    using System.Linq;
    using Logging;
    using Magnum.Reflection;
    using Util;

    public class ConsumeContext<TMessage> :
        IConsumeContext<TMessage>
        where TMessage : class
    {
        static readonly ILog _log = Logger.Get(typeof(ReceiveContext));

        readonly IReceiveContext _context;
        readonly TMessage _message;
        readonly Uri _responseAddress;

        public ConsumeContext(IReceiveContext context, TMessage message)
        {
            _context = context;
            _message = message;
            _responseAddress = context.ResponseAddress;
        }

        public IMessageHeaders Headers
        {
            get { return _context.Headers; }
        }

        public string RequestId
        {
            get { return _context.RequestId; }
        }

        public string ConversationId
        {
            get { return _context.ConversationId; }
        }

        public string CorrelationId
        {
            get { return _context.CorrelationId; }
        }

        public string MessageId
        {
            get { return _context.MessageId; }
        }

        public string MessageType
        {
            get { return typeof(TMessage).ToMessageName(); }
        }

        public string ContentType
        {
            get { return _context.ContentType; }
        }

        public Uri SourceAddress
        {
            get { return _context.SourceAddress; }
        }

        public Uri DestinationAddress
        {
            get { return _context.DestinationAddress; }
        }

        public Uri ResponseAddress
        {
            get { return _responseAddress; }
        }

        public Uri FaultAddress
        {
            get { return _context.FaultAddress; }
        }

        public string Network
        {
            get { return _context.Network; }
        }

        public DateTime? ExpirationTime
        {
            get { return _context.ExpirationTime; }
        }

        public int RetryCount
        {
            get { return _context.RetryCount; }
        }

        public IServiceBus Bus
        {
            get { return _context.Bus; }
        }

        public IEndpoint Endpoint
        {
            get { return _context.Endpoint; }
        }

        public Uri InputAddress
        {
            get { return _context.InputAddress; }
        }

        public TMessage Message
        {
            get { return _message; }
        }

        public bool IsContextAvailable(Type messageType)
        {
            if (messageType.IsInstanceOfType(Message))
                return true;

            if (_context != null)
                return _context.IsContextAvailable(messageType);

            return false;
        }

        public bool TryGetContext<T>(out IConsumeContext<T> context)
            where T : class
        {
            var messageOfT = Message as T;
            if (messageOfT != null)
            {
                context = new ConsumeContext<T>(_context, messageOfT);
                return true;
            }
            if (_context != null)
                return _context.TryGetContext(out context);

            context = null;
            return false;
        }

        public IReceiveContext BaseContext
        {
            get { return _context; }
        }

        public void RetryLater()
        {
            if (_log.IsDebugEnabled)
                _log.DebugFormat("Retrying message of type {0} later", typeof(TMessage));

            Bus.Endpoint.Send(Message, x =>
                {
                    x.SetUsing(this);
                    x.SetRetryCount(RetryCount + 1);
                });
        }

        public void Respond<T>(T message, Action<ISendContext<T>> contextCallback)
            where T : class
        {
            _context.Respond(message, contextCallback);
        }

        public void GenerateFault(Exception ex)
        {
            if (Message == null)
                throw new InvalidOperationException("A fault cannot be generated when no message is present");

            Type correlationType = typeof(TMessage).GetInterfaces()
                .Where(x => x.IsGenericType)
                .Where(x => x.GetGenericTypeDefinition() == typeof(CorrelatedBy<>))
                .Select(x => x.GetGenericArguments()[0])
                .DefaultIfEmpty(null)
                .FirstOrDefault();

            if (correlationType != null)
            {
                this.FastInvoke(new[] {typeof(TMessage), correlationType}, "CreateAndSendCorrelatedFault", Message, ex);
            }
            else
            {
                this.FastInvoke(new[] {typeof(TMessage)}, "CreateAndSendFault", Message, ex);
            }
        }

        [UsedImplicitly]
        void CreateAndSendFault<T>(T message, Exception exception)
            where T : class
        {
            var fault = new Fault<T>(message, exception);
            var bus = Bus;
            var faultAddress = FaultAddress;
            var responseAddress = ResponseAddress;
            var requestId = RequestId;

            _context.NotifyFault(() => SendFault(bus, faultAddress, responseAddress, requestId, fault));
        }

        [UsedImplicitly]
        void CreateAndSendCorrelatedFault<T, TKey>(T message, Exception exception)
            where T : class, CorrelatedBy<TKey>
        {
            var fault = new Fault<T, TKey>(message, exception);
            var bus = Bus;
            var faultAddress = FaultAddress;
            var responseAddress = ResponseAddress;
            var requestId = RequestId;

            _context.NotifyFault(() => SendFault(bus, faultAddress, responseAddress, requestId, fault));
        }

        static void SendFault<T>(IServiceBus bus, Uri faultAddress, Uri responseAddress, string requestId, T message)
            where T : class
        {
            if (faultAddress != null)
            {
                bus.GetEndpoint(faultAddress).Send(message, context =>
                    {
                        context.SetSourceAddress(bus.Endpoint.Address.Uri);
                        context.SetRequestId(requestId);
                    });
            }
            else if (responseAddress != null)
            {
                bus.GetEndpoint(responseAddress).Send(message, context =>
                    {
                        context.SetSourceAddress(bus.Endpoint.Address.Uri);
                        context.SetRequestId(requestId);
                    });
            }
            else
            {
                bus.Publish(message, context => context.SetRequestId(requestId));
            }
        }
    }
}