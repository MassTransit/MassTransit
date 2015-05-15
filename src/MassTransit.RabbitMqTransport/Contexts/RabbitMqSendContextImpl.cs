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
namespace MassTransit.RabbitMqTransport.Contexts
{
    using System;
    using System.IO;
    using System.Net.Mime;
    using System.Runtime.Serialization;
    using System.Threading;
    using Context;
    using RabbitMQ.Client;


    public class RabbitMqSendContextImpl<T> :
        RabbitMqSendContext<T>
        where T : class
    {
        readonly PayloadCache _payloadCache;
        byte[] _body;
        IMessageSerializer _serializer;

        public RabbitMqSendContextImpl(IBasicProperties basicProperties, T message, SendSettings sendSettings,
            CancellationToken cancellationToken,
            string routingKey = "")
        {
            CancellationToken = cancellationToken;

            _payloadCache = new PayloadCache();

            // provide access to the extended settings for RabbitMQ developers
            _payloadCache.GetOrAddPayload<RabbitMqSendContext<T>>(() => this);
            _payloadCache.GetOrAddPayload<RabbitMqSendContext>(() => this);

            Headers = new RabbitMqSendHeaders(basicProperties);
            BasicProperties = basicProperties;
            Message = message;
            Exchange = sendSettings.ExchangeName;
            RoutingKey = routingKey;

            MessageId = NewId.NextGuid();

            Durable = true;
        }

        public CancellationToken CancellationToken { get; private set; }

        public Guid? MessageId { get; set; }
        public Guid? RequestId { get; set; }
        public Guid? CorrelationId { get; set; }

        public SendHeaders Headers { get; set; }

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
        public bool Immediate { get; set; }
        public bool Mandatory { get; set; }

        public string Exchange { get; private set; }
        public string RoutingKey { get; set; }

        public IBasicProperties BasicProperties { get; private set; }

        public byte[] Body
        {
            get
            {
                if (_body != null)
                    return _body;

                if (Serializer == null)
                    throw new SerializationException("No serializer specified");
                if (Message == null)
                    throw new SendException(typeof(T), DestinationAddress, "No message specified");

                using (var memoryStream = new MemoryStream())
                {
                    Serializer.Serialize(memoryStream, this);

                    _body = memoryStream.ToArray();
                    return _body;
                }
            }
        }

        public T Message { get; private set; }

        public bool HasPayloadType(Type contextType)
        {
            if (contextType.IsAssignableFrom(typeof(RabbitMqSendContextImpl<T>)))
                return true;

            return _payloadCache.HasPayloadType(contextType);
        }

        public bool TryGetPayload<TPayload>(out TPayload context)
            where TPayload : class
        {
            context = this as TPayload;
            if (context != null)
                return true;

            return _payloadCache.TryGetPayload(out context);
        }

        public TPayload GetOrAddPayload<TPayload>(PayloadFactory<TPayload> payloadFactory)
            where TPayload : class
        {
            var context = this as TPayload;
            if (context != null)
                return context;

            return _payloadCache.GetOrAddPayload(payloadFactory);
        }
    }
}