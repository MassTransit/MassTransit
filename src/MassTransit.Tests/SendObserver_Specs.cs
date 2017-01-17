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
namespace MassTransit.Tests
{
    using System;
    using System.Runtime.Serialization;
    using System.Threading.Tasks;
    using NUnit.Framework;
    using Shouldly;
    using TestFramework;
    using TestFramework.Messages;

    namespace ObserverTests
    {
        using GreenPipes;


        [TestFixture]
        public class Connecting_a_send_observer_to_the_endpoint :
            InMemoryTestFixture
        {
            [Test]
            public async Task Should_invoke_the_exception_after_send_failure()
            {
                var observer = new Observer();
                using (InputQueueSendEndpoint.ConnectSendObserver(observer))
                {
                    Assert.That(
                        async () => await InputQueueSendEndpoint.Send(new PingMessage(), Pipe.Execute<SendContext>(x => x.Serializer = null)),
                        Throws.TypeOf<SerializationException>());

                    await observer.SendFaulted;
                }
            }

            [Test]
            public async Task Should_invoke_the_observer_after_send()
            {
                var observer = new Observer();
                using (InputQueueSendEndpoint.ConnectSendObserver(observer))
                {
                    await InputQueueSendEndpoint.Send(new PingMessage());

                    await observer.PostSent;
                }
            }

            [Test]
            public async Task Should_invoke_the_observer_prior_to_send()
            {
                var observer = new Observer();
                using (InputQueueSendEndpoint.ConnectSendObserver(observer))
                {
                    await InputQueueSendEndpoint.Send(new PingMessage());

                    await observer.PreSent;
                }
            }

            [Test]
            public async Task Should_not_invoke_post_sent_on_exception()
            {
                var observer = new Observer();
                using (InputQueueSendEndpoint.ConnectSendObserver(observer))
                {
                    Assert.That(async () => await InputQueueSendEndpoint.Send(new PingMessage(), Pipe.Execute<SendContext>(x => x.Serializer = null)),
                        Throws.TypeOf<SerializationException>());

                    await observer.SendFaulted;

                    observer.PostSent.Status.ShouldBe(TaskStatus.WaitingForActivation);
                }
            }
        }

        [TestFixture]
        public class Connecting_a_send_observer_to_the_bus_A :
            InMemoryTestFixture
        {
            Observer _observer;

            protected override void ConnectObservers(IBus bus)
            {
                base.ConnectObservers(bus);

                _observer = new Observer();

                bus.ConnectSendObserver(_observer);
            }

            [Test]
            public async Task Should_not_invoke_post_sent_on_exception()
            {
                    Assert.That(async () => await InputQueueSendEndpoint.Send(new PingMessage(), Pipe.Execute<SendContext>(x => x.Serializer = null)),
                        Throws.TypeOf<SerializationException>());

                    await _observer.SendFaulted;

                _observer.PostSent.Status.ShouldBe(TaskStatus.WaitingForActivation);
            }
        }
        [TestFixture]
        public class Connecting_a_send_observer_to_the_bus_B :
            InMemoryTestFixture
        {
            Observer _observer;

            protected override void ConnectObservers(IBus bus)
            {
                base.ConnectObservers(bus);

                _observer = new Observer();

                bus.ConnectSendObserver(_observer);
            }
            [Test]
            public async Task Should_invoke_the_observer_prior_to_send()
            {
                    await InputQueueSendEndpoint.Send(new PingMessage());

                    await _observer.PreSent;
            }

        }
        [TestFixture]
        public class Connecting_a_send_observer_to_the_bus_C :
            InMemoryTestFixture
        {
            Observer _observer;

            protected override void ConnectObservers(IBus bus)
            {
                base.ConnectObservers(bus);

                _observer = new Observer();

                bus.ConnectSendObserver(_observer);
            }
            

            [Test]
            public async Task Should_invoke_the_observer_after_send()
            {
                    await InputQueueSendEndpoint.Send(new PingMessage());

                    await _observer.PostSent;
            }
            
        }

        [TestFixture]
        public class Connecting_a_send_observer_to_the_bus_D :
            InMemoryTestFixture
        {
            Observer _observer;

            protected override void ConnectObservers(IBus bus)
            {
                base.ConnectObservers(bus);

                _observer = new Observer();

                bus.ConnectSendObserver(_observer);
            }

            [Test]
            public async Task Should_invoke_the_exception_after_send_failure()
            {
                    Assert.That(
                        async () => await InputQueueSendEndpoint.Send(new PingMessage(), Pipe.Execute<SendContext>(x => x.Serializer = null)),
                        Throws.TypeOf<SerializationException>());

                    await _observer.SendFaulted;
            }

        }


        class Observer :
            ISendObserver
        {
            readonly TaskCompletionSource<SendContext> _postSend = new TaskCompletionSource<SendContext>();
            readonly TaskCompletionSource<SendContext> _preSend = new TaskCompletionSource<SendContext>();
            readonly TaskCompletionSource<SendContext> _sendFaulted = new TaskCompletionSource<SendContext>();

            public Task<SendContext> PreSent
            {
                get { return _preSend.Task; }
            }

            public Task<SendContext> PostSent
            {
                get { return _postSend.Task; }
            }

            public Task<SendContext> SendFaulted
            {
                get { return _sendFaulted.Task; }
            }

            public async Task PreSend<T>(SendContext<T> context)
                where T : class
            {
                _preSend.TrySetResult(context);
            }

            public async Task PostSend<T>(SendContext<T> context)
                where T : class
            {
                _postSend.TrySetResult(context);
            }

            public async Task SendFault<T>(SendContext<T> context, Exception exception)
                where T : class
            {
                _sendFaulted.TrySetResult(context);
            }
        }
    }
}