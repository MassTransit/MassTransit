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
    using Context;
    using RabbitMQ.Client;
    using Transports;


    public class RabbitMqReceiveContext :
        ReceiveContextBase,
        RabbitMqBasicConsumeContext
    {
        readonly byte[] _body;
        readonly string _consumerTag;
        readonly ulong _deliveryTag;
        readonly string _exchange;
        readonly IBasicProperties _properties;
        readonly string _routingKey;

        public RabbitMqReceiveContext(Uri inputAddress, string exchange, string routingKey, string consumerTag, ulong deliveryTag,
            byte[] body, bool redelivered, IBasicProperties properties)
            : base(inputAddress, redelivered)
        {
            _exchange = exchange;
            _routingKey = routingKey;
            _consumerTag = consumerTag;
            _deliveryTag = deliveryTag;
            _body = body;
            _properties = properties;

            ((ReceiveContext)this).GetOrAddPayload<RabbitMqBasicConsumeContext>(() => this);
        }

        protected override IContextHeaderProvider HeaderProvider
        {
            get { return new RabbitMqContextHeaderProvider(this); }
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

        protected override Stream GetBodyStream()
        {
            return new MemoryStream(_body, false);
        }
    }
}