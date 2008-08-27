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
namespace MassTransit.ServiceBus.Tests.Subscriptions
{
    using System;
    using MassTransit.ServiceBus.Internal;
    using MassTransit.ServiceBus.Subscriptions;
    using Messages;
    using NUnit.Framework;
    using NUnit.Framework.SyntaxHelpers;
    using Rhino.Mocks;
    using Transports;

    [TestFixture]
    public class When_a_handler_unsubscribes_from_the_service_bus
        : Specification
    {
        private IObjectBuilder _builder;

        protected override void Before_each()
        {
            _endpointResolver = new EndpointResolver();
            EndpointResolver.AddTransport(typeof (LoopbackEndpoint));

            _builder = StrictMock<IObjectBuilder>();
            _endpoint = _endpointResolver.Resolve(_endpointUri);

            _cache = _mocks.DynamicMock<ISubscriptionCache>();

            _bus = new ServiceBus(_endpoint, _builder, _cache, _endpointResolver);
        }


        private MockRepository _mocks = new MockRepository();
        private IEndpoint _endpoint;
        private ISubscriptionCache _cache;
        private ServiceBus _bus;
        private object _message = new PingMessage();
        private Uri _endpointUri = new Uri("loopback://localhost/test");
        private EndpointResolver _endpointResolver;


        private static void HandleAllMessages(IMessageContext<PingMessage> ctx)
        {
        }

        private static bool HandleSomeMessagesPredicate(PingMessage message)
        {
            return true;
        }

        [Test]
        public void The_service_bus_should_continue_to_handle_messages_if_at_least_one_handler_is_available()
        {
            using (_mocks.Record())
            {
                Expect.Call(delegate { _cache.Add(null); }).IgnoreArguments();

                Expect.Call(delegate { _cache.Add(null); }).IgnoreArguments();

                Expect.Call(delegate { _cache.Remove(null); }).IgnoreArguments();
                //        Expect.Call(delegate { _cache.Remove(null); }).IgnoreArguments();
            }

            using (_mocks.Playback())
            {
                _bus.Subscribe<PingMessage>(HandleAllMessages);
                Assert.That(_bus.Accept(_message), Is.True);

                _bus.Subscribe<PingMessage>(HandleAllMessages, HandleSomeMessagesPredicate);
                Assert.That(_bus.Accept(_message), Is.True);

                _bus.Unsubscribe<PingMessage>(HandleAllMessages);
                Assert.That(_bus.Accept(_message), Is.True);

                _bus.Unsubscribe<PingMessage>(HandleAllMessages, HandleSomeMessagesPredicate);
                Assert.That(_bus.Accept(_message), Is.False);
            }
        }

        [Test]
        public void The_service_bus_should_no_longer_show_the_message_type_as_handled()
        {
            using (_mocks.Record())
            {
                Expect.Call(delegate { _cache.Add(null); }).IgnoreArguments();

                Expect.Call(delegate { _cache.Remove(null); }).IgnoreArguments();
            }

            using (_mocks.Playback())
            {
                _bus.Subscribe<PingMessage>(HandleAllMessages);
                Assert.That(_bus.Accept(_message), Is.True);

                _bus.Unsubscribe<PingMessage>(HandleAllMessages);
                Assert.That(_bus.Accept(_message), Is.False);
            }
        }
    }
}