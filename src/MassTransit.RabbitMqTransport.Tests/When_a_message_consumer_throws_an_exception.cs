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
    using System;
    using System.Threading.Tasks;
    using Configuration;
    using NUnit.Framework;
    using Shouldly;
    using TestFramework;


    public class When_a_message_consumer_throws_an_exception :
        RabbitMqTestFixture
    {
        [Test]
        public async void Should_be_received_by_the_handler()
        {
            Task<ConsumeContext<Fault<A>>> faultHandled = SubscribeHandler<Fault<A>>();

            _message = new A
            {
                StringA = "ValueA",
            };

            await InputQueueSendEndpoint.Send(_message);

            ConsumeContext<Fault<A>> fault = await faultHandled;

            fault.Message.Message.StringA.ShouldBe("ValueA");
        }

        [Test]
        public void Should_have_a_copy_of_the_error_in_the_error_queue()
        {
//            _received.WaitUntilCompleted(Debugger.IsAttached ? 5.Minutes() : 8.Seconds());
            //          LocalBus.GetEndpoint(LocalErrorUri).ShouldContain(_message, Debugger.IsAttached ? 5.Minutes() : 8.Seconds());
        }

        TaskCompletionSource<A> _received;
        A _message;
        TaskCompletionSource<Fault<A>> _faultReceived;

        protected override void ConfigureInputQueueEndpoint(IRabbitMqReceiveEndpointConfigurator configurator)
        {
            _received = GetTask<A>();

            Handler<A>(configurator, async context =>
            {
                _received.TrySetResult(context.Message);

                throw new IntentionalTestException("This is supposed to happen");
            });
        }


        class A :
            CorrelatedBy<Guid>
        {
            public string StringA { get; set; }

            public Guid CorrelationId
            {
                get { return Guid.Empty; }
            }
        }
    }
}