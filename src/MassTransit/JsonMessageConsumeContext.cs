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
namespace MassTransit
{
    using System;
    using System.Threading;
    using Util;


    public class JsonMessageConsumeContext<TMessage> :
        ConsumeContext<TMessage>
        where TMessage : class
    {
        readonly ConsumeContext _context;
        readonly TMessage _message;

        public JsonMessageConsumeContext(ConsumeContext context, TMessage message)
        {
            _context = context;
            _message = message;
        }

        public Guid? MessageId
        {
            get { return _context.MessageId; }
        }

        public Guid? RequestId
        {
            get { return _context.RequestId; }
        }

        public Guid? CorrelationId
        {
            get { return _context.CorrelationId; }
        }

        public DateTime? ExpirationTime
        {
            get { return _context.ExpirationTime; }
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
            get { return _context.ResponseAddress; }
        }

        public Uri FaultAddress
        {
            get { return _context.FaultAddress; }
        }

        public Headers Headers
        {
            get { return _context.Headers; }
        }

        public CancellationToken CancellationToken
        {
            get { return _context.CancellationToken; }
        }

        public ReceiveContext ReceiveContext
        {
            get { return _context.ReceiveContext; }
        }

        public bool HasMessageType(Type messageType)
        {
            return _context.HasMessageType(messageType);
        }

        public bool TryGetMessage<T>(out ConsumeContext<T> consumeContext) 
            where T : class
        {
            return _context.TryGetMessage(out consumeContext);
        }

        public void NotifyConsumed(TimeSpan elapsed, string messageType, string consumerType)
        {
            _context.NotifyConsumed(elapsed, messageType, consumerType);
        }

        public void NotifyFaulted(string messageType, string consumerType, Exception exception)
        {
            
        }

        public TMessage Message
        {
            get { return _message; }
        }

        public void NotifyConsumed(TimeSpan elapsed, string consumerType)
        {
            _context.NotifyConsumed(elapsed, TypeMetadataCache<TMessage>.ShortName, consumerType);
        }

        public void NotifyFaulted(string consumerType, Exception exception)
        {
            _context.NotifyFaulted(TypeMetadataCache<TMessage>.ShortName, consumerType, exception);
        }
    }
}