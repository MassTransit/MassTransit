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
namespace MassTransit.RabbitMqTransport.Configuration.Builders
{
    using System.Collections.Generic;
    using MassTransit.Pipeline;
    using Topology;
    using Transports;


    public class RabbitMqReceiveEndpointBuilder :
        IRabbitMqReceiveEndpointBuilder
    {
        readonly IConsumePipe _consumePipe;
        readonly List<ExchangeBindingSettings> _exchangeBindings;
        readonly IMessageNameFormatter _messageNameFormatter;
        readonly IExchangeTypeDeterminer _exchangeTypeDeterminer;
        readonly IRoutingkeyFormatter _routingkeyFormatter;
        readonly bool _bindMessageExchanges;

        public RabbitMqReceiveEndpointBuilder(IConsumePipe consumePipe, IMessageNameFormatter messageNameFormatter, IExchangeTypeDeterminer exchangeTypeDeterminer, IRoutingkeyFormatter routingkeyFormatter ,bool bindMessageExchanges)
        {
            _consumePipe = consumePipe;
            _messageNameFormatter = messageNameFormatter;
            _exchangeTypeDeterminer = exchangeTypeDeterminer;
            _routingkeyFormatter = routingkeyFormatter;
            _bindMessageExchanges = bindMessageExchanges;

            _exchangeBindings = new List<ExchangeBindingSettings>();
        }

        ConnectHandle IConsumePipeConnector.ConnectConsumePipe<T>(IPipe<ConsumeContext<T>> pipe)
        {
            if(_bindMessageExchanges)
                _exchangeBindings.AddRange(typeof(T).GetExchangeBindings(_messageNameFormatter, _exchangeTypeDeterminer, _routingkeyFormatter));

            return _consumePipe.ConnectConsumePipe(pipe);
        }

        ConnectHandle IConsumeMessageObserverConnector.ConnectConsumeMessageObserver<T>(IConsumeMessageObserver<T> observer)
        {
            return _consumePipe.ConnectConsumeMessageObserver(observer);
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