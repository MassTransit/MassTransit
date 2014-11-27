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
    using System.Collections.Generic;
    using System.IO;
    using Serialization;


    public class ReceiveContextAdapter :
        IReceiveContext
    {
        readonly ConsumeContext _context;

        public ReceiveContextAdapter(ConsumeContext context)
        {
            _context = context;
        }

        public string MessageId
        {
            get { throw new NotImplementedException(); }
        }

        public string MessageType
        {
            get { throw new NotImplementedException(); }
        }

        public string ContentType
        {
            get { throw new NotImplementedException(); }
        }

        public string RequestId
        {
            get { throw new NotImplementedException(); }
        }

        public string ConversationId
        {
            get { throw new NotImplementedException(); }
        }

        public string CorrelationId
        {
            get { throw new NotImplementedException(); }
        }

        public Uri SourceAddress
        {
            get { throw new NotImplementedException(); }
        }

        public Uri InputAddress
        {
            get { throw new NotImplementedException(); }
        }

        public Uri DestinationAddress
        {
            get { throw new NotImplementedException(); }
        }

        public Uri ResponseAddress
        {
            get { throw new NotImplementedException(); }
        }

        public Uri FaultAddress
        {
            get { throw new NotImplementedException(); }
        }

        public string Network
        {
            get { throw new NotImplementedException(); }
        }

        public DateTime? ExpirationTime
        {
            get { throw new NotImplementedException(); }
        }

        public int RetryCount
        {
            get { throw new NotImplementedException(); }
        }

        public IMessageHeaders Headers
        {
            get { throw new NotImplementedException(); }
        }

        public IReceiveContext BaseContext
        {
            get { throw new NotImplementedException(); }
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
            throw new NotImplementedException();
        }

        public bool TryGetContext<T>(out IConsumeContext<T> context) where T : class
        {
            throw new NotImplementedException();
        }

        public void Respond<T>(T message, Action<ISendContext<T>> contextCallback) where T : class
        {
            throw new NotImplementedException();
        }

        public Stream BodyStream
        {
            get { throw new NotImplementedException(); }
        }

        public IEnumerable<ISent> Sent
        {
            get { throw new NotImplementedException(); }
        }

        public IEnumerable<IReceived> Received
        {
            get { throw new NotImplementedException(); }
        }

        public Guid Id
        {
            get { throw new NotImplementedException(); }
        }

        public bool IsTransactional
        {
            get { throw new NotImplementedException(); }
        }

        public string OriginalMessageId
        {
            get { throw new NotImplementedException(); }
        }

        public void SetContentType(string value)
        {
            throw new NotImplementedException();
        }

        public void SetMessageId(string value)
        {
            throw new NotImplementedException();
        }

        public void SetInputAddress(Uri uri)
        {
            throw new NotImplementedException();
        }

        public void SetEndpoint(IEndpoint endpoint)
        {
            throw new NotImplementedException();
        }

        public void SetBus(IServiceBus bus)
        {
            throw new NotImplementedException();
        }

        public void SetRequestId(string value)
        {
            throw new NotImplementedException();
        }

        public void SetConversationId(string value)
        {
            throw new NotImplementedException();
        }

        public void SetCorrelationId(string value)
        {
            throw new NotImplementedException();
        }

        public void SetOriginalMessageId(string value)
        {
            throw new NotImplementedException();
        }

        public void SetSourceAddress(Uri uri)
        {
            throw new NotImplementedException();
        }

        public void SetDestinationAddress(Uri uri)
        {
            throw new NotImplementedException();
        }

        public void SetResponseAddress(Uri uri)
        {
            throw new NotImplementedException();
        }

        public void SetFaultAddress(Uri uri)
        {
            throw new NotImplementedException();
        }

        public void SetNetwork(string value)
        {
            throw new NotImplementedException();
        }

        public void SetRetryCount(int retryCount)
        {
            throw new NotImplementedException();
        }

        public void SetExpirationTime(DateTime value)
        {
            throw new NotImplementedException();
        }

        public void SetMessageType(string messageType)
        {
            throw new NotImplementedException();
        }

        public void SetHeader(string key, string value)
        {
            throw new NotImplementedException();
        }

        public void SetBodyStream(Stream stream)
        {
            throw new NotImplementedException();
        }

        public void CopyBodyTo(Stream stream)
        {
            throw new NotImplementedException();
        }

        public void SetMessageTypeConverter(IMessageTypeConverter messageTypeConverter)
        {
            throw new NotImplementedException();
        }

        public void NotifyFault(Action faultAction)
        {
            throw new NotImplementedException();
        }

        public void NotifySend(ISendContext context, EndpointAddress address)
        {
            throw new NotImplementedException();
        }

        public void NotifySend<T>(ISendContext<T> sendContext, EndpointAddress address) where T : class
        {
            throw new NotImplementedException();
        }


        public void NotifyConsume<T>(IConsumeContext<T> consumeContext, string consumerType, string correlationId)
            where T : class
        {
            throw new NotImplementedException();
        }

        public void ExecuteFaultActions(IEnumerable<Action> faultActions)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<Action> GetFaultActions()
        {
            throw new NotImplementedException();
        }
    }
}