namespace MassTransit.RabbitMqTransport.Tests
{
    using System;
    using System.Runtime.Serialization;
    using System.Threading.Tasks;
    using GreenPipes;
    using NUnit.Framework;
    using Shouldly;
    using TestFramework.Messages;


    namespace ObserverTests
    {
        using GreenPipes.Internals.Extensions;
        using Util;


        [TestFixture]
        public class Connecting_to_the_publish_observer_bus :
            RabbitMqTestFixture
        {
            [Test]
            public async Task Should_invoke_the_exception_after_send_failure()
            {
                var observer = new PublishObserver();
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
                var observer = new PublishObserver();
                using (Bus.ConnectPublishObserver(observer))
                {
                    await Bus.Publish(new PingMessage());

                    await observer.PostSent;
                }
            }

            [Test]
            public async Task Should_invoke_the_observer_prior_to_send()
            {
                var observer = new PublishObserver();
                using (Bus.ConnectPublishObserver(observer))
                {
                    await Bus.Publish(new PingMessage());

                    await observer.PreSent;
                }
            }

            [Test]
            public async Task Should_not_invoke_the_send_observer_prior_to_send()
            {
                var observer = new PublishObserver();
                using (Bus.ConnectPublishObserver(observer))
                {
                    var sendObserver = new SendObserver();
                    using (Bus.ConnectSendObserver(sendObserver))
                    {
                        await Bus.Publish(new PingMessage());

                        await observer.PreSent;

                        Assert.That(async () => await sendObserver.PreSent.OrTimeout(s:5), Throws.TypeOf<TimeoutException>());
                    }
                }
            }

            [Test]
            public async Task Should_not_invoke_post_sent_on_exception()
            {
                var observer = new PublishObserver();
                using (Bus.ConnectPublishObserver(observer))
                {
                    Assert.That(
                        async () => await Bus.Publish(new PingMessage(), Pipe.Execute<SendContext>(x => x.Serializer = null)),
                        Throws.TypeOf<SerializationException>());

                    await observer.SendFaulted;

                    observer.PostSent.Status.ShouldBe(TaskStatus.WaitingForActivation);
                }
            }
        }


        class PublishObserver :
            IPublishObserver
        {
            readonly TaskCompletionSource<PublishContext> _postSend;
            readonly TaskCompletionSource<PublishContext> _preSend;
            readonly TaskCompletionSource<PublishContext> _sendFaulted;

            public PublishObserver()
            {
                _sendFaulted = TaskUtil.GetTask<PublishContext>();
                _preSend = TaskUtil.GetTask<PublishContext>();
                _postSend = TaskUtil.GetTask<PublishContext>();
            }

            public Task<PublishContext> PreSent
            {
                get { return _preSend.Task; }
            }

            public Task<PublishContext> PostSent
            {
                get { return _postSend.Task; }
            }

            public Task<PublishContext> SendFaulted
            {
                get { return _sendFaulted.Task; }
            }

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


        [TestFixture]
        public class Connecting_a_send_observer_to_the_endpoint :
            RabbitMqTestFixture
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
                    Assert.That(
                        async () => await InputQueueSendEndpoint.Send(new PingMessage(), Pipe.Execute<SendContext>(x => x.Serializer = null)),
                        Throws.TypeOf<SerializationException>());

                    await observer.SendFaulted;

                    observer.PostSent.Status.ShouldBe(TaskStatus.WaitingForActivation);
                }
            }
        }
    }
}
