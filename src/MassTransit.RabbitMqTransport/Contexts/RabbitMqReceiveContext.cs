// Copyright 2007-2016 Chris Patterson, Dru Sellers, Travis Smith, et. al.
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
    using MassTransit.Topology;
    using RabbitMQ.Client;


    public class RabbitMqReceiveContext :
        BaseReceiveContext,
        RabbitMqBasicConsumeContext
    {
        readonly byte[] _body;

        public RabbitMqReceiveContext(Uri inputAddress, string exchange, string routingKey, string consumerTag, ulong deliveryTag, byte[] body, bool redelivered,
            IBasicProperties properties, IReceiveObserver observer, IReceiveEndpointTopology topology)
            : base(inputAddress, redelivered, observer, topology)
        {
            Exchange = exchange;
            RoutingKey = routingKey;
            ConsumerTag = consumerTag;
            DeliveryTag = deliveryTag;
            _body = body;
            Properties = properties;

            ((ReceiveContext)this).GetOrAddPayload<RabbitMqBasicConsumeContext>(() => this);
        }

        protected override IHeaderProvider HeaderProvider => new RabbitMqHeaderProvider(this);
        public string ConsumerTag { get; }
        public ulong DeliveryTag { get; }
        public string Exchange { get; }
        public string RoutingKey { get; }
        public IBasicProperties Properties { get; }

        protected override Stream GetBodyStream()
        {
            return new MemoryStream(_body, false);
        }
    }
}