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
    using Messages;
    using NUnit.Framework;
    using NUnit.Framework.SyntaxHelpers;
    using Rhino.Mocks;

    [TestFixture]
    public class When_publishing_a_message
    {

        MockRepository _mocks = new MockRepository();

        [Test]
        [Ignore]
        public void Multiple_Local_Services_Should_Be_Available()
        {
            using (QueueTestContext qtc = new QueueTestContext(_mocks.DynamicMock<IObjectBuilder>()))
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
            using (QueueTestContext qtc = new QueueTestContext(_mocks.DynamicMock<IObjectBuilder>()))
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
            using (QueueTestContext qtc = new QueueTestContext(_mocks.DynamicMock<IObjectBuilder>()))
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
            using (QueueTestContext qtc = new QueueTestContext(_mocks.DynamicMock<IObjectBuilder>()))
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
            IObjectBuilder obj = _mocks.DynamicMock<IObjectBuilder>();
            SetupResult.For(obj.GetInstance<IEndpoint>(new Hashtable())).Return(_mocks.DynamicMock<IEndpoint>());
            _mocks.ReplayAll();

            using (QueueTestContext qtc = new QueueTestContext(obj))
            {
                ManualResetEvent _updateEvent = new ManualResetEvent(false);

                Action<IMessageContext<UpdateMessage>> handler =
                    delegate(IMessageContext<UpdateMessage> ctx)
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