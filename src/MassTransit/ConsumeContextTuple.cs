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
    using System.Threading.Tasks;


    public class ConsumeContextTuple<T1, T> :
        ConsumeContext<T1, T>
        where T : class
    {
        readonly ConsumeContext<T> _context;
        readonly T1 _item1;

        public ConsumeContextTuple(ConsumeContext<T> context, T1 item1)
        {
            _context = context;
            _item1 = item1;
        }

        public T1 Item1
        {
            get { return _item1; }
        }

        public ConsumeContext<T> Pop()
        {
            return _context;
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

        public ContextHeaders ContextHeaders
        {
            get { return _context.ContextHeaders; }
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

        public bool TryGetMessage<T2>(out ConsumeContext<T2> consumeContext) where T2 : class
        {
            return _context.TryGetMessage(out consumeContext);
        }

        public Task RespondAsync<T2>(T2 message) where T2 : class
        {
            return _context.RespondAsync(message);
        }

        public void Respond<T2>(T2 message) where T2 : class
        {
            _context.Respond(message);
        }

        public void RetryLater()
        {
            _context.RetryLater();
        }

        public IEndpoint GetEndpoint(Uri address)
        {
            return _context.GetEndpoint(address);
        }

        public void NotifyConsumed(TimeSpan elapsed, string messageType, string consumerType)
        {
            _context.NotifyConsumed(elapsed, messageType, consumerType);
        }

        public void NotifyFaulted(string messageType, string consumerType, Exception exception)
        {
            _context.NotifyFaulted(messageType, consumerType, exception);
        }

        public T Message
        {
            get { return _context.Message; }
        }

        public void NotifyConsumed(TimeSpan elapsed, string consumerType)
        {
            _context.NotifyConsumed(elapsed, consumerType);
        }

        public void NotifyFaulted(string consumerType, Exception exception)
        {
            _context.NotifyFaulted(consumerType, exception);
        }
    }
}