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
        using System;
        using System.Threading.Tasks;
        using GreenPipes;
        using NUnit.Framework;
        using TestFramework.Messages;


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

            protected override void ConfigureInputQueueEndpoint(IServiceBusReceiveEndpointConfigurator configurator)
            {
                _handledA = Handled<MessageA>(configurator);
            }

            protected override void ConfigureBusHost(IServiceBusBusFactoryConfigurator configurator, IServiceBusHost host)
            {
                base.ConfigureBusHost(configurator, host);

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
    }
}