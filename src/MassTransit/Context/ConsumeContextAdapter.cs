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
namespace MassTransit.Context
{
    using System;


    public class ConsumeContextAdapter<TMessage> :
        IConsumeContext<TMessage>
        where TMessage : class
    {
        readonly ConsumeContext<TMessage> _context;
        readonly Lazy<IMessageHeaders> _messageHeaders;
        readonly Lazy<IReceiveContext> _receiveContext;

        public ConsumeContextAdapter(ConsumeContext<TMessage> context)
        {
            _context = context;
            _receiveContext = new Lazy<IReceiveContext>(() => new ReceiveContextAdapter(context));
            _messageHeaders = new Lazy<IMessageHeaders>(() => new MessageHeadersAdapter(context));
        }

        public string MessageId
        {
            get { return _context.MessageId.HasValue ? _context.MessageId.ToString() : null; }
        }

        public string MessageType
        {
            get { return typeof(TMessage).ToMessageName(); }
        }

        public string ContentType
        {
            get { return _context.ReceiveContext.ContentType.ToString(); }
        }

        public string RequestId
        {
            get { return _context.RequestId.HasValue ? _context.RequestId.ToString() : null; }
        }

        public string ConversationId
        {
            get { return null; }
        }

        public string CorrelationId
        {
            get { return _context.CorrelationId.HasValue ? _context.CorrelationId.ToString() : null; }
        }

        public Uri SourceAddress
        {
            get { return _context.SourceAddress; }
        }

        public Uri InputAddress
        {
            get { return _context.ReceiveContext.InputAddress; }
        }

        public Uri DestinationAddress
        {
            get { return _context.DestinationAddress; }
        }

        public Uri ResponseAddress
        {
            get { return _context.ResponseAddress; }
        }

        public Uri FaultAddress
        {
            get { return _context.FaultAddress; }
        }

        public string Network
        {
            get { return null; }
        }

        public DateTime? ExpirationTime
        {
            get { return _context.ExpirationTime; }
        }

        public int RetryCount
        {
            get { return 0; }
        }

        public IMessageHeaders Headers
        {
            get { return _messageHeaders.Value; }
        }

        public IReceiveContext BaseContext
        {
            get { return _receiveContext.Value; }
        }

        public IServiceBus Bus
        {
            get { throw new NotImplementedException(); }
        }

        public IEndpoint Endpoint
        {
            get { throw new NotImplementedException(); }
        }

        public bool IsContextAvailable(Type messageType)
        {
            return _context.HasMessageType(messageType);
        }

        public bool TryGetContext<T>(out IConsumeContext<T> context)
            where T : class
        {
            ConsumeContext<T> consumeContext;
            if (_context.TryGetMessage(out consumeContext))
            {
                context = new ConsumeContextAdapter<T>(consumeContext);
                return true;
            }

            context = default(IConsumeContext<T>);
            return false;
        }

        public void Respond<T>(T message, Action<ISendContext<T>> contextCallback)
            where T : class
        {
            _context.Respond(message);
        }

        public TMessage Message
        {
            get { return _context.Message; }
        }

        public void RetryLater()
        {
            throw new NotImplementedException();
        }

        public void GenerateFault(Exception ex)
        {
            throw new NotImplementedException();
        }
    }
}