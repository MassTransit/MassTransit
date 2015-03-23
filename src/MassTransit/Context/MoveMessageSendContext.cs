// Copyright 2007-2015 Chris Patterson, Dru Sellers, Travis Smith, et. al.
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
    using System.Net.Mime;
    using System.Threading;
    using Serialization;
    using Transports.InMemory;


    public class MoveMessageSendContext :
        SendContext
    {
        readonly SendHeaders _headers;
        readonly ReceiveContext _receiveContext;

        public MoveMessageSendContext(ReceiveContext receiveContext)
        {
            _receiveContext = receiveContext;
            _headers = new InMemorySendHeaders();
            Serializer = new CopyBodyMessageSerializer(_receiveContext.Body, _receiveContext.ContentType);
            ContentType = _receiveContext.ContentType;

            Guid? messageId = _receiveContext.TransportHeaders.Get("MessageId", default(Guid?));
            MessageId = messageId.HasValue ? messageId.Value : NewId.NextGuid();
        }

        public bool HasPayloadType(Type contextType)
        {
            return _receiveContext.HasPayloadType(contextType);
        }

        public bool TryGetPayload<TPayload>(out TPayload payload) where TPayload : class
        {
            return _receiveContext.TryGetPayload(out payload);
        }

        public TPayload GetOrAddPayload<TPayload>(PayloadFactory<TPayload> payloadFactory) where TPayload : class
        {
            return _receiveContext.GetOrAddPayload(payloadFactory);
        }

        public CancellationToken CancellationToken
        {
            get { return _receiveContext.CancellationToken; }
        }

//            CopyOrInitializeOriginalMessageId(context);
//
//          //  Id = context.Id;
//
//        //    _notifySend = address => context.NotifySend(this, address);
//        void CopyOrInitializeOriginalMessageId(ReceiveContext context)
//        {
////            SetOriginalMessageId(context.OriginalMessageId);
////
////            if (string.IsNullOrEmpty(OriginalMessageId))
////                SetOriginalMessageId(context.MessageId);
//        }
//
        public Uri SourceAddress { get; set; }

        public Uri DestinationAddress { get; set; }

        public Uri ResponseAddress { get; set; }

        public Uri FaultAddress { get; set; }

        public Guid? RequestId { get; set; }

        public Guid? MessageId { get; set; }

        public Guid? CorrelationId { get; set; }

        public SendHeaders Headers
        {
            get { return _headers; }
        }

        public TimeSpan? TimeToLive { get; set; }

        public ContentType ContentType { get; set; }

        public bool Durable { get; set; }

        public IMessageSerializer Serializer { get; set; }
    }
}