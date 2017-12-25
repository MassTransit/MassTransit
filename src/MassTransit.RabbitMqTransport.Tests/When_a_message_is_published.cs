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
    using System.Threading.Tasks;
    using NUnit.Framework;
    using Shouldly;


    [TestFixture]
    public class When_a_message_is_published :
        RabbitMqTestFixture
    {
        Task<ConsumeContext<A>> _received;
        Task<ConsumeContext<B>> _receivedB;

        protected override void ConfigureRabbitMqReceiveEndpoint(IRabbitMqReceiveEndpointConfigurator configurator)
        {
            _received = Handled<A>(configurator);
            _receivedB = Handled<B>(configurator);
        }

        [OneTimeSetUp]
        public void A_message_is_published()
        {
            Bus.Publish(new A
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


        [Test]
        public async Task Should_be_received_by_the_queue()
        {
            ConsumeContext<A> context = await _received;

            context.Message.StringA.ShouldBe("ValueA");
        }

        [Test]
        public async Task Should_receive_the_inherited_version()
        {
            ConsumeContext<B> context = await _receivedB;

            context.Message.StringB.ShouldBe("ValueB");
        }
    }
}