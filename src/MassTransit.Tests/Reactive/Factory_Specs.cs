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
namespace MassTransit.Tests.Reactive
{
    using System;
    using System.Reactive;
    using System.Reactive.Linq;
    using System.Threading.Tasks;
    using MassTransit.Testing.Observers;
    using NUnit.Framework;
    using TestFramework;
    using Util;


    [TestFixture]
    public class Subscribing_from_the_service_bus_factory :
        InMemoryTestFixture
    {
        [Test]
        public async Task Should_allow_rx_subscribers()
        {
            await InputQueueSendEndpoint.Send(new A {Name = "Joe"});

            await _observer.Value;
        }

        TestObserver<A> _observer;


        class A
        {
            public string Name { get; set; }
        }


        protected override void ConfigureInMemoryReceiveEndpoint(IInMemoryReceiveEndpointConfigurator configurator)
        {
            _observer = GetObserver<A>();

            configurator.Observer(_observer);

            configurator.Observer(Observer.Create<ConsumeContext<A>>(m => Console.WriteLine(m.Message.Name)));
        }
    }


    [TestFixture]
    public class Making_a_receive_endpoint_observable :
        InMemoryTestFixture
    {
        [Test]
        public async Task Should_allow_rx_subscribers()
        {
            await InputQueueSendEndpoint.Send(new A {Name = "Joe"});
            await InputQueueSendEndpoint.Send(new A {Name = "Joe"});
            await InputQueueSendEndpoint.Send(new A {Name = "Frank"});
            
            await Task.Delay(1000);
        }


        class A
        {
            public string Name { get; set; }
        }


        ObservableObserver<ConsumeContext<A>> _observer;

        protected override void ConfigureInMemoryReceiveEndpoint(IInMemoryReceiveEndpointConfigurator configurator)
        {
            _observer = new ObservableObserver<ConsumeContext<A>>();

            _observer.GroupBy(x => x.Message.Name).Subscribe(value => Console.WriteLine("Key: {0}", value.Key));

            configurator.Observer(_observer);
        }
    }
}