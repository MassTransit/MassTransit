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
namespace MassTransit.AzureServiceBusTransport
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.IO;
    using System.Net.Mime;
    using System.Text;
    using System.Threading;
    using Context;
    using Microsoft.ServiceBus.Messaging;


    public class AzureServiceBusReceiveContext :
        ReceiveContext,
        BrokeredMessageContext
    {
        static readonly ContentType DefaultContentType = new ContentType("application/vnd.masstransit+json");

        readonly byte[] _body;
        readonly CancellationTokenSource _cancellationTokenSource;
        readonly Uri _inputAddress;
        readonly BrokeredMessage _message;
        readonly PayloadCache _payloadCache;
        readonly Stopwatch _receiveTimer;
        ContentType _contentType;
        Encoding _encoding;
        ContextHeaders _headers;

        public AzureServiceBusReceiveContext(BrokeredMessage message, Uri inputAddress)
        {
            _receiveTimer = Stopwatch.StartNew();

            _payloadCache = new PayloadCache();

            _message = message;
            _inputAddress = inputAddress;

            _cancellationTokenSource = new CancellationTokenSource();
        }

        public IDictionary<string, object> Properties
        {
            get { return _message.Properties; }
        }

        public int DeliveryCount
        {
            get { return _message.DeliveryCount; }
        }

        public string Label
        {
            get { return _message.Label; }
        }

        public long SequenceNumber
        {
            get { return _message.SequenceNumber; }
        }

        public long EnqueuedSequenceNumber
        {
            get { return _message.EnqueuedSequenceNumber; }
        }

        public Guid LockToken
        {
            get { return _message.LockToken; }
        }

        public DateTime LockedUntil
        {
            get { return _message.LockedUntilUtc; }
        }

        public string SessionId
        {
            get { return _message.SessionId; }
        }

        public long Size
        {
            get { return _message.Size; }
        }

        public MessageState State
        {
            get { return _message.State; }
        }

        public bool ForcePersistence
        {
            get { return _message.ForcePersistence; }
        }

        public string To
        {
            get { return _message.To; }
        }

        public string ReplyToSessionId
        {
            get { return _message.ReplyToSessionId; }
        }

        public string PartitionKey
        {
            get { return _message.PartitionKey; }
        }

        public string ViaPartitionKey
        {
            get { return _message.ViaPartitionKey; }
        }

        public string ReplyTo
        {
            get { return _message.ReplyTo; }
        }

        public DateTime EnqueuedTime
        {
            get { return _message.EnqueuedTimeUtc; }
        }

        public DateTime ScheduledEnqueueTime
        {
            get { return _message.ScheduledEnqueueTimeUtc; }
        }

        public bool HasPayloadType(Type contextType)
        {
            return _payloadCache.HasPayloadType(contextType);
        }

        public bool TryGetPayload<TPayload>(out TPayload context)
            where TPayload : class
        {
            return _payloadCache.TryGetPayload(out context);
        }

        public TPayload GetOrAddPayload<TPayload>(PayloadFactory<TPayload> payloadFactory)
            where TPayload : class
        {
            return _payloadCache.GetOrAddPayload(payloadFactory);
        }

        public bool Redelivered
        {
            get { return _message.DeliveryCount > 1; }
        }

        public CancellationToken CancellationToken
        {
            get { return _cancellationTokenSource.Token; }
        }

        public Stream Body
        {
            get { return new MemoryStream(_body, 0, _body.Length, false); }
        }

        public TimeSpan ElapsedTime
        {
            get { return _receiveTimer.Elapsed; }
        }

        public Uri InputAddress
        {
            get { return _inputAddress; }
        }

        public ContentType ContentType
        {
            get { return _contentType ?? (_contentType = GetContentType()); }
        }

        public ContextHeaders TransportHeaders
        {
            get { return _headers ?? (_headers = new JsonContextHeaders(new AzureServiceBusContextHeaderProvider(this))); }
        }

        public void NotifyConsumed(TimeSpan elapsed, string messageType, string consumerType)
        {
        }

        public void NotifyFaulted(string messageType, string consumerType, Exception exception)
        {
        }

        ContentType GetContentType()
        {
            object contentTypeHeader;
            if (TransportHeaders.TryGetHeader("Content-Type", out contentTypeHeader))
            {
                var contentType = contentTypeHeader as ContentType;
                if (contentType != null)
                    return contentType;
                var s = contentTypeHeader as string;
                if (s != null)
                    return new ContentType(s);
            }

            return DefaultContentType;
        }
    }
}