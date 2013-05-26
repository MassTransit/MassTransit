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
namespace MassTransit.Containers.Tests.Scenarios
{
    using System.Linq;
    using System.Threading;
    using Magnum.Extensions;
    using Magnum.TestFramework;
    using Testing;


    [Scenario]
    public abstract class When_registering_a_consumer :
        Given_a_service_bus_instance
    {
        [When]
        public void Registering_a_consumer()
        {
        }

        [Then]
        public void Should_have_a_subscription_for_the_consumer_message_type()
        {
            LocalBus.HasSubscription<SimpleMessageInterface>().Count()
                    .ShouldEqual(1, "No subscription for the SimpleMessageInterface was found.");
        }

        [Then]
        public void Should_have_a_subscription_for_the_nested_consumer_type()
        {
            LocalBus.HasSubscription<AnotherMessageInterface>().Count()
                    .ShouldEqual(1, "Only one subscription should be registered for another consumer");
        }

        [Then]
        public void Should_receive_using_the_first_consumer()
        {
            const string name = "Joe";

            var complete = new ManualResetEvent(false);

            LocalBus.SubscribeHandler<SimpleMessageClass>(x => complete.Set());
            LocalBus.Publish(new SimpleMessageClass(name));

            complete.WaitOne(8.Seconds());

            SimpleConsumer lastConsumer = SimpleConsumer.LastConsumer;
            lastConsumer.ShouldNotBeNull();

            lastConsumer.Last.Name
                        .ShouldEqual(name);

            lastConsumer.Dependency.WasDisposed
                        .ShouldBeTrue("Dependency was not disposed");
            lastConsumer.Dependency.SomethingDone
                        .ShouldBeTrue("Dependency was disposed before consumer executed");

        }
    }
}