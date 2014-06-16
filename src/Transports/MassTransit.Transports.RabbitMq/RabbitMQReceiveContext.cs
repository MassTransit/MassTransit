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
namespace MassTransit.Transports.RabbitMq
{
    using System;
    using System.Diagnostics;
    using System.IO;
    using System.Net.Mime;
    using System.Threading;
    using RabbitMQ.Client;


    public class RabbitMqReceiveContext :
        ReceiveContext,
        RabbitMqBasicConsumeContext
    {
        static readonly ContentType DefaultContentType = new ContentType("application/vnd.masstransit+json");

        readonly byte[] _body;
        readonly CancellationTokenSource _cancellationTokenSource;
        readonly string _consumerTag;
        readonly ulong _deliveryTag;
        readonly string _exchange;
        readonly Uri _inputAddress;
        readonly IBasicProperties _properties;
        readonly Stopwatch _receiveTimer;
        readonly bool _redelivered;
        readonly string _routingKey;

        ContentType _contentType;
        RabbitMqReceiveContextHeaders _headers;

        public RabbitMqReceiveContext(string exchange, string routingKey, string consumerTag, Uri inputAddress,
            ulong deliveryTag,
            byte[] body, bool redelivered, IBasicProperties properties)
        {
            _receiveTimer = Stopwatch.StartNew();

            _exchange = exchange;
            _routingKey = routingKey;
            _body = body;
            _redelivered = redelivered;
            _deliveryTag = deliveryTag;
            _properties = properties;
            _inputAddress = inputAddress;
            _consumerTag = consumerTag;

            _cancellationTokenSource = new CancellationTokenSource();
        }

        public string ConsumerTag
        {
            get { return _consumerTag; }
        }

        public ulong DeliveryTag
        {
            get { return _deliveryTag; }
        }

        public string Exchange
        {
            get { return _exchange; }
        }

        public string RoutingKey
        {
            get { return _routingKey; }
        }

        public IBasicProperties Properties
        {
            get { return _properties; }
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

        public bool Redelivered
        {
            get { return _redelivered; }
        }

        public ContextHeaders ContextHeaders
        {
            get { return _headers ?? (_headers = new RabbitMqReceiveContextHeaders(this)); }
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
            if (ContextHeaders.TryGetHeader("Content-Type", out contentTypeHeader))
            {
                var s = contentTypeHeader as string;
                if (s != null)
                    return new ContentType(s);
            }

            return DefaultContentType;
        }

        public void Cancel()
        {
            _cancellationTokenSource.Cancel();
        }
    }
}