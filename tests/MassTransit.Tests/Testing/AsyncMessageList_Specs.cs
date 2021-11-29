namespace MassTransit.Tests.Testing
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using AsyncMessageListTestTypes;
    using Context;
    using MassTransit.Testing;
    using NUnit.Framework;


    namespace AsyncMessageListTestTypes
    {
        class A
        {
        }


        class B
        {
        }
    }


    public class AsyncMessageList_Specs
    {
        [Test]
        public async Task Should_complete_now()
        {
            var messageList = new SentMessageList(TimeSpan.FromMilliseconds(10));

            messageList.Add(new MessageSendContext<A>(new A()));

            Assert.That(await messageList.Any<A>(), Is.True);
        }

        [Test]
        public async Task Should_complete_now_using_select_any()
        {
            var messageList = new SentMessageList(TimeSpan.FromMilliseconds(10));

            messageList.Add(new MessageSendContext<A>(new A()));

            Assert.That(await messageList.SelectAsync<A>().Any(), Is.True);
        }

        [Test]
        public async Task Should_complete_now_with_a()
        {
            var messageList = new SentMessageList(TimeSpan.FromMilliseconds(10));

            messageList.Add(new MessageSendContext<A>(new A()));

            Assert.That(await messageList.Any(x => x.Includes.Add<A>().Add<B>()), Is.True);
        }

        [Test]
        public async Task Should_complete_now_with_b()
        {
            var messageList = new SentMessageList(TimeSpan.FromMilliseconds(10));

            messageList.Add(new MessageSendContext<B>(new B()));

            Assert.That(await messageList.Any(x => x.Includes.Add<A>().Add<B>()), Is.True);
        }

        [Test]
        public async Task Should_not_complete_with_b()
        {
            var messageList = new SentMessageList(TimeSpan.FromMilliseconds(10));

            messageList.Add(new MessageSendContext<B>(new B()));

            Assert.That(await messageList.Any(x => x.Excludes.Add<B>()), Is.False);
        }

        [Test]
        public async Task Should_complete_now_with_either_a_or_b()
        {
            var messageList = new SentMessageList(TimeSpan.FromMilliseconds(10));

            messageList.Add(new MessageSendContext<B>(new B()));

            Assert.That(await messageList.Any(m => m switch
            {
                (A a, _) => true,
                (B b, _) => true,
                _ => false
            }), Is.True);
        }

        [Test]
        public async Task Should_complete_later()
        {
            var messageList = new SentMessageList(TimeSpan.FromSeconds(10));

        #pragma warning disable 4014
            Task.Run(async () =>
        #pragma warning restore 4014
            {
                await Task.Delay(1000);

                messageList.Add(new MessageSendContext<A>(new A()));
            });

            Assert.That(await messageList.Any<A>(), Is.True);
        }

        [Test]
        public async Task Should_complete_when_cancelled()
        {
            using var token = new CancellationTokenSource(TimeSpan.FromSeconds(1));

            var messageList = new SentMessageList(TimeSpan.FromSeconds(10), token.Token);

            Assert.That(await messageList.SelectAsync<A>().Count(), Is.EqualTo(0));
        }

        [Test]
        public async Task Should_return_false_on_timeout()
        {
            var messageList = new SentMessageList(TimeSpan.FromMilliseconds(10));

            Assert.That(await messageList.Any<A>(), Is.False);
        }
    }
}
