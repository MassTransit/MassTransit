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
namespace MassTransit.RabbitMqTransport.Tests
{
    using System;
    using System.Threading.Tasks;
    using ConsumerBind_Specs;
    using NUnit.Framework;
    using RabbitMQ.Client;


    [TestFixture]
    public class Binding_an_additional_exchange :
        ConsumerBindingTestFixture
    {
        [Test]
        public async Task Should_deliver_the_message()
        {
            var endpoint = await Bus.GetSendEndpoint(_host.Topology.GetDestinationAddress(_boundExchange));

            await endpoint.Send(new A());

            await _handled;
        }

        Task<ConsumeContext<A>> _handled;
        IRabbitMqHost _host;
        string _boundExchange = "bound-exchange";

        protected override void ConfigureRabbitMqBusHost(IRabbitMqBusFactoryConfigurator configurator, IRabbitMqHost host)
        {
            base.ConfigureRabbitMqBusHost(configurator, host);

            _host = host;
        }

        protected override void ConfigureRabbitMqReceiveEndpoint(IRabbitMqReceiveEndpointConfigurator configurator)
        {
            _handled = Handled<A>(configurator);

            configurator.Bind(_boundExchange);
        }

        protected override void OnCleanupVirtualHost(IModel model)
        {
            base.OnCleanupVirtualHost(model);

            model.ExchangeDelete(_boundExchange);
        }
    }


    [TestFixture]
    public class Binding_an_additional_exchange_with_routing_key :
        ConsumerBindingTestFixture
    {
        [Test]
        public async Task Should_deliver_the_message()
        {
            Uri sendAddress = _host.Topology.GetDestinationAddress(BoundExchange, x =>
            {
                x.Durable = false;
                x.AutoDelete = true;
                x.ExchangeType = ExchangeType.Direct;
            });

            var endpoint = await Bus.GetSendEndpoint(sendAddress);

            await endpoint.Send(new A(), x => x.SetRoutingKey("bondage"));

            await _handled;
        }

        Task<ConsumeContext<A>> _handled;
        IRabbitMqHost _host;
        const string BoundExchange = "bound-exchange";

        protected override void ConfigureRabbitMqBusHost(IRabbitMqBusFactoryConfigurator configurator, IRabbitMqHost host)
        {
            base.ConfigureRabbitMqBusHost(configurator, host);

            _host = host;
        }

        protected override void ConfigureRabbitMqReceiveEndpoint(IRabbitMqReceiveEndpointConfigurator configurator)
        {
            _handled = Handled<A>(configurator);

            configurator.Bind(BoundExchange, x =>
            {
                x.Durable = false;
                x.AutoDelete = true;
                x.ExchangeType = ExchangeType.Direct;
                x.RoutingKey = "bondage";
            });
        }

        protected override void OnCleanupVirtualHost(IModel model)
        {
            base.OnCleanupVirtualHost(model);

            model.ExchangeDelete(BoundExchange);
        }
    }
}