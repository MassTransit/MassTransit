namespace MassTransit.Tests
{
    namespace ObserverTests
    {
        using System;
        using System.Runtime.Serialization;
        using System.Threading.Tasks;
        using Internals;
        using NUnit.Framework;
        using Shouldly;
        using TestFramework;
        using TestFramework.Messages;
        using Util;


        [TestFixture]
        public class Connecting_to_the_publish_observer_bus :
            InMemoryTestFixture
        {
            [Test]
            public async Task Should_invoke_the_exception_after_send_failure()
            {
                var observer = new Observer();
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
                var observer = new Observer();
                using (Bus.ConnectPublishObserver(observer))
                {
                    await Bus.Publish(new PingMessage());

                    await observer.PostSent;
                }
            }

            [Test]
            public async Task Should_invoke_the_observer_prior_to_send()
            {
                var observer = new Observer();
                using (Bus.ConnectPublishObserver(observer))
                {
                    await Bus.Publish(new PingMessage());

                    await observer.PreSent;
                }
            }

            [Test]
            public async Task Should_not_invoke_post_sent_on_exception()
            {
                var observer = new Observer();
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
                var observer = new Observer();
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


            class Observer :
                IPublishObserver
            {
                readonly TaskCompletionSource<PublishContext> _postSend;
                readonly TaskCompletionSource<PublishContext> _preSend;
                readonly TaskCompletionSource<PublishContext> _sendFaulted;

                public Observer()
                {
                    _sendFaulted = TaskUtil.GetTask<PublishContext>();
                    _preSend = TaskUtil.GetTask<PublishContext>();
                    _postSend = TaskUtil.GetTask<PublishContext>();
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
        }
    }
}
