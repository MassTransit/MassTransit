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
namespace MassTransit.Reactive.Tests
{
    using System;
    using System.Reactive;
    using System.Threading.Tasks;
    using NUnit.Framework;
    using TestFramework;


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


        protected override void ConfigureInputQueueEndpoint(IInMemoryReceiveEndpointConfigurator configurator)
        {
            _observer = GetObserver<A>();

            configurator.Observer(_observer);

            configurator.Observer(Observer.Create<ConsumeContext<A>>(m => Console.WriteLine(m.Message.Name)));
        }
    }
}