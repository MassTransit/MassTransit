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
namespace MassTransit.RabbitMqTransport.Builders
{
    using System;
    using System.Collections.Generic;
    using GreenPipes;
    using MassTransit.Builders;
    using MassTransit.Pipeline;
    using Topology;
    using Transport;
    using Transports;


    public class RabbitMqReceiveEndpointBuilder :
        ReceiveEndpointBuilder,
        IRabbitMqReceiveEndpointBuilder
    {
        readonly bool _bindMessageExchanges;
        readonly List<ExchangeBindingSettings> _exchangeBindings;
        readonly IRabbitMqHost _host;

        public RabbitMqReceiveEndpointBuilder(IConsumePipe consumePipe, IBusBuilder busBuilder, bool bindMessageExchanges, IRabbitMqHost host)
            : base(consumePipe, busBuilder)
        {
            _bindMessageExchanges = bindMessageExchanges;
            _host = host;

            _exchangeBindings = new List<ExchangeBindingSettings>();
        }

        public override ConnectHandle ConnectConsumePipe<T>(IPipe<ConsumeContext<T>> pipe)
        {
            if (_bindMessageExchanges)
                _exchangeBindings.AddRange(typeof(T).GetExchangeBindings(_host.Settings.MessageNameFormatter));

            return base.ConnectConsumePipe(pipe);
        }

        public void AddExchangeBindings(params ExchangeBindingSettings[] bindings)
        {
            _exchangeBindings.AddRange(bindings);
        }

        public ISendEndpointProvider CreateSendEndpointProvider(Uri sourceAddress, params ISendPipeSpecification[] specifications)
        {
            var pipe = CreateSendPipe(specifications);

            var provider = new RabbitMqSendEndpointProvider(MessageSerializer, sourceAddress, SendTransportProvider, pipe);

            return new SendEndpointCache(provider, CacheDurationProvider);
        }

        public IPublishEndpointProvider CreatePublishEndpointProvider(Uri sourceAddress, params IPublishPipeSpecification[] specifications)
        {
            var pipe = CreatePublishPipe(specifications);

            return new RabbitMqPublishEndpointProvider(_host, MessageSerializer, sourceAddress, pipe);
        }

        public TimeSpan CacheDurationProvider(Uri address)
        {
            if (address.GetReceiveSettings().AutoDelete)
                return TimeSpan.FromMinutes(1);

            return TimeSpan.FromDays(1);
        }

        public IEnumerable<ExchangeBindingSettings> GetExchangeBindings()
        {
            return _exchangeBindings;
        }
    }
}