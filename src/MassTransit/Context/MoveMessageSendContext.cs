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
    using System.IO;
    using System.Threading;


    public class MoveMessageSendContext :
        MessageContext,
        ISendContext
    {
        readonly Action<Stream> _bodyWriter;
        readonly Action<EndpointAddress> _notifySend;

        public MoveMessageSendContext(IReceiveContext context)
        {
//            SetUsing(context);
            CopyOrInitializeOriginalMessageId(context);

            Id = context.Id;

            _notifySend = address => context.NotifySend(this, address);

            _bodyWriter = stream => context.CopyBodyTo(stream);
        }

        public Guid Id { get; set; }

        public Type DeclaringMessageType
        {
            get { return typeof(object); }
        }

        public string OriginalMessageId
        {
            get { throw new NotImplementedException(); }
        }

        public void SetMessageType(string messageType)
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

        public void SetNetwork(string network)
        {
            throw new NotImplementedException();
        }

        public void SetExpirationTime(DateTime value)
        {
            throw new NotImplementedException();
        }

        public void SetRetryCount(int retryCount)
        {
            throw new NotImplementedException();
        }

        public void SetUsing(IMessageContext context)
        {
            throw new NotImplementedException();
        }

        public void SetHeader(string key, string value)
        {
            throw new NotImplementedException();
        }

        public void SetDeliveryMode(DeliveryMode deliveryMode)
        {
            DeliveryMode = deliveryMode;
        }

        public DeliveryMode DeliveryMode { get; private set; }

        public void SerializeTo(Stream stream)
        {
            _bodyWriter(stream);
        }

        public void NotifySend(EndpointAddress address)
        {
            _notifySend(address);
        }

        void CopyOrInitializeOriginalMessageId(IReceiveContext context)
        {
//            SetOriginalMessageId(context.OriginalMessageId);
//
//            if (string.IsNullOrEmpty(OriginalMessageId))
//                SetOriginalMessageId(context.MessageId);
        }

        public CancellationToken CancellationToken
        {
            get { throw new NotImplementedException(); }
        }

        public bool HasPayloadType(Type contextType)
        {
            throw new NotImplementedException();
        }

        public bool TryGetPayload<TPayload>(out TPayload payload) where TPayload : class
        {
            throw new NotImplementedException();
        }

        public TPayload GetOrAddPayload<TPayload>(PayloadFactory<TPayload> payloadFactory) where TPayload : class
        {
            throw new NotImplementedException();
        }

        Guid? MessageContext.MessageId
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

        string IMessageContext.RequestId
        {
            get { throw new NotImplementedException(); }
        }

        public string ConversationId
        {
            get { throw new NotImplementedException(); }
        }

        string IMessageContext.CorrelationId
        {
            get { throw new NotImplementedException(); }
        }

        Uri IMessageContext.SourceAddress
        {
            get { throw new NotImplementedException(); }
        }

        public Uri InputAddress
        {
            get { throw new NotImplementedException(); }
        }

        Uri IMessageContext.DestinationAddress
        {
            get { throw new NotImplementedException(); }
        }

        Uri IMessageContext.ResponseAddress
        {
            get { throw new NotImplementedException(); }
        }

        Uri IMessageContext.FaultAddress
        {
            get { throw new NotImplementedException(); }
        }

        public string Network
        {
            get { throw new NotImplementedException(); }
        }

        DateTime? IMessageContext.ExpirationTime
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

        string IMessageContext.MessageId
        {
            get { throw new NotImplementedException(); }
        }

        Guid? MessageContext.RequestId
        {
            get { throw new NotImplementedException(); }
        }

        Guid? MessageContext.CorrelationId
        {
            get { throw new NotImplementedException(); }
        }

        DateTime? MessageContext.ExpirationTime
        {
            get { throw new NotImplementedException(); }
        }

        Uri MessageContext.SourceAddress
        {
            get { throw new NotImplementedException(); }
        }

        Uri MessageContext.DestinationAddress
        {
            get { throw new NotImplementedException(); }
        }

        Uri MessageContext.ResponseAddress
        {
            get { throw new NotImplementedException(); }
        }

        Uri MessageContext.FaultAddress
        {
            get { throw new NotImplementedException(); }
        }

        public ContextHeaders ContextHeaders
        {
            get { throw new NotImplementedException(); }
        }
    }
}