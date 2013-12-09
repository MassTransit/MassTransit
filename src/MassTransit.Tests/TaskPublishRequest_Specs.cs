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
namespace MassTransit.Tests
{
    using System;
    using System.Linq;
    using System.Threading.Tasks;
    using BusConfigurators;
    using Exceptions;
    using Magnum.Extensions;
    using Magnum.TestFramework;
    using NUnit.Framework;
    using TestFramework;
    using TextFixtures;

#if NET40
    [TestFixture]
    public class Publishing_request_using_the_task_parallel_library :
        LoopbackLocalAndRemoteTestFixture
    {
        [Test]
        public void Should_call_timeout_callback_if_timeout_occurs()
        {
            var ping = new Messages.PingMessage();
            ITaskRequest<Messages.PingMessage> request = LocalBus.PublishRequestAsync(ping, x =>
                {
                    //
                    x.SetTimeout(1.Seconds());
                });

            var aggregateException = Assert.Throws<AggregateException>(() => request.Task.Wait(8.Seconds()));

            Assert.IsInstanceOf<RequestTimeoutException>(aggregateException.InnerExceptions.First());
        }

        [Test]
        public void Should_call_timeout_callback_if_timeout_occurs_and_not_fault()
        {
            var continueCalled = new FutureMessage<PingMessage>();

            var ping = new PingMessage();
            ITaskRequest<PingMessage> request = LocalBus.PublishRequestAsync(ping, x =>
                {
                    //
                    x.HandleTimeout(1.Seconds(), continueCalled.Set);
                });

            request.Task.Wait(8.Seconds()).ShouldBeTrue("Should have completed successfully");

            continueCalled.IsAvailable(8.Seconds()).ShouldBeTrue("The timeout continuation was not called");
        }

        [Test]
        public void Should_complete_all_related_tasks()
        {
            var pongReceived = new FutureMessage<PongMessage>();
            var continueCalled = new FutureMessage<Task<PongMessage>>();

            TimeSpan timeout = 8.Seconds();

            var ping = new PingMessage();
            ITaskRequest<PingMessage> request = LocalBus.PublishRequestAsync(ping, x =>
                {
                    x.SetTimeout(4.Seconds());

                    x.Handle<PongMessage>(pongReceived.Set)
                        .ContinueWith(continueCalled.Set);
                });

            pongReceived.IsAvailable(timeout).ShouldBeTrue("The pong was not received");

            request.Task.Wait(timeout).ShouldBeTrue("Task was not completed");

            request.GetResponseTask<PongMessage>().Wait(timeout).ShouldBeTrue("The response task was not completed");

            continueCalled.IsAvailable(timeout).ShouldBeTrue("The continuation was not called");
        }

        [Test]
        public void Should_not_complete_timeout_if_handler_completes()
        {
            var pongReceived = new FutureMessage<PongMessage>();
            var continueCalled = new FutureMessage<Task<PongMessage>>();
            var timeoutCalled = new FutureMessage<PingMessage>();

            TimeSpan timeout = 8.Seconds();

            var ping = new PingMessage();
            ITaskRequest<PingMessage> request = LocalBus.PublishRequestAsync(ping, x =>
                {
                    x.HandleTimeout(4.Seconds(), timeoutCalled.Set);

                    x.Handle<PongMessage>(pongReceived.Set)
                        .ContinueWith(continueCalled.Set);
                });

            pongReceived.IsAvailable(timeout).ShouldBeTrue("The pong was not received");

            request.Task.Wait(timeout).ShouldBeTrue("Task was not completed");

            request.GetResponseTask<PongMessage>().Wait(timeout).ShouldBeTrue("The response task was not completed");

            continueCalled.IsAvailable(timeout).ShouldBeTrue("The continuation was not called");

            timeoutCalled.IsAvailable(2.Seconds()).ShouldBeFalse("The timeout should not have been called");
        }

        [Test]
        public void Should_throw_an_exception_from_the_timeout()
        {
            var ping = new PingMessage();
            ITaskRequest<PingMessage> request = LocalBus.PublishRequestAsync(ping, x => { x.SetTimeout(1.Seconds()); });

            var aggregateException = Assert.Throws<AggregateException>(() => request.Task.Wait(8.Seconds()));

            Assert.IsInstanceOf<RequestTimeoutException>(aggregateException.InnerExceptions.First());
        }

        FutureMessage<PingMessage> _pingReceived;

        protected override void EstablishContext()
        {
            base.EstablishContext();

            LocalBus.ShouldHaveRemoteSubscriptionFor<PingMessage>();

            _pingReceived = new FutureMessage<PingMessage>();
        }

        protected override void ConfigureRemoteBus(ServiceBusConfigurator configurator)
        {
            base.ConfigureRemoteBus(configurator);

            configurator.Subscribe(x =>
                {
                    x.Handler<PingMessage>((context, message) =>
                        {
                            _pingReceived.Set(message);
                            context.Respond(new PongMessage {TransactionId = message.TransactionId});
                        });
                });
        }

        class PingMessage
        {
            public Guid TransactionId { get; set; }
        }

        class PongMessage
        {
            public Guid TransactionId { get; set; }
        }
    }

    [TestFixture]
    public class With_no_message_being_handled :
        LoopbackTestFixture
    {
        [Test]
        public void Should_call_timeout_callback_if_timeout_occurs()
        {
            var pongCompleted = new FutureMessage<PongMessage>();
            var pongCancelled = new FutureMessage<bool>();

            Task<PongMessage> pongTask;

            var ping = new PingMessage();
            ITaskRequest<PingMessage> request = LocalBus.PublishRequestAsync(ping, x =>
                {
                    x.SetTimeout(1.Seconds());

                    pongTask = x.Handle<PongMessage>(message => { });
                    pongTask.ContinueWith(t => pongCompleted.Set(t.Result), TaskContinuationOptions.OnlyOnRanToCompletion);
                    pongTask.ContinueWith((Task t) => pongCancelled.Set(t.IsCanceled), TaskContinuationOptions.OnlyOnCanceled);
                });

            var aggregateException = Assert.Throws<AggregateException>(() => request.Task.Wait(8.Seconds()));

            Assert.IsInstanceOf<RequestTimeoutException>(aggregateException.InnerExceptions.First());

            pongCompleted.IsAvailable(1.Seconds()).ShouldBeFalse("We only asked to be notified on success");
            
            pongCancelled.IsAvailable(1.Seconds()).ShouldBeTrue("We like to know we were cancelled due to timeout");

        }

        FutureMessage<PingMessage> _pingReceived;

        protected override void EstablishContext()
        {
            base.EstablishContext();

            _pingReceived = new FutureMessage<PingMessage>();
        }

        class PingMessage
        {
            public Guid TransactionId { get; set; }
        }

        class PongMessage
        {
            public Guid TransactionId { get; set; }
        }
    }
#endif
}