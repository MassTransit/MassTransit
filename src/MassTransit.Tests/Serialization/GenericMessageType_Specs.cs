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
    using BusConfigurators;
    using Magnum.Extensions;
    using Magnum.TestFramework;
    using NUnit.Framework;
    using TextFixtures;

    [TestFixture]
    public class When_sending_a_generic_message :
        LoopbackTestFixture
    {
        FutureMessage<Message<int>> _called;

        public When_sending_a_generic_message()
        {
            _called = new FutureMessage<Message<int>>();
        }

        protected override void ConfigureLocalBus(ServiceBusConfigurator configurator)
        {
            configurator.Subscribe(x => x.Handler<Message<int>>((context,message) =>
                {
                    _called.Set(message);

                    context.Respond(new Message<string> {Body = message.Body.ToString()});
                }));
        }

        class Message<T>
        {
            public T Body { get; set; }
        }

        [Test]
        public void Should_call_the_message_handler()
        {
            var responded = new FutureMessage<Message<string>>();

            LocalBus.Endpoint.SendRequest(new Message<int> { Body = 42 }, LocalBus, x =>
                {
                    x.Handle<Message<string>>(responded.Set);
                });

            _called.IsAvailable(8.Seconds()).ShouldBeTrue();
            _called.Message.Body.ShouldEqual(42);

            responded.IsAvailable(8.Seconds()).ShouldBeTrue();
            responded.Message.Body.ShouldEqual("42");
        }

        [Test]
        public void Should_call_the_message_handler_again()
        {
            var responded = new FutureMessage<Message<string>>();

            LocalBus.Endpoint.SendRequest(new Message<int> { Body = 27 }, LocalBus, x =>
                {
                    x.Handle<Message<string>>(responded.Set);
                });

            responded.IsAvailable(8.Seconds()).ShouldBeTrue();
            responded.Message.Body.ShouldEqual("27");            
            
            responded = new FutureMessage<Message<string>>();

            LocalBus.Endpoint.SendRequest(new Message<int> { Body = 69 }, LocalBus, x =>
                {
                    x.Handle<Message<string>>(responded.Set);
                });

            responded.IsAvailable(8.Seconds()).ShouldBeTrue();
            responded.Message.Body.ShouldEqual("69");
        }
    }
}