namespace MassTransit.RabbitMqTransport.Tests
{
    namespace ObserverTests
    {
        using System;
        using System.Runtime.Serialization;
        using System.Threading;
        using System.Threading.Tasks;
        using Internals;
        using NUnit.Framework;
        using Shouldly;
        using TestFramework;
        using TestFramework.Messages;


        [TestFixture]
        public class Connecting_to_the_publish_observer_bus :
            RabbitMqTestFixture
        {
            [Test]
            public async Task Should_invoke_the_exception_after_send_failure()
            {
                var observer = new PublishObserver(this);
                using (Bus.ConnectPublishObserver(observer))
                {
                    Assert.That(
                        async () => await Bus.Publish(new PingMessage(), Pipe.Execute<SendContext>(x => x.Serializer = null)),
                        Throws.TypeOf<SerializationException>());

                    await observer.SendFaulted;
                }
            }

            [Test]
            public async Task Should_invoke_the_observer_after_send()
            {
                var observer = new PublishObserver(this);
                using (Bus.ConnectPublishObserver(observer))
                {
                    await Bus.Publish(new PingMessage());

                    await observer.PostSent;
                }
            }

            [Test]
            public async Task Should_invoke_the_observer_prior_to_send()
            {
                var observer = new PublishObserver(this);
                using (Bus.ConnectPublishObserver(observer))
                {
                    await Bus.Publish(new PingMessage());

                    await observer.PreSent;
                }
            }

            [Test]
            public async Task Should_not_invoke_post_sent_on_exception()
            {
                var observer = new PublishObserver(this);
                using (Bus.ConnectPublishObserver(observer))
                {
                    Assert.That(
                        async () => await Bus.Publish(new PingMessage(), Pipe.Execute<SendContext>(x => x.Serializer = null)),
                        Throws.TypeOf<SerializationException>());

                    await observer.SendFaulted;

                    observer.PostSent.Status.ShouldBe(TaskStatus.WaitingForActivation);
                }
            }

            [Test]
            public async Task Should_not_invoke_the_send_observer_prior_to_send()
            {
                var observer = new PublishObserver(this);
                using (Bus.ConnectPublishObserver(observer))
                {
                    var sendObserver = new SendObserver(this);
                    using (Bus.ConnectSendObserver(sendObserver))
                    {
                        await Bus.Publish(new PingMessage());

                        await observer.PreSent;

                        Assert.That(async () => await sendObserver.PreSent.OrTimeout(s: 5), Throws.TypeOf<TimeoutException>());
                    }
                }
            }
        }


        class PublishObserver :
            IPublishObserver
        {
            readonly TaskCompletionSource<PublishContext> _postSend;
            readonly TaskCompletionSource<PublishContext> _preSend;
            readonly TaskCompletionSource<PublishContext> _sendFaulted;

            public PublishObserver(AsyncTestFixture fixture)
            {
                _postSend = fixture.GetTask<PublishContext>();
                _preSend = fixture.GetTask<PublishContext>();
                _sendFaulted = fixture.GetTask<PublishContext>();
            }

            public Task<PublishContext> PreSent => _preSend.Task;
            public Task<PublishContext> PostSent => _postSend.Task;
            public Task<PublishContext> SendFaulted => _sendFaulted.Task;

            public async Task PrePublish<T>(PublishContext<T> context)
                where T : class
            {
                _preSend.TrySetResult(context);
            }

            public async Task PostPublish<T>(PublishContext<T> context)
                where T : class
            {
                _postSend.TrySetResult(context);
            }

            public async Task PublishFault<T>(PublishContext<T> context, Exception exception)
                where T : class
            {
                _sendFaulted.TrySetResult(context);
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


        [TestFixture]
        public class Connecting_a_send_observer_to_the_endpoint :
            RabbitMqTestFixture
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
    }
}
