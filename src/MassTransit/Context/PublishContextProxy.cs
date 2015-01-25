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
    using System.Net.Mime;
    using System.Threading;
    using Serialization;


    public class PublishContextProxy :
        PublishContext
    {
        readonly SendContext _context;

        public PublishContextProxy(SendContext context)
        {
            _context = context;
        }

        public bool Mandatory { get; set; }

        public CancellationToken CancellationToken
        {
            get { return _context.CancellationToken; }
        }

        public bool HasPayloadType(Type contextType)
        {
            return _context.HasPayloadType(contextType);
        }

        public bool TryGetPayload<TPayload>(out TPayload payload) where TPayload : class
        {
            return _context.TryGetPayload(out payload);
        }

        public TPayload GetOrAddPayload<TPayload>(PayloadFactory<TPayload> payloadFactory) where TPayload : class
        {
            return _context.GetOrAddPayload(payloadFactory);
        }

        public Uri SourceAddress
        {
            get { return _context.SourceAddress; }
            set { _context.SourceAddress = value; }
        }

        public Uri DestinationAddress
        {
            get { return _context.DestinationAddress; }
            set { _context.DestinationAddress = value; }
        }

        public Uri ResponseAddress
        {
            get { return _context.ResponseAddress; }
            set { _context.ResponseAddress = value; }
        }

        public Uri FaultAddress
        {
            get { return _context.FaultAddress; }
            set { _context.FaultAddress = value; }
        }

        public Guid? RequestId
        {
            get { return _context.RequestId; }
            set { _context.RequestId = value; }
        }

        public Guid? MessageId
        {
            get { return _context.MessageId; }
            set { _context.MessageId = value; }
        }

        public Guid? CorrelationId
        {
            get { return _context.CorrelationId; }
            set { _context.CorrelationId = value; }
        }

        public SendContextHeaders Headers
        {
            get { return _context.Headers; }
        }

        public TimeSpan? TimeToLive
        {
            get { return _context.TimeToLive; }
            set { _context.TimeToLive = value; }
        }

        public ContentType ContentType
        {
            get { return _context.ContentType; }
            set { _context.ContentType = value; }
        }

        public bool Durable
        {
            get { return _context.Durable; }
            set { _context.Durable = value; }
        }

        public IMessageSerializer Serializer
        {
            get { return _context.Serializer; }
            set { _context.Serializer = value; }
        }
    }


    public class PublishContextProxy<T> :
        PublishContext<T>
        where T : class
    {
        readonly SendContext<T> _context;

        public PublishContextProxy(SendContext<T> context)
        {
            _context = context;
        }

        public bool Mandatory { get; set; }

        public CancellationToken CancellationToken
        {
            get { return _context.CancellationToken; }
        }

        public bool HasPayloadType(Type contextType)
        {
            return _context.HasPayloadType(contextType);
        }

        public bool TryGetPayload<TPayload>(out TPayload payload) where TPayload : class
        {
            return _context.TryGetPayload(out payload);
        }

        public TPayload GetOrAddPayload<TPayload>(PayloadFactory<TPayload> payloadFactory) where TPayload : class
        {
            return _context.GetOrAddPayload(payloadFactory);
        }

        public Uri SourceAddress
        {
            get { return _context.SourceAddress; }
            set { _context.SourceAddress = value; }
        }

        public Uri DestinationAddress
        {
            get { return _context.DestinationAddress; }
            set { _context.DestinationAddress = value; }
        }

        public Uri ResponseAddress
        {
            get { return _context.ResponseAddress; }
            set { _context.ResponseAddress = value; }
        }

        public Uri FaultAddress
        {
            get { return _context.FaultAddress; }
            set { _context.FaultAddress = value; }
        }

        public Guid? RequestId
        {
            get { return _context.RequestId; }
            set { _context.RequestId = value; }
        }

        public Guid? MessageId
        {
            get { return _context.MessageId; }
            set { _context.MessageId = value; }
        }

        public Guid? CorrelationId
        {
            get { return _context.CorrelationId; }
            set { _context.CorrelationId = value; }
        }

        public SendContextHeaders Headers
        {
            get { return _context.Headers; }
        }

        public TimeSpan? TimeToLive
        {
            get { return _context.TimeToLive; }
            set { _context.TimeToLive = value; }
        }

        public ContentType ContentType
        {
            get { return _context.ContentType; }
            set { _context.ContentType = value; }
        }

        public bool Durable
        {
            get { return _context.Durable; }
            set { _context.Durable = value; }
        }

        public IMessageSerializer Serializer
        {
            get { return _context.Serializer; }
            set { _context.Serializer = value; }
        }

        public T Message
        {
            get { return _context.Message; }
        }
    }
}