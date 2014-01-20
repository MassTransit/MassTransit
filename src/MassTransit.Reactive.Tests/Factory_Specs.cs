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
namespace MassTransit.Reactive.Tests
{
    using System;
    using System.Reactive;
    using System.Threading.Tasks;
    using Magnum.Extensions;
    using Magnum.TestFramework;
    using NUnit.Framework;


    [TestFixture]
    public class Subscribing_from_the_service_bus_factory
    {
        class A
        {
            public string Name { get; set; }
        }


        class MyObserver<T> :
            IObserver<T>
        {
            readonly TaskCompletionSource<bool> _completed = new TaskCompletionSource<bool>();
            readonly TaskCompletionSource<Exception> _exception = new TaskCompletionSource<Exception>();
            readonly TaskCompletionSource<T> _value = new TaskCompletionSource<T>();

            public Task<T> Value
            {
                get { return _value.Task; }
            }


            public void OnNext(T value)
            {
                _value.TrySetResult(value);
            }

            public void OnError(Exception error)
            {
                _exception.TrySetException(error);
            }

            public void OnCompleted()
            {
                _completed.TrySetResult(true);
            }
        }


        [Test]
        public void Should_allow_rx_subscribers()
        {
            var observer = new MyObserver<A>();

            using (var bus = ServiceBusFactory.New(x =>
                {
                    x.ReceiveFrom("loopback://localhost/queue");

                    x.Subscribe(s =>
                        {
                            s.Observe(observer);
                            s.Observe(Observer.Create<A>(m => Console.WriteLine(m.Name)));
                        });
                }))
            {
                bus.Publish(new A {Name = "Joe"});

                observer.Value.Wait(8.Seconds()).ShouldBeTrue();
            }
        }
    }
}