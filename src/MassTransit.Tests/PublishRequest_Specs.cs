// Copyright 2007-2011 Chris Patterson, Dru Sellers, Travis Smith, et. al.
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
    using System.Threading.Tasks;
    using Exceptions;
    using Magnum.Extensions;
    using Magnum.TestFramework;
    using NUnit.Framework;
    using RequestResponse;
    using TestFramework;
    using TextFixtures;

    [TestFixture]
    public class Publishing_a_simple_request :
        LoopbackLocalAndRemoteTestFixture
    {
        class PingMessage
        {
            public Guid TransactionId { get; set; }
        }

        class PongMessage
        {
            public Guid TransactionId { get; set; }
        }

        [Test]
        public void Should_support_send_as_well()
        {
            var pongReceived = new FutureMessage<PongMessage>();
            var pingReceived = new FutureMessage<PingMessage>();

            RemoteBus.SubscribeContextHandler<PingMessage>(x =>
                {
                    pingReceived.Set(x.Message);
                    x.Respond(new PongMessage {TransactionId = x.Message.TransactionId});
                });
            LocalBus.ShouldHaveSubscriptionFor<PingMessage>();

            var ping = new PingMessage();

            TimeSpan timeout = 8.Seconds();

            RemoteBus.Endpoint.SendRequest(ping, LocalBus, x =>
                {
                    x.Handle<PongMessage>(message =>
                        {
                            message.TransactionId.ShouldEqual(ping.TransactionId,
                                "The response correlationId did not match");
                            pongReceived.Set(message);
                        });

                    x.SetTimeout(timeout);
                });

            pingReceived.IsAvailable(timeout).ShouldBeTrue("The ping was not received");
            pongReceived.IsAvailable(timeout).ShouldBeTrue("The pong was not received");
        }

        [Test]
        public void Should_ignore_a_response_that_was_not_for_us()
        {
            var pongReceived = new FutureMessage<PongMessage>();
            var pingReceived = new FutureMessage<PingMessage>();
            var badResponse = new FutureMessage<PongMessage>();

            LocalBus.SubscribeHandler<PongMessage>(pongReceived.Set);

            RemoteBus.SubscribeContextHandler<PingMessage>(x =>
                {
                    RemoteBus.ShouldHaveRemoteSubscriptionFor<PongMessage>();

                    pingReceived.Set(x.Message);
                    RemoteBus.Publish(new PongMessage {TransactionId = x.Message.TransactionId});
                });
            LocalBus.ShouldHaveSubscriptionFor<PingMessage>();

            var ping = new PingMessage();

            TimeSpan timeout = 8.Seconds();

            Assert.Throws<RequestTimeoutException>(() =>
                {
                    RemoteBus.Endpoint.SendRequest(ping, LocalBus, x =>
                        {
                            x.Handle<PongMessage>(badResponse.Set);

                            x.SetTimeout(timeout);
                        });
                });

            pingReceived.IsAvailable(timeout).ShouldBeTrue("The ping was not received");
            pongReceived.IsAvailable(timeout).ShouldBeTrue("The pong was not received");
            badResponse.IsAvailable(2.Seconds()).ShouldBeFalse("Should not have received a response");
        }

        [Test, Category("NotOnTeamCity")]
        public void Should_support_the_asynchronous_programming_model()
        {
            var pongReceived = new FutureMessage<PongMessage>();
            var pingReceived = new FutureMessage<PingMessage>();
            var callbackCalled = new FutureMessage<IAsyncResult>();

            RemoteBus.SubscribeContextHandler<PingMessage>(x =>
                {
                    pingReceived.Set(x.Message);
                    x.Respond(new PongMessage {TransactionId = x.Message.TransactionId});
                });
            LocalBus.ShouldHaveSubscriptionFor<PingMessage>();

            var ping = new PingMessage();

            TimeSpan timeout = 18.Seconds();

            LocalBus.BeginPublishRequest(ping, callbackCalled.Set, null, x =>
                {
                    x.Handle<PongMessage>(message =>
                        {
                            message.TransactionId.ShouldEqual(ping.TransactionId,
                                "The response correlationId did not match");
                            pongReceived.Set(message);
                        });

                    x.SetTimeout(timeout);
                });

            pingReceived.IsAvailable(timeout).ShouldBeTrue("The ping was not received");
            pongReceived.IsAvailable(timeout).ShouldBeTrue("The pong was not received");

            callbackCalled.IsAvailable(timeout).ShouldBeTrue("The callback was not called");

            bool result = LocalBus.EndRequest<PingMessage>(callbackCalled.Message);

            Assert.IsTrue(result, "EndRequest should be true");
        }

#if NET40
        [Test, Category("NotOnTeamCity")]
        public void Should_support_the_async_tpl()
        {
            var pongReceived = new FutureMessage<PongMessage>();
            var pingReceived = new FutureMessage<PingMessage>();
            var continueCalled = new FutureMessage<Task<PongMessage>>();

            RemoteBus.SubscribeContextHandler<PingMessage>(x =>
            {
                pingReceived.Set(x.Message);
                x.Respond(new PongMessage { TransactionId = x.Message.TransactionId });
            });
            LocalBus.ShouldHaveSubscriptionFor<PingMessage>();

            var ping = new PingMessage();

            TimeSpan timeout = 18.Seconds();

            ITaskRequest<PingMessage> request = LocalBus.PublishRequestAsync(ping, x =>
            {
                x.SetTimeout(timeout);

                x.Handle<PongMessage>(message =>
                {
                    message.TransactionId.ShouldEqual(ping.TransactionId,
                        "The response correlationId did not match");
                    pongReceived.Set(message);
                })
                .ContinueWith(continueCalled.Set);
            });

            pingReceived.IsAvailable(timeout).ShouldBeTrue("The ping was not received");
            pongReceived.IsAvailable(timeout).ShouldBeTrue("The pong was not received");

            request.Task.Wait(timeout).ShouldBeTrue("Task was not completed");

            continueCalled.IsAvailable(timeout).ShouldBeTrue("The continuation was not called");
        }
#endif
        [Test]
        public void Should_throw_a_handler_exception_on_the_calling_thread()
        {
            var pongReceived = new FutureMessage<PongMessage>();
            var pingReceived = new FutureMessage<PingMessage>();

            RemoteBus.SubscribeContextHandler<PingMessage>(x =>
                {
                    pingReceived.Set(x.Message);
                    x.Respond(new PongMessage {TransactionId = x.Message.TransactionId});
                });
            LocalBus.ShouldHaveSubscriptionFor<PingMessage>();

            var ping = new PingMessage();

            TimeSpan timeout = 24.Seconds();

            var exception = Assert.Throws<RequestException>(() =>
                {
                    LocalBus.PublishRequest(ping, x =>
                        {
                            x.Handle<PongMessage>(message =>
                                {
                                    pongReceived.Set(message);

                                    throw new InvalidOperationException("I got it, but I am naughty with it.");
                                });

                            x.SetTimeout(timeout);
                        });
                }, "A request exception should have been thrown");

            exception.Response.ShouldBeAnInstanceOf<PongMessage>();
            exception.InnerException.ShouldBeAnInstanceOf<InvalidOperationException>();

            pingReceived.IsAvailable(timeout).ShouldBeTrue("The ping was not received");
            pongReceived.IsAvailable(timeout).ShouldBeTrue("The pong was not received");
        }

        [Test, Category("NotOnTeamCity")]
        public void Should_throw_a_handler_exception_on_the_calling_thread_using_async()
        {
            var pongReceived = new FutureMessage<PongMessage>();
            var pingReceived = new FutureMessage<PingMessage>();
            var callbackCalled = new FutureMessage<IAsyncResult>();

            RemoteBus.SubscribeContextHandler<PingMessage>(x =>
                {
                    pingReceived.Set(x.Message);
                    x.Respond(new PongMessage {TransactionId = x.Message.TransactionId});
                });
            LocalBus.ShouldHaveSubscriptionFor<PingMessage>();

            var ping = new PingMessage();

            TimeSpan timeout = 18.Seconds();

            LocalBus.BeginPublishRequest(ping, callbackCalled.Set, null, x =>
                {
                    x.Handle<PongMessage>(message =>
                        {
                            pongReceived.Set(message);

                            throw new InvalidOperationException("I got it, but I am naughty with it.");
                        });

                    x.SetTimeout(timeout);
                });

            pingReceived.IsAvailable(timeout).ShouldBeTrue("The ping was not received");
            pongReceived.IsAvailable(timeout).ShouldBeTrue("The pong was not received");

            callbackCalled.IsAvailable(timeout).ShouldBeTrue("Called was not called");

            var exception = Assert.Throws<RequestException>(() => { LocalBus.EndRequest<PingMessage>(callbackCalled.Message); },
                "A request exception should have been thrown");

            exception.Response.ShouldBeAnInstanceOf<PongMessage>();
            exception.InnerException.ShouldBeAnInstanceOf<InvalidOperationException>();
        }

        [Test, Category("NotOnTeamCity")]
        public void Should_throw_a_timeout_exception_for_async_when_end_is_called()
        {
            var pongReceived = new FutureMessage<PongMessage>();
            var pingReceived = new FutureMessage<PingMessage>();
            var callbackCalled = new FutureMessage<IAsyncResult>();

            RemoteBus.SubscribeHandler<PingMessage>(pingReceived.Set);
            LocalBus.ShouldHaveSubscriptionFor<PingMessage>();

            var ping = new PingMessage();

            TimeSpan timeout = 2.Seconds();

            LocalBus.BeginPublishRequest(ping, callbackCalled.Set, null, x =>
                {
                    x.Handle<PongMessage>(pongReceived.Set);

                    x.SetTimeout(timeout);
                });

            callbackCalled.IsAvailable(8.Seconds()).ShouldBeTrue("Callback was not invoked");

            Assert.Throws<RequestTimeoutException>(() => { LocalBus.EndRequest<PingMessage>(callbackCalled.Message); },
                "A timeout exception should have been thrown");

            pingReceived.IsAvailable(timeout).ShouldBeTrue("The ping was not received");
            pongReceived.IsAvailable(timeout).ShouldBeFalse("The pong should not have been received");
        }

        [Test]
        public void Should_throw_a_timeout_exception_if_no_response_received()
        {
            var pongReceived = new FutureMessage<PongMessage>();
            var pingReceived = new FutureMessage<PingMessage>();

            RemoteBus.SubscribeHandler<PingMessage>(pingReceived.Set);
            LocalBus.ShouldHaveSubscriptionFor<PingMessage>();

            var ping = new PingMessage();

            TimeSpan timeout = 2.Seconds();

            Assert.Throws<RequestTimeoutException>(() =>
                {
                    LocalBus.PublishRequest(ping, x =>
                        {
                            x.Handle<PongMessage>(pongReceived.Set);

                            x.SetTimeout(timeout);
                        });
                }, "A timeout exception should have been thrown");

            pingReceived.IsAvailable(timeout).ShouldBeTrue("The ping was not received");
            pongReceived.IsAvailable(timeout).ShouldBeFalse("The pong should not have been received");
        }

        [Test]
        public void Should_call_the_timeout_handler_and_not_throw_an_exception()
        {
            var pongReceived = new FutureMessage<PongMessage>();
            var pingReceived = new FutureMessage<PingMessage>();
            var timeoutCalled = new FutureMessage<bool>();

            RemoteBus.SubscribeHandler<PingMessage>(pingReceived.Set);
            LocalBus.ShouldHaveSubscriptionFor<PingMessage>();

            var ping = new PingMessage();

            TimeSpan timeout = 2.Seconds();

            LocalBus.PublishRequest(ping, x =>
                {
                    x.Handle<PongMessage>(pongReceived.Set);

                    x.HandleTimeout(timeout, () => timeoutCalled.Set(true));
                });

            pingReceived.IsAvailable(timeout).ShouldBeTrue("The ping was not received");
            pongReceived.IsAvailable(timeout).ShouldBeFalse("The pong should not have been received");
            timeoutCalled.IsAvailable(timeout).ShouldBeTrue("The timeout handler was not called");
        }

        [Test]
        public void Should_use_a_clean_syntax_following_standard_conventions()
        {
            var pongReceived = new FutureMessage<PongMessage>();
            var pingReceived = new FutureMessage<PingMessage>();

            RemoteBus.SubscribeContextHandler<PingMessage>(x =>
                {
                    pingReceived.Set(x.Message);
                    x.Respond(new PongMessage {TransactionId = x.Message.TransactionId});
                });
            LocalBus.ShouldHaveSubscriptionFor<PingMessage>();

            var ping = new PingMessage();

            TimeSpan timeout = 8.Seconds();

            LocalBus.PublishRequest(ping, x =>
                {
                    x.Handle<PongMessage>(message =>
                        {
                            message.TransactionId.ShouldEqual(ping.TransactionId,
                                "The response correlationId did not match");
                            pongReceived.Set(message);
                        });

                    x.SetTimeout(timeout);
                });

            pingReceived.IsAvailable(timeout).ShouldBeTrue("The ping was not received");
            pongReceived.IsAvailable(timeout).ShouldBeTrue("The pong was not received");
        }

        [Test]
        public void Should_allow_publish_request_more_than_once()
        {
            var pongReceived = new FutureMessage<PongMessage>();
            var pingReceived = new FutureMessage<PingMessage>();

            RemoteBus.SubscribeContextHandler<PingMessage>(x =>
            {
                pingReceived.Set(x.Message);
                x.Respond(new PongMessage { TransactionId = x.Message.TransactionId });
            });
            LocalBus.ShouldHaveSubscriptionFor<PingMessage>();

            var ping = new PingMessage();

            TimeSpan timeout = 8.Seconds();

            LocalBus.PublishRequest(ping, x =>
            {
                x.Handle<PongMessage>(message =>
                {
                    message.TransactionId.ShouldEqual(ping.TransactionId,
                        "The response correlationId did not match");
                    pongReceived.Set(message);
                });

                x.SetTimeout(timeout);
            });

            pingReceived.IsAvailable(timeout).ShouldBeTrue("The ping was not received");
            pongReceived.IsAvailable(timeout).ShouldBeTrue("The pong was not received");

            var secondPongReceived = new FutureMessage<PongMessage>();

            ping = new PingMessage();

            LocalBus.PublishRequest(ping, x =>
            {
                x.Handle<PongMessage>(message =>
                {
                    message.TransactionId.ShouldEqual(ping.TransactionId,
                        "The response correlationId did not match");
                    secondPongReceived.Set(message);
                });

                x.SetTimeout(timeout);
            });

            secondPongReceived.IsAvailable(timeout).ShouldBeTrue("The pong was not received");

        }
    }
}