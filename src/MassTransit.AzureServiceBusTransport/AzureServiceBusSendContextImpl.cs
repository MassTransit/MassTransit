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
    using System.IO;
    using System.Net.Mime;
    using System.Runtime.Serialization;
    using System.Threading;
    using Context;
    using Serialization;
    using Transports;


    public class AzureServiceBusSendContextImpl<T> :
        AzureServiceBusSendContext<T>
        where T : class
    {
        readonly CancellationToken _cancellationToken;
        readonly PayloadCache _payloadCache;
        byte[] _body;
        IMessageSerializer _serializer;

        public AzureServiceBusSendContextImpl(T message, CancellationToken cancellationToken)
        {
            _payloadCache = new PayloadCache();
            _payloadCache.GetOrAddPayload<AzureServiceBusSendContext<T>>(() => this);

            ContextHeaders = new AzureServiceBusSendContextHeaders();

            Message = message;
            _cancellationToken = cancellationToken;

            MessageId = NewId.NextGuid();

            Durable = true;
        }

        public Guid? MessageId { get; set; }
        public Guid? RequestId { get; set; }
        public Guid? CorrelationId { get; set; }

        public SendContextHeaders ContextHeaders { get; set; }

        public Uri SourceAddress { get; set; }
        public Uri DestinationAddress { get; set; }
        public Uri ResponseAddress { get; set; }
        public Uri FaultAddress { get; set; }

        public TimeSpan? TimeToLive { get; set; }

        public ContentType ContentType { get; set; }

        public IMessageSerializer Serializer
        {
            get { return _serializer; }
            set
            {
                _serializer = value;
                ContentType = _serializer.ContentType;
            }
        }

        public bool Durable { get; set; }
        public T Message { get; private set; }

        public CancellationToken CancellationToken
        {
            get { return _cancellationToken; }
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

        public Stream GetBodyStream()
        {
            if (_body != null)
                return new MemoryStream(_body);

            if (_serializer == null)
                throw new SerializationException("No serializer specified");
            if (Message == null)
                throw new SendException(typeof(T), DestinationAddress, "No message specified");

            var memoryStream = new MemoryStream();
            try
            {
                _serializer.Serialize(memoryStream, this);

                _body = memoryStream.ToArray();

                memoryStream.Position = 0;
                return memoryStream;
            }
            catch (Exception)
            {
                memoryStream.Dispose();
                throw;
            }
        }
    }
}