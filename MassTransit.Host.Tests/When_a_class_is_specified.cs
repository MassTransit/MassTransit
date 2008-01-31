using System;
using System.Collections.Generic;
using MassTransit.ServiceBus;
using NUnit.Framework;
using NUnit.Framework.SyntaxHelpers;
using Rhino.Mocks;

namespace MassTransit.Host.Tests
{
    [TestFixture]
    public class When_a_class_is_specified
    {
        [Test]
        public void Find_the_members_that_handle_messages()
        {
            IClassSubscriptionManager csm = new ClassSubscriptionManager();

            IList<Type> messageTypes = csm.FindMessageTypes(typeof (ClassMessageHandler));

            Assert.That(messageTypes, Is.Not.Null);

            Assert.That(messageTypes.Count, Is.EqualTo(1));

            Assert.That(messageTypes[0] == typeof(SampleMessage));
        }

        [Test]
        public void The_message_handler_should_be_subscribed_to_the_service_bus()
        {
            IClassSubscriptionManager csm = new ClassSubscriptionManager();

            MockRepository mocks = new MockRepository();
            IServiceBus bus = mocks.CreateMock<IServiceBus>();

            using(mocks.Record())
            {
                Expect.Call(delegate { bus.Subscribe<SampleMessage>(null); }).IgnoreArguments();
            }

            using(mocks.Playback())
            {
                ClassMessageHandler handlerClass = new ClassMessageHandler();

                csm.SubscribeHandlers(bus, handlerClass);
            }

            
        }

    }

    public class ClassMessageHandler : IAutoSubscriber
    {
        public void SampleMessageHandler(IMessageContext<SampleMessage> context)
        {
        }

        public void AddSubscriptions(IServiceBus bus)
        {
            bus.Subscribe<SampleMessage>(SampleMessageHandler);
        }

        public void RemoveSubscriptions(IServiceBus bus)
        {
            bus.Unsubscribe<SampleMessage>(SampleMessageHandler);
        }
    }

    public class SampleMessage :
        IMessage
    {
    }
}