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
namespace MassTransit.Tests
{
    using System;
    using System.Collections.Generic;
    using System.Threading;
    using NUnit.Framework;
    using NUnit.Framework.SyntaxHelpers;
    using ServiceBus;
    using ServiceBus.Internal;
    using ServiceBus.Subscriptions;
    using ServiceBus.Tests;
    using ServiceBus.Tests.Messages;
    using ServiceBus.Transports;

    [TestFixture]
    public class MessageGroup_Specs :
        Specification
    {
        [Test]
        public void I_should_be_able_to_join_a_bunch_of_messages_into_a_group()
        {
            object[] items = new object[] {new PingMessage(), new PongMessage()};

            MessageGroup group = MessageGroup.Join(items);

            Assert.That(group.Count, Is.EqualTo(2));

            Assert.That(group[0], Is.TypeOf(typeof (PingMessage)));
            Assert.That(group[1], Is.TypeOf(typeof (PongMessage)));
        }

        [Test]
        public void I_should_be_able_to_retrieve_a_single_message_by_position()
        {
            PingMessage ping = new PingMessage();
            PongMessage pong = new PongMessage();

            MessageGroup group = MessageGroup.Build<MessageGroup>()
                .Add(ping)
                .Add(pong);

            PingMessage thePing = group.Get<PingMessage>(0);
        }

        [Test]
        public void I_should_be_able_to_split_a_bunch_of_messages_from_a_group()
        {
            PingMessage ping = new PingMessage();
            PongMessage pong = new PongMessage();

            MessageGroup group = MessageGroup.Build<MessageGroup>()
                .Add(ping)
                .Add(pong);

            object[] items = group.ToArray();

            Assert.That(items.Length, Is.EqualTo(2));
        }

        [Test]
        public void I_should_be_able_to_split_the_group_into_individual_messages_and_handle_each_one_on_its_own()
        {
            IServiceBus bus = DynamicMock<IServiceBus>();

            PingMessage ping = new PingMessage();
            PongMessage pong = new PongMessage();

            MessageGroup group = MessageGroup.Build<MessageGroup>()
                .Add(ping)
                .Add(pong);

            using (Record())
            {
                bus.Publish(ping);
                bus.Publish(pong);
            }

            using (Playback())
            {
                group.Split(bus);
            }
        }

        [Test, ExpectedException(typeof (ArgumentException))]
        public void I_should_get_an_exception_when_I_try_to_get_an_unmatched_type()
        {
            PingMessage ping = new PingMessage();
            PongMessage pong = new PongMessage();

            MessageGroup group = MessageGroup.Build<MessageGroup>()
                .Add(ping)
                .Add(pong);

            PingMessage thePing = group.Get<PingMessage>(1);
        }

        [Test]
        public void One()
        {
            IServiceBus bus = DynamicMock<IServiceBus>();

            PingMessage ping = new PingMessage();
            PongMessage pong = new PongMessage();

            MessageGroup group = MessageGroup.Build<MessageGroup>()
                .Add(ping)
                .Add(pong);

            Assert.That(group.Count, Is.EqualTo(2));

            Assert.That(group[0], Is.TypeOf(typeof (PingMessage)));
            Assert.That(group[1], Is.TypeOf(typeof (PongMessage)));

            bus.Publish(group);
        }
    }

    [TestFixture]
    public class When_a_custom_message_group_is_defined : Specification
    {
        private IServiceBus _bus;
        private IEndpoint _endpoint;
        private IObjectBuilder _builder;
        private ISubscriptionCache _cache;
        private IEndpointResolver _resolver;

        protected override void Before_each()
        {
            _resolver = new EndpointResolver();
            EndpointResolver.AddTransport(typeof (LoopbackEndpoint));

            _endpoint = _resolver.Resolve(new Uri("loopback://localhost/test"));

            _builder = Stub<IObjectBuilder>();
            _cache = new LocalSubscriptionCache();
            _bus = new ServiceBus(_endpoint, _builder, _cache, _resolver, new TypeInfoCache());
        }

        internal class Consumer : Consumes<SpecialGroup>.All, Consumes<PingMessage>.All, Consumes<PongMessage>.All
        {
            private readonly IServiceBus _bus;

            private readonly ManualResetEvent _gotPing = new ManualResetEvent(false);
            private readonly ManualResetEvent _gotPong = new ManualResetEvent(false);
            private readonly ManualResetEvent _received = new ManualResetEvent(false);

            public Consumer(IServiceBus bus)
            {
                _bus = bus;
            }

            public ManualResetEvent Received
            {
                get { return _received; }
            }

            public ManualResetEvent GotPing
            {
                get { return _gotPing; }
            }

            public ManualResetEvent GotPong
            {
                get { return _gotPong; }
            }

            public void Consume(PingMessage message)
            {
                _gotPing.Set();
            }

            public void Consume(PongMessage message)
            {
                _gotPong.Set();
            }

            public void Consume(SpecialGroup message)
            {
                _received.Set();

                if (message.SplitOnConsume)
                    message.Split(_bus);
            }
        }

        [Serializable]
        [AllowMessageType(typeof (PingMessage), typeof (PongMessage))]
        public class SpecialGroup : MessageGroup
        {
            private bool _splitOnConsume;

            protected SpecialGroup()
            {
            }

            public SpecialGroup(List<object> messages) :
                base(messages)
            {
            }

            public bool SplitOnConsume
            {
                get { return _splitOnConsume; }
                set { _splitOnConsume = value; }
            }
        }

        [Test]
        public void I_should_be_able_to_split_a_group_of_messages_into_parts()
        {
            Consumer c = new Consumer(_bus);

            _bus.Subscribe(c);

            SpecialGroup group = MessageGroup.Build<SpecialGroup>()
                .Add(new PingMessage())
                .Add(new PongMessage());

            group.SplitOnConsume = true;

            _bus.Publish(group);

            Assert.That(c.Received.WaitOne(TimeSpan.FromSeconds(3), true), Is.True, "No message received by consumer");
            Assert.That(c.GotPing.WaitOne(TimeSpan.FromSeconds(3), true), Is.True, "No ping received by consumer");
            Assert.That(c.GotPong.WaitOne(TimeSpan.FromSeconds(3), true), Is.True, "No pong received by consumer");
        }

        [Test]
        public void I_Should_be_able_to_subscribe()
        {
            Consumer c = new Consumer(_bus);

            _bus.Subscribe(c);

            SpecialGroup group = MessageGroup.Build<SpecialGroup>()
                .Add(new PingMessage())
                .Add(new PongMessage());

            _bus.Publish(group);

            Assert.That(c.Received.WaitOne(TimeSpan.FromSeconds(5), true), Is.True, "No message received by consumer");
        }

        [Test, ExpectedException(typeof (ArgumentException))]
        public void I_should_only_be_allowed_to_add_valid_message_types()
        {
            SpecialGroup group = MessageGroup.Build<SpecialGroup>()
                .Add(new PingMessage())
                .Add(new PongMessage())
                .Add(new UpdateMessage());
        }
    }
}