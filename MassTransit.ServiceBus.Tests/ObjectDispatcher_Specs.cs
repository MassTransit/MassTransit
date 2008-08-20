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
namespace MassTransit.ServiceBus.Tests
{
    using System;
    using System.Collections;
    using MassTransit.ServiceBus.Internal;
    using MassTransit.ServiceBus.Subscriptions;
    using NUnit.Framework;
    using NUnit.Framework.SyntaxHelpers;
    using Rhino.Mocks;

    [TestFixture]
    public class When_a_type_is_registered_with_the_dispatcher :
        Specification
    {
        private IObjectBuilder builder;
        private IServiceBus bus;
        private ISubscriptionCache cache;
        private MessageTypeDispatcher dispatcher;
        private TypeInfoCache typeInfoCache;
        private DispatcherContext context;
        private RequestHandler rh;
        private SelectiveHandler sh;
        private IEndpoint endpoint;

        protected override void Before_each()
        {
            builder = DynamicMock<IObjectBuilder>();
            endpoint = DynamicMock<IEndpoint>();
            SetupResult.For(endpoint.Uri).Return(new Uri("looopback://localhost/test"));

            bus = DynamicMock<IServiceBus>();
            SetupResult.For(bus.Endpoint).Return(endpoint);

            cache = DynamicMock<ISubscriptionCache>();

            dispatcher = new MessageTypeDispatcher();
            typeInfoCache = new TypeInfoCache();

            context = new DispatcherContext(builder, bus, dispatcher, cache, typeInfoCache);

            rh = new RequestHandler();
            sh = new SelectiveHandler();

            Hashtable t = new Hashtable();

            SetupResult.For(builder.Build<RequestHandler>()).IgnoreArguments().Return(rh);
            SetupResult.For(builder.Build<RequestHandler>(t)).IgnoreArguments().Return(rh);
            SetupResult.For(builder.Build<SelectiveHandler>()).IgnoreArguments().Return(sh);
            SetupResult.For(builder.Build<SelectiveHandler>(t)).IgnoreArguments().Return(sh);

            ReplayAll();
        }

        internal class RequestHandler : Consumes<TestMessage>.All
        {
            private static int _value;

            public static int Value
            {
                get { return _value; }
            }

            public void Consume(TestMessage message)
            {
                _value = message.Value;
            }
        }

        internal class SelectiveHandler : Consumes<TestMessage>.Selected
        {
            private static int _value;

            public static int Value
            {
                get { return _value; }
            }

            public bool Accept(TestMessage message)
            {
                return message.Value > 27;
            }

            public void Consume(TestMessage message)
            {
                _value = message.Value;
            }
        }

        internal class TestMessage
        {
            private readonly int _value;

            public TestMessage(int value)
            {
                _value = value;
            }

            public int Value
            {
                get { return _value; }
            }
        }

        [Test]
        public void A_new_object_should_be_created_to_handle_each_message()
        {
            typeInfoCache.GetSubscriptionTypeInfo<RequestHandler>().AddComponent(context);
            typeInfoCache.GetSubscriptionTypeInfo<SelectiveHandler>().AddComponent(context);

            TestMessage message = new TestMessage(27);

            dispatcher.Consume(message);

            Assert.That(RequestHandler.Value, Is.EqualTo(27));
            Assert.That(SelectiveHandler.Value, Is.EqualTo(default(int)));
        }

        [Test]
        public void A_new_object_should_be_created_to_handle_each_message_including_selective_ones()
        {
            typeInfoCache.GetSubscriptionTypeInfo<RequestHandler>().AddComponent(context);
            typeInfoCache.GetSubscriptionTypeInfo<SelectiveHandler>().AddComponent(context);

            TestMessage message = new TestMessage(42);

            dispatcher.Consume(message);

            Assert.That(RequestHandler.Value, Is.EqualTo(42));
            Assert.That(SelectiveHandler.Value, Is.EqualTo(42));
        }
    }
}