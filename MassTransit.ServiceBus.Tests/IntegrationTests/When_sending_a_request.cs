using System;
using System.Threading;
using NUnit.Framework;
using NUnit.Framework.SyntaxHelpers;

namespace MassTransit.ServiceBus.Tests
{
    [TestFixture]
    public class When_sending_a_request
    {
        [Test]
        public void A_response_should_release_the_waiting_process()
        {
            using (QueueTestContext qtc = new QueueTestContext())
            {
                PingMessage ping = new PingMessage();

                qtc.ServiceBus.Subscribe<PingMessage>(
                    delegate(MessageContext<PingMessage> context) { context.Reply(new PongMessage()); });

                IServiceBusAsyncResult asyncResult = qtc.ServiceBus.Request(qtc.ServiceBus.Endpoint, ping);

                Assert.That(asyncResult, Is.Not.Null);

                Assert.That(asyncResult.AsyncWaitHandle.WaitOne(TimeSpan.FromSeconds(10), true), Is.True,
                    "Timeout Expired Waiting For Response");

                Assert.That(asyncResult.Messages, Is.Not.Null);

                Assert.That(asyncResult.Messages, Is.Not.Empty);

                PongMessage pong = asyncResult.Messages[0] as PongMessage;

                Assert.That(pong, Is.Not.Null);
            }
        }

        [Test]
        public void The_callback_function_should_be_called_when_a_response_is_received()
        {
            using (QueueTestContext qtc = new QueueTestContext())
            {
                PingMessage ping = new PingMessage();

                qtc.ServiceBus.Subscribe<PingMessage>(
                    delegate(MessageContext<PingMessage> context) { context.Reply(new PongMessage()); });

                ManualResetEvent _called = new ManualResetEvent(false);

                IServiceBusAsyncResult asyncResult = qtc.ServiceBus.Request(qtc.ServiceBus.Endpoint,
                    delegate(IAsyncResult ar)
                        {
                            IServiceBusAsyncResult sbar = ar as IServiceBusAsyncResult;
                            Assert.That(sbar, Is.Not.Null);

                            Assert.That(sbar.Messages, Is.Not.Null);

                            Assert.That(sbar.Messages, Is.Not.Empty);

                            PongMessage pong = sbar.Messages[0] as PongMessage;

                            Assert.That(pong, Is.Not.Null);

                            _called.Set();
                        },
                    27,
                    ping);

                Assert.That(asyncResult, Is.Not.Null);

                Assert.That(_called.WaitOne(TimeSpan.FromSeconds(10), true), Is.True,
                    "Timeout Expired Waiting For Response");
            }
        }
    }
}