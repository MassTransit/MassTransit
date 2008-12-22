// Copyright 2007-2008 The Apache Software Foundation.
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
namespace MassTransit.Transports.Msmq.Tests
{
    using System;
    using System.Collections;
    using System.Threading;
    using MassTransit.Tests;
    using MassTransit.Tests.Messages;
    using MassTransit.Tests.TestConsumers;
    using NUnit.Framework;
    using NUnit.Framework.SyntaxHelpers;
    using Rhino.Mocks;

    [TestFixture]
    public class As_a_ServiceBus_working_with_an_endpoint
    {
        [Test]
        public void When_an_error_is_thrown_on_publish_the_bus_will_not_rollback_any_messages()
        {
            //publish ignores transactional semantics
        }

        [Test]
        public void When_an_error_is_encountered_while_publishing_only_the_errorendpoint_is_skipped()
        {
            
        }

        [Test]
        public void When_an_error_is_encountered_on_read_only_the_errorconsumers_get_skipped()
        {
            
        }
    }
    [TestFixture]
    public class When_a_message_is_published_to_a_transactional_queue :
        LocalAndRemoteTestContext
    {
        protected override string GetCastleConfigurationFile()
        {
            return "transactional.castle.xml";
        }

        private readonly TimeSpan _timeout = TimeSpan.FromSeconds(10);

        [Test]
        public void It_should_be_received_by_one_subscribed_consumer()
        {
            var consumer = new TestMessageConsumer<PingMessage>();
            RemoteBus.Subscribe(consumer);

            var message = new PingMessage();
            LocalBus.Publish(message);

            consumer.ShouldHaveReceivedMessage(message, _timeout);
        }

        [Test]
        public void It_should_leave_the_message_in_the_queue_if_an_exception_is_thrown()
        {
            RemoteBus.Subscribe<PingMessage>(m => { throw new ApplicationException("Boing!"); });

            var message = new PingMessage();
            LocalBus.Publish(message);
        }

        [Test]
        public void It_should_rollback_a_send_if_an_exception_is_thrown()
        {
            var consumer = new TestMessageConsumer<PongMessage>();
            LocalBus.Subscribe(consumer);

            var message = new PingMessage();
            var response = new PongMessage(message.CorrelationId);

            RemoteBus.Subscribe<PingMessage>(m =>
            {
                RemoteBus.Publish(response);
                throw new ApplicationException("Boing!");
            });

            LocalBus.Publish(message);

            consumer.ShouldNotHaveReceivedMessage(response, _timeout);
        }
    }

    [TestFixture]
    public class When_publishing_a_message
    {

        [Test]
        [Ignore]
        public void Multiple_Local_Services_Should_Be_Available()
        {
            using (QueueTestContext qtc = new QueueTestContext(MockRepository.GenerateMock<IObjectBuilder>()))
            {
                ManualResetEvent _updateEvent = new ManualResetEvent(false);

                qtc.ServiceBus.Subscribe<UpdateMessage>(
                    delegate { _updateEvent.Set(); });

                ManualResetEvent _deleteEvent = new ManualResetEvent(false);

                qtc.ServiceBus.Subscribe<DeleteMessage>(
                    delegate { _deleteEvent.Set(); });

                DeleteMessage dm = new DeleteMessage();

                qtc.ServiceBus.Publish(dm);

                UpdateMessage um = new UpdateMessage();

                qtc.ServiceBus.Publish(um);

                Assert.That(_deleteEvent.WaitOne(TimeSpan.FromSeconds(4), true), Is.True,
                            "Timeout expired waiting for message");

                Assert.That(_updateEvent.WaitOne(TimeSpan.FromSeconds(4), true), Is.True,
                            "Timeout expired waiting for message");
            }
        }

        [Test]
        [Ignore]
        public void Multiple_messages_should_be_delivered_to_the_appropriate_remote_subscribers()
        {
            using (QueueTestContext qtc = new QueueTestContext(MockRepository.GenerateMock<IObjectBuilder>()))
            {
                ManualResetEvent _updateEvent = new ManualResetEvent(false);

                qtc.RemoteServiceBus.Subscribe<UpdateMessage>(
                    delegate { _updateEvent.Set(); });

                ManualResetEvent _deleteEvent = new ManualResetEvent(false);

                qtc.RemoteServiceBus.Subscribe<DeleteMessage>(
                    delegate { _deleteEvent.Set(); });

                DeleteMessage dm = new DeleteMessage();

                qtc.ServiceBus.Publish(dm);

                UpdateMessage um = new UpdateMessage();

                qtc.ServiceBus.Publish(um);

                Assert.That(_deleteEvent.WaitOne(TimeSpan.FromSeconds(6), true), Is.True,
                            "Timeout expired waiting for message");

                Assert.That(_updateEvent.WaitOne(TimeSpan.FromSeconds(6), true), Is.True,
                            "Timeout expired waiting for message");
            }
        }

        [Test]
        [Ignore]
        public void The_message_should_be_delivered_to_a_local_subscriber()
        {
            using (QueueTestContext qtc = new QueueTestContext(MockRepository.GenerateMock<IObjectBuilder>()))
            {
                ManualResetEvent _updateEvent = new ManualResetEvent(false);

                qtc.ServiceBus.Subscribe<UpdateMessage>(
                    delegate { _updateEvent.Set(); });

                UpdateMessage um = new UpdateMessage();

                qtc.ServiceBus.Publish(um);

                Assert.That(_updateEvent.WaitOne(TimeSpan.FromSeconds(3), true), Is.True,
                            "Timeout expired waiting for message");
            }
        }

        [Test]
        [Ignore]
        public void The_message_should_be_delivered_to_a_remote_subscriber()
        {
            using (QueueTestContext qtc = new QueueTestContext(MockRepository.GenerateMock<IObjectBuilder>()))
            {
                ManualResetEvent _updateEvent = new ManualResetEvent(false);

                qtc.RemoteServiceBus.Subscribe<UpdateMessage>(
                    delegate { _updateEvent.Set(); });

                UpdateMessage um = new UpdateMessage();

                qtc.ServiceBus.Publish(um);

                Assert.That(_updateEvent.WaitOne(TimeSpan.FromSeconds(3), true), Is.True,
                            "Timeout expired waiting for message");
            }
        }

        [Test]
        [Ignore]
        public void The_message_should_be_delivered_to_a_remote_subscriber_with_a_reply()
        {
            IObjectBuilder obj = MockRepository.GenerateMock<IObjectBuilder>();
            SetupResult.For(obj.GetInstance<IEndpoint>(new Hashtable())).Return(MockRepository.GenerateMock<IEndpoint>());

            using (QueueTestContext qtc = new QueueTestContext(obj))
            {
                ManualResetEvent _updateEvent = new ManualResetEvent(false);

                Action<UpdateMessage> handler =
                    msg =>
                    {
                        _updateEvent.Set();

                        qtc.RemoteServiceBus.Publish(new UpdateAcceptedMessage());
                    };

                ManualResetEvent _repliedEvent = new ManualResetEvent(false);

                qtc.RemoteServiceBus.Subscribe(handler);

                qtc.ServiceBus.Subscribe<UpdateAcceptedMessage>(
                    delegate { _repliedEvent.Set(); });

                UpdateMessage um = new UpdateMessage();

                qtc.ServiceBus.Publish(um);

                Assert.That(_updateEvent.WaitOne(TimeSpan.FromSeconds(3), true), Is.True,
                            "Timeout expired waiting for message");

                Assert.That(_repliedEvent.WaitOne(TimeSpan.FromSeconds(3), true), Is.True, "NO response message received");
            }
        }
    }
}