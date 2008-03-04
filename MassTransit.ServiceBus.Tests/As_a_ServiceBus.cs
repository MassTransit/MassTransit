namespace MassTransit.ServiceBus.Tests
{
    using System;
    using System.Collections.Generic;
    using Internal;
    using MassTransit.ServiceBus.Subscriptions;
    using NUnit.Framework;
    using Rhino.Mocks;

    [TestFixture]
    public class As_a_ServiceBus
    {
        private MockRepository mocks;
        private ISubscriptionStorage mockSubscriptionStorage;
        private IEndpoint mockEndpoint;
        private IEndpoint mockSendEndpoint;
        private IMessageSender mockSender;

        [SetUp]
        public void SetUp()
        {
            mocks = new MockRepository();
            mockSubscriptionStorage = mocks.CreateMock<ISubscriptionStorage>();
            mockEndpoint = mocks.CreateMock<IEndpoint>();
            mockSendEndpoint = mocks.CreateMock<IEndpoint>();
            mockSender = mocks.CreateMock<IMessageSender>();

            SetupResult.For(mockEndpoint.Uri).Return(new Uri("msmq://localhost/test"));
        }

        [TearDown]
        public void TearDown()
        {
            mocks = null;
            mockEndpoint = null;
            mockSendEndpoint = null;
            mockSubscriptionStorage = null;
            mockSender = null;
        }

        [Test]
        public void When_Sending_a_message()
        {
            using (mocks.Record())
            {
                Expect.Call(mockSendEndpoint.Sender).Return(mockSender);
                mockSender.Send(null);
                LastCall.IgnoreArguments();
            }
            using (mocks.Playback())
            {
                ServiceBus bus = new ServiceBus(mockEndpoint, mockSubscriptionStorage);
                bus.Send(mockSendEndpoint, new PingMessage());
            }
        }

        [Test]
        public void When_Sending_2_messages()
        {
            using (mocks.Record())
            {
                Expect.Call(mockSendEndpoint.Sender).Return(mockSender).Repeat.Twice();
                Expect.Call(delegate { mockSender.Send(null); }).IgnoreArguments().Repeat.Twice();
            }
            using (mocks.Playback())
            {
                ServiceBus bus = new ServiceBus(mockEndpoint, mockSubscriptionStorage);
                bus.Send(mockSendEndpoint, new PingMessage(), new PingMessage());
            }
        }

        [Test, Ignore("Endpoint cache causing some pain")]
        public void When_Publishing_a_message()
        {
            Subscription sub =
                new Subscription("MassTransit.ServiceBus.Tests.PingMessage", new Uri("msmq://localhost/subscriber"));
            List<Subscription> subs = new List<Subscription>();
            subs.Add(sub);

            using (mocks.Record())
            {
                Expect.Call(mockSendEndpoint.Uri).Return(new Uri("msmq://localhost/send"));
                Expect.Call(mockSendEndpoint.Uri).Return(new Uri("msmq://localhost/send"));

                Expect.Call(mockSubscriptionStorage.List("MassTransit.ServiceBus.Tests.PingMessage")).Return(subs);

                //Expect.Call(mockSendEndpoint.Sender).Return(mockSender);
                //mockSender.Send(null);
                //LastCall.IgnoreArguments();
            }
            using (mocks.Playback())
            {
                ServiceBus bus = new ServiceBus(mockEndpoint, mockSubscriptionStorage);
                bus.Publish(new PingMessage());
            }
        }
    }
}