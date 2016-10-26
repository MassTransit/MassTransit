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
namespace MassTransit.RabbitMqTransport.Builders
{
    using System.Collections.Generic;
    using GreenPipes;
    using MassTransit.Builders;
    using MassTransit.Pipeline;
    using Topology;
    using Transports;


    public class RabbitMqReceiveEndpointBuilder :
        ReceiveEndpointBuilder,
        IRabbitMqReceiveEndpointBuilder
    {
        readonly List<ExchangeBindingSettings> _exchangeBindings;
        readonly IMessageNameFormatter _messageNameFormatter;
        readonly bool _bindMessageExchanges;

        public RabbitMqReceiveEndpointBuilder(IConsumePipe consumePipe, IBusBuilder busBuilder, IMessageNameFormatter messageNameFormatter, bool bindMessageExchanges)
            : base(consumePipe, busBuilder)
        {
            _messageNameFormatter = messageNameFormatter;
            _bindMessageExchanges = bindMessageExchanges;

            _exchangeBindings = new List<ExchangeBindingSettings>();
        }

        public override ConnectHandle ConnectConsumePipe<T>(IPipe<ConsumeContext<T>> pipe)
        {
            if(_bindMessageExchanges)
                _exchangeBindings.AddRange(typeof(T).GetExchangeBindings(_messageNameFormatter));

            return base.ConnectConsumePipe(pipe);
        }

        public void AddExchangeBindings(params ExchangeBindingSettings[] bindings)
        {
            _exchangeBindings.AddRange(bindings);
        }

        public IEnumerable<ExchangeBindingSettings> GetExchangeBindings()
        {
            return _exchangeBindings;
        }
    }
}