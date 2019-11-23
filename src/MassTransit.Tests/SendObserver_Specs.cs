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
        using Util;


        [TestFixture]
        public class Connecting_a_send_observer_to_the_endpoint :
            InMemoryTestFixture
        {
            [Test]
            public async Task Should_invoke_the_exception_after_send_failure()
            {
                var observer = new SendObserver();
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
                var observer = new SendObserver();
                using (InputQueueSendEndpoint.ConnectSendObserver(observer))
                {
                    await InputQueueSendEndpoint.Send(new PingMessage());

                    await observer.PostSent;
                }
            }

            [Test]
            public async Task Should_invoke_the_observer_prior_to_send()
            {
                var observer = new SendObserver();
                using (InputQueueSendEndpoint.ConnectSendObserver(observer))
                {
                    await InputQueueSendEndpoint.Send(new PingMessage());

                    await observer.PreSent;
                }
            }

            [Test]
            public async Task Should_not_invoke_post_sent_on_exception()
            {
                var observer = new SendObserver();
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
            SendObserver _observer;

            protected override void ConnectObservers(IBus bus)
            {
                base.ConnectObservers(bus);

                _observer = new SendObserver();

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
            SendObserver _observer;

            protected override void ConnectObservers(IBus bus)
            {
                base.ConnectObservers(bus);

                _observer = new SendObserver();

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
            SendObserver _observer;

            protected override void ConnectObservers(IBus bus)
            {
                base.ConnectObservers(bus);

                _observer = new SendObserver();

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
            SendObserver _observer;

            protected override void ConnectObservers(IBus bus)
            {
                base.ConnectObservers(bus);

                _observer = new SendObserver();

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


        class SendObserver :
            ISendObserver
        {
            readonly TaskCompletionSource<SendContext> _postSend = TaskUtil.GetTask<SendContext>();
            readonly TaskCompletionSource<SendContext> _preSend = TaskUtil.GetTask<SendContext>();
            readonly TaskCompletionSource<SendContext> _sendFaulted = TaskUtil.GetTask<SendContext>();

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
