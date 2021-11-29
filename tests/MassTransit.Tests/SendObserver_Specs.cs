namespace MassTransit.Tests
{
    namespace ObserverTests
    {
        using System;
        using System.Runtime.Serialization;
        using System.Threading;
        using System.Threading.Tasks;
        using Middleware;
        using NUnit.Framework;
        using Shouldly;
        using TestFramework;
        using TestFramework.Messages;
        using Util;


        [TestFixture]
        public class Connecting_a_send_observer_to_the_endpoint :
            InMemoryTestFixture
        {
            [Test]
            public async Task Should_invoke_the_exception_after_send_failure()
            {
                var observer = new SendObserver(this);
                using (InputQueueSendEndpoint.ConnectSendObserver(observer))
                {
                    Assert.That(async () => await InputQueueSendEndpoint.Send(new PingMessage(), Pipe.Execute<SendContext>(x => x.Serializer = null)),
                        Throws.TypeOf<SerializationException>());

                    await observer.PreSent;
                    await observer.SendFaulted;
                    Assert.That(observer.PreSentCount, Is.EqualTo(1));
                    Assert.That(observer.SendFaultCount, Is.EqualTo(1));
                }
            }

            [Test]
            public async Task Should_invoke_the_observer_after_send()
            {
                var observer = new SendObserver(this);
                using (InputQueueSendEndpoint.ConnectSendObserver(observer))
                {
                    await InputQueueSendEndpoint.Send(new PingMessage());

                    await observer.PreSent;
                    await observer.PostSent;

                    Assert.That(observer.PreSentCount, Is.EqualTo(1));
                    Assert.That(observer.PostSentCount, Is.EqualTo(1));
                }
            }

            [Test]
            public async Task Should_invoke_the_observer_prior_to_send()
            {
                var observer = new SendObserver(this);
                using (InputQueueSendEndpoint.ConnectSendObserver(observer))
                {
                    await InputQueueSendEndpoint.Send(new PingMessage());

                    await observer.PreSent;

                    Assert.That(observer.PreSentCount, Is.EqualTo(1));
                    Assert.That(observer.PostSentCount, Is.EqualTo(1));
                }
            }

            [Test]
            public async Task Should_not_invoke_post_sent_on_exception()
            {
                var observer = new SendObserver(this);
                using (InputQueueSendEndpoint.ConnectSendObserver(observer))
                {
                    Assert.That(async () => await InputQueueSendEndpoint.Send(new PingMessage(), Pipe.Execute<SendContext>(x => x.Serializer = null)),
                        Throws.TypeOf<SerializationException>());

                    await observer.SendFaulted;

                    observer.PostSent.Status.ShouldBe(TaskStatus.WaitingForActivation);

                    Assert.That(observer.PreSentCount, Is.EqualTo(1));
                    Assert.That(observer.PostSentCount, Is.EqualTo(0));
                    Assert.That(observer.SendFaultCount, Is.EqualTo(1));
                }
            }
        }


        [TestFixture]
        public class An_observer_on_an_endpoint_with_response :
            InMemoryTestFixture
        {
            [Test]
            public async Task Should_invoke_the_observer_after_send()
            {
                var observer = new SendObserver(this);
                using (Bus.ConnectSendObserver(observer))
                {
                    await InputQueueSendEndpoint.Send(new PingMessage(), context => context.ResponseAddress = BusAddress);

                    await observer.PreSent;
                    await observer.PostSent;
                    await observer.SendFaulted;

                    Assert.That(observer.PreSentCount, Is.EqualTo(2));
                    Assert.That(observer.PostSentCount, Is.EqualTo(1));
                    Assert.That(observer.SendFaultCount, Is.EqualTo(1));
                }
            }

            protected override void ConfigureInMemoryReceiveEndpoint(IInMemoryReceiveEndpointConfigurator configurator)
            {
                configurator.Handler<PingMessage>(async context =>
                {
                    try
                    {
                        await context.RespondAsync(new PongMessage(context.Message.CorrelationId), Pipe.Execute<SendContext>(x => x.Serializer = null));
                    }
                    catch (Exception exception)
                    {
                        Console.WriteLine(exception);
                    }
                });
            }
        }


        [TestFixture]
        public class Connecting_a_send_observer_to_the_bus_A :
            InMemoryTestFixture
        {
            [Test]
            public async Task Should_not_invoke_post_sent_on_exception()
            {
                Assert.That(async () => await InputQueueSendEndpoint.Send(new PingMessage(), Pipe.Execute<SendContext>(x => x.Serializer = null)),
                    Throws.TypeOf<SerializationException>());

                await _observer.SendFaulted;

                _observer.PostSent.Status.ShouldBe(TaskStatus.WaitingForActivation);

                Assert.That(_observer.PreSentCount, Is.EqualTo(1));
                Assert.That(_observer.PostSentCount, Is.EqualTo(0));
                Assert.That(_observer.SendFaultCount, Is.EqualTo(1));
            }

            SendObserver _observer;

            protected override void ConnectObservers(IBus bus)
            {
                base.ConnectObservers(bus);

                _observer = new SendObserver(this);

                bus.ConnectSendObserver(_observer);
            }
        }


        [TestFixture]
        public class Connecting_a_send_observer_to_the_bus_B :
            InMemoryTestFixture
        {
            [Test]
            public async Task Should_invoke_the_observer_prior_to_send()
            {
                await InputQueueSendEndpoint.Send(new PingMessage());

                await _observer.PreSent;
            }

            SendObserver _observer;

            protected override void ConnectObservers(IBus bus)
            {
                base.ConnectObservers(bus);

                _observer = new SendObserver(this);

                bus.ConnectSendObserver(_observer);
            }
        }


        [TestFixture]
        public class Connecting_a_send_observer_to_the_bus_C :
            InMemoryTestFixture
        {
            [Test]
            public async Task Should_invoke_the_observer_after_send()
            {
                await InputQueueSendEndpoint.Send(new PingMessage());

                await _observer.PostSent;
            }

            SendObserver _observer;

            protected override void ConnectObservers(IBus bus)
            {
                base.ConnectObservers(bus);

                _observer = new SendObserver(this);

                bus.ConnectSendObserver(_observer);
            }
        }


        [TestFixture]
        public class Connecting_a_send_observer_to_the_bus_D :
            InMemoryTestFixture
        {
            [Test]
            public async Task Should_invoke_the_exception_after_send_failure()
            {
                Assert.That(async () => await InputQueueSendEndpoint.Send(new PingMessage(), Pipe.Execute<SendContext>(x => x.Serializer = null)),
                    Throws.TypeOf<SerializationException>());

                await _observer.SendFaulted;
            }

            SendObserver _observer;

            protected override void ConnectObservers(IBus bus)
            {
                base.ConnectObservers(bus);

                _observer = new SendObserver(this);

                bus.ConnectSendObserver(_observer);
            }
        }


        [TestFixture]
        [Category("Unit")]
        public class Observing_sent_messages_with_mediator :
            InMemoryTestFixture
        {
            [Test]
            public async Task Should_trigger_the_send_message_observer()
            {
                var observer = new SendObserver(this);

                var mediator = MassTransit.Bus.Factory.CreateMediator(cfg =>
                {
                });

                mediator.ConnectSendObserver(observer);

                TaskCompletionSource<ConsumeContext<PingMessage>> received = GetTask<ConsumeContext<PingMessage>>();

                var handle = mediator.ConnectHandler<PingMessage>(x =>
                {
                    received.SetResult(x);

                    return Task.CompletedTask;
                });

                await mediator.Send(new PingMessage());

                await received.Task;

                handle.Disconnect();

                await observer.PreSent;
                await observer.PostSent;

                Assert.That(observer.PreSentCount, Is.EqualTo(1));
                Assert.That(observer.PostSentCount, Is.EqualTo(1));
            }

            [Test]
            public async Task Should_trigger_the_send_message_observer_for_both_messages()
            {
                var observer = new SendObserver(this);

                var mediator = MassTransit.Bus.Factory.CreateMediator(cfg =>
                {
                });

                mediator.ConnectSendObserver(observer);

                TaskCompletionSource<ConsumeContext<PingMessage>> received = GetTask<ConsumeContext<PingMessage>>();

                var handle = mediator.ConnectHandler<PingMessage>(x =>
                {
                    received.SetResult(x);

                    return x.RespondAsync(new PongMessage());
                });

                await mediator.Send(new PingMessage());

                await received.Task;

                handle.Disconnect();

                await observer.PreSent;
                await observer.PostSent;

                Assert.That(observer.PreSentCount, Is.EqualTo(2));
                Assert.That(observer.PostSentCount, Is.EqualTo(2));
            }
        }


        class SendObserver :
            ISendObserver
        {
            readonly TaskCompletionSource<SendContext> _postSend;
            readonly TaskCompletionSource<SendContext> _preSend;
            readonly TaskCompletionSource<SendContext> _sendFaulted;
            int _postSentCount;

            int _preSentCount;
            int _sendFaultCount;

            public SendObserver(AsyncTestFixture fixture)
            {
                _postSend = fixture.GetTask<SendContext>();
                _preSend = fixture.GetTask<SendContext>();
                _sendFaulted = fixture.GetTask<SendContext>();
            }

            public int PreSentCount => _preSentCount;
            public int PostSentCount => _postSentCount;
            public int SendFaultCount => _sendFaultCount;

            public Task<SendContext> PreSent => _preSend.Task;

            public Task<SendContext> PostSent => _postSend.Task;

            public Task<SendContext> SendFaulted => _sendFaulted.Task;

            public async Task PreSend<T>(SendContext<T> context)
                where T : class
            {
                Interlocked.Increment(ref _preSentCount);
                _preSend.TrySetResult(context);
            }

            public async Task PostSend<T>(SendContext<T> context)
                where T : class
            {
                Interlocked.Increment(ref _postSentCount);
                _postSend.TrySetResult(context);
            }

            public async Task SendFault<T>(SendContext<T> context, Exception exception)
                where T : class
            {
                Interlocked.Increment(ref _sendFaultCount);
                _sendFaulted.TrySetResult(context);
            }
        }
    }
}
