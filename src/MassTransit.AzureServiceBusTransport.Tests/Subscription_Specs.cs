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
namespace MassTransit.AzureServiceBusTransport.Tests
{
    namespace SubscriptionTests
    {
        using System.Threading.Tasks;
        using Configuration;
        using NUnit.Framework;


        [TestFixture]
        public class Using_a_subscription_endpoint :
            AzureServiceBusTestFixture
        {
            [Test]
            public async Task Should_succeed()
            {
                await Bus.Publish(new MessageA());
                await Bus.Publish(new MessageB());

                await _handledA;
                await _handledB;
            }

            Task<ConsumeContext<MessageA>> _handledA;
            Task<ConsumeContext<MessageB>> _handledB;

            protected override void ConfigureServiceBusReceiveEndpoint(IServiceBusReceiveEndpointConfigurator configurator)
            {
                _handledA = Handled<MessageA>(configurator);
            }

            protected override void ConfigureServiceBusBusHost(IServiceBusBusFactoryConfigurator configurator, IServiceBusHost host)
            {
                base.ConfigureServiceBusBusHost(configurator, host);

                configurator.SubscriptionEndpoint<MessageB>(host, "phatboyg_you_know_me", x =>
                {
                    _handledB = Handled<MessageB>(x);
                });
            }
        }


        public class MessageA
        {
            public string Value { get; set; }
        }


        public class MessageB
        {
            public string Value { get; set; }
        }


        public class ResponseA
        {
            public string Value { get; set; }
        }


        public class ResponseB
        {
            public string Value { get; set; }
        }


        [TestFixture]
        public class Using_multiple_subscription_endpoints :
            AzureServiceBusTestFixture
        {
            [Test]
            public async Task Should_handle_a_message_per_endpoint()
            {
                _responseA = SubscribeHandler<ResponseA>();
                _responseB = SubscribeHandler<ResponseB>();

                await Bus.Publish(new MessageA(), context => context.ResponseAddress = Bus.Address);
                await Bus.Publish(new MessageB(), context => context.ResponseAddress = Bus.Address);

                await _responseA;
                await _responseB;
            }

            Task<ConsumeContext<ResponseA>> _responseA;
            Task<ConsumeContext<ResponseB>> _responseB;

            protected override void ConfigureServiceBusBusHost(IServiceBusBusFactoryConfigurator configurator, IServiceBusHost host)
            {
                base.ConfigureServiceBusBusHost(configurator, host);

                configurator.SubscriptionEndpoint<MessageA>(host, "phatboyg_you_know_me", x =>
                {
                    x.Consumer<Consumer>();
                });

                configurator.SubscriptionEndpoint<MessageB>(host, "phatboyg_you_know_me", x =>
                {
                    x.Consumer<Consumer>();
                });
            }


            public class Consumer :
                IConsumer<MessageA>,
                IConsumer<MessageB>
            {
                public Task Consume(ConsumeContext<MessageA> context)
                {
                    return context.RespondAsync(new ResponseA {Value = context.Message.Value});
                }

                public Task Consume(ConsumeContext<MessageB> context)
                {
                    return context.RespondAsync(new ResponseB {Value = context.Message.Value});
                }
            }
        }
    }
}