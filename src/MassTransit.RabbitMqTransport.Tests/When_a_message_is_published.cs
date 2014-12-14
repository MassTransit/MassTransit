// Copyright 2007-2014 Chris Patterson, Dru Sellers, Travis Smith, et. al.
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
    using System.Threading.Tasks;
    using Configuration;
    using NUnit.Framework;
    using NUnit.Framework;
    using Shouldly;


    [TestFixture]
    public class When_a_message_is_published_between_buses :
        RabbitMqTestFixture
    {
        [Test]
        public async void Should_be_received_by_the_queue()
        {
            ConsumeContext<A> context = await _received;

            context.Message.StringA.ShouldBe("ValueA");
        }

        [Test]
        public async void Should_receive_the_inherited_version()
        {
            ConsumeContext<B> context = await _receivedB;

            context.Message.StringB.ShouldBe("ValueB");
        }

        Task<ConsumeContext<A>> _received;
        Task<ConsumeContext<B>> _receivedB;

        protected override void ConfigureInputQueueEndpoint(IRabbitMqReceiveEndpointConfigurator configurator)
        {
            _received = Handler<A>(configurator);
            _receivedB = Handler<B>(configurator);
        }

        [TestFixtureSetUp]
        public void A_message_is_published()
        {
            InputQueueSendEndpoint.Send(new A
            {
                StringA = "ValueA",
                StringB = "ValueB",
            })
                .Wait(TestCancellationToken);
        }


        class A :
            B
        {
            public string StringA { get; set; }
        }


        class B
        {
            public string StringB { get; set; }
        }
    }
}