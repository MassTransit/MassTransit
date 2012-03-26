// Copyright 2007-2011 Chris Patterson, Dru Sellers, Travis Smith, et. al.
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
namespace MassTransit.Tests.Serialization
{
    using System;
    using BusConfigurators;
    using Magnum.TestFramework;
    using MassTransit.Transports;
    using TestFramework;
    using TextFixtures;
    using Util;

    [Scenario]
    public class When_a_message_deserialization_exception_occurs
        : LoopbackTestFixture
    {
        protected override void ConfigureLocalBus(ServiceBusConfigurator configurator)
        {
            base.ConfigureLocalBus(configurator);

            configurator.Subscribe(s => { s.Handler<BadMessage>(x => { }); });
        }

        [Then]
        public void Should_put_message_in_error_queue()
        {
            LocalBus.Endpoint.Send(new BadMessage("Good"));

            IEndpoint errorEndpoint =
                LocalBus.GetEndpoint(LocalBus.Endpoint.InboundTransport.Address.Uri.AppendToPath("_error"));
            errorEndpoint.InboundTransport.ShouldContain(errorEndpoint.Serializer, typeof(BadMessage));

            LocalBus.Endpoint.ShouldNotContain<BadMessage>();

            var errorTransport = LocalBus.Endpoint.ErrorTransport as LoopbackTransport;
            errorTransport.ShouldNotBeNull();

            errorTransport.Count.ShouldEqual(1);
        }

        class BadMessage
        {
            public BadMessage()
            {
                throw new InvalidOperationException("I want to be bad.");
            }

            public BadMessage(string value)
            {
                Value = value;
            }

            public string Value { get; set; }
        }
    }
}