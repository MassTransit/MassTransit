// Copyright 2007-2017 Chris Patterson, Dru Sellers, Travis Smith, et. al.
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
namespace MassTransit.RabbitMqTransport.Topology.Configuration.Specifications
{
    using System.Collections.Generic;
    using Builders;
    using Configurators;
    using GreenPipes;


    /// <summary>
    /// Used to bind an exchange to the consuming queue's exchange
    /// </summary>
    public class ExchangeBindingConsumeTopologySpecification :
        IRabbitMqConsumeTopologySpecification
    {
        readonly string _exchangeName;
        readonly string _exchangeType;
        readonly bool _durable;
        readonly bool _autoDelete;
        readonly IDictionary<string, object> _exchangeArguments;
        readonly string _routingKey;
        readonly IDictionary<string, object> _bindingArguments;

        public ExchangeBindingConsumeTopologySpecification(ExchangeBindingConfigurator configurator)
        {
            _exchangeName = configurator.ExchangeName;
            _exchangeType = configurator.ExchangeType;
            _durable = configurator.Durable;
            _autoDelete = configurator.AutoDelete;
            _exchangeArguments = configurator.ExchangeArguments;
            _routingKey = configurator.RoutingKey;
            _bindingArguments = configurator.BindingArguments;
        }

        public IEnumerable<ValidationResult> Validate()
        {
            yield break;
        }

        public void Apply(IReceiveEndpointBrokerTopologyBuilder builder)
        {
            var exchangeHandle = builder.ExchangeDeclare(_exchangeName, _exchangeType, _durable, _autoDelete, _exchangeArguments);

            var bindingHandle = builder.ExchangeBind(exchangeHandle, builder.Exchange, _routingKey, _bindingArguments);
        }
    }
}