// Copyright 2007-2012 Chris Patterson, Dru Sellers, Travis Smith, et. al.
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
namespace MassTransit.Transports.RabbitMq.Tests
{
    using System;
    using System.Diagnostics;
    using BusConfigurators;
    using Magnum.Extensions;
    using Magnum.TestFramework;
    using TestFramework;


    [Scenario]
    public class When_a_message_consumer_throws_an_exception :
        Given_a_rabbitmq_bus
    {
        public When_a_message_consumer_throws_an_exception()
        {
            ConfigureEndpointFactory(x => x.SetDefaultRetryLimit(0));
        }

        Future<A> _received;
        A _message;
        Future<Fault<A>> _faultReceived;

        protected override void ConfigureServiceBus(Uri uri, ServiceBusConfigurator configurator)
        {
            base.ConfigureServiceBus(uri, configurator);

            _received = new Future<A>();
            _faultReceived = new Future<Fault<A>>();

            configurator.Subscribe(s =>
                {
                    s.Handler<A>(message =>
                        {
                            _received.Complete(message);

                            throw new NullReferenceException(
                                "This is supposed to happen, cause this handler is naughty.");
                        });

                    s.Handler<Fault<A, Guid>>(message => { _faultReceived.Complete(message); });
                });
        }

        [When]
        public void A_message_is_published()
        {
            _message = new A
                {
                    StringA = "ValueA",
                };

            LocalBus.Publish(_message);
        }

        [Then]
        public void Should_be_received_by_the_handler()
        {
            _received.WaitUntilCompleted(Debugger.IsAttached ? 5.Minutes() : 8.Seconds()).ShouldBeTrue();
            _received.Value.StringA.ShouldEqual("ValueA");
        }

        [Then]
        public void Should_receive_the_fault()
        {
            _faultReceived.WaitUntilCompleted(Debugger.IsAttached ? 5.Minutes() : 8.Seconds()).ShouldBeTrue();
            _faultReceived.Value.FailedMessage.StringA.ShouldEqual("ValueA");
        }

        [Then]
        public void Should_have_a_copy_of_the_error_in_the_error_queue()
        {
            _received.WaitUntilCompleted(Debugger.IsAttached ? 5.Minutes() : 8.Seconds());
            LocalBus.GetEndpoint(LocalErrorUri).ShouldContain(_message, Debugger.IsAttached ? 5.Minutes() : 8.Seconds());
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