using System;
using System.Collections.Generic;
using MassTransit.ServiceBus.Subscriptions.Messages;
using NUnit.Framework;
using Rhino.Mocks;

namespace MassTransit.ServiceBus.SubscriptionsManager.Tests
{
    using Subscriptions;
    using Util;

    [TestFixture]
    public class SubscriptionServiceBusTests
    {
        private IMessageReceiver mockReceiver;
        private MockRepository mocks;
        private ISubscriptionStorage ss;
        private Uri subsUri;
        private IEndpoint ep;

        [SetUp]
        public void SetUp()
        {
            mocks = new MockRepository();
            mockReceiver = mocks.CreateMock<IMessageReceiver>();
            ss = mocks.CreateMock<ISubscriptionStorage>();
            subsUri = new Uri("msmq://localhost/subs");
            ep = mocks.CreateMock<IEndpoint>();
        }

        

        [Test]
        public void How_does_the_ServiceBus_start_up()
        {
            using(mocks.Record())
            {
                Expect.Call(ep.Uri).Return(subsUri);
                ss.Add(typeof(RequestCacheUpdate).FullName, subsUri);
                Expect.Call(ep.Receiver).Return(mockReceiver);
                mockReceiver.Subscribe(null);
                LastCall.IgnoreArguments();

                Expect.Call(ep.Uri).Return(subsUri);
                ss.Add(typeof(SubscriptionChange).FullName, subsUri);
                Expect.Call(ep.Receiver).Return(mockReceiver);
                mockReceiver.Subscribe(null);
                LastCall.IgnoreArguments();

                Expect.Call(ep.Uri).Return(subsUri);
                ss.Add(typeof(RequestCacheUpdateForMessage).FullName, subsUri);
                Expect.Call(ep.Receiver).Return(mockReceiver);
                mockReceiver.Subscribe(null);
                LastCall.IgnoreArguments();
            }
            using(mocks.Playback())
            {
                new SubscriptionServiceBus(ep, ss);
            }
        }

        [Test]
        public void What_happens_when_a_message_is_received()
        {
            ISubscriptionRepository sr = mocks.CreateMock<ISubscriptionRepository>();
            IEndpoint returnEndpoint = mocks.CreateMock<IEndpoint>();
            IEnvelope env = mocks.CreateMock<IEnvelope>();
            IMessageSender mockSender = mocks.CreateMock<IMessageSender>();

            string envId = Guid.NewGuid().ToString() + "\\1";

            using (mocks.Record())
            {
                Expect.Call(ep.Uri).Return(subsUri);
                ss.Add(typeof(RequestCacheUpdate).FullName, subsUri);
                Expect.Call(ep.Receiver).Return(mockReceiver);
                mockReceiver.Subscribe(null);
                LastCall.IgnoreArguments();

                Expect.Call(ep.Uri).Return(subsUri);
                ss.Add(typeof(SubscriptionChange).FullName, subsUri);
                Expect.Call(ep.Receiver).Return(mockReceiver);
                mockReceiver.Subscribe(null);
                LastCall.IgnoreArguments();

                Expect.Call(ep.Uri).Return(subsUri);
                ss.Add(typeof(RequestCacheUpdateForMessage).FullName, subsUri);
                Expect.Call(ep.Receiver).Return(mockReceiver);
                mockReceiver.Subscribe(null);
                LastCall.IgnoreArguments();



                Expect.Call(env.CorrelationId).Return(MessageId.Empty);
                Expect.Call(env.Messages).Return(new IMessage[] {new RequestCacheUpdate()});
                Expect.Call(env.ReturnEndpoint).Return(returnEndpoint);
                Expect.Call(returnEndpoint.Uri).Return(new Uri("msmq://localhost/return"));
                ss.Add(typeof(CacheUpdateResponse).FullName, new Uri("msmq://localhost/return"));
                Expect.Call(ss.List()).Return(new List<Subscription>());
                Expect.Call(env.ReturnEndpoint).Return(returnEndpoint);
                Expect.Call(env.Id).Return(envId);
                Expect.Call(returnEndpoint.Sender).Return(mockSender);
                mockSender.Send(null);
                LastCall.IgnoreArguments();
            }
            using (mocks.Playback())
            {
                SubscriptionServiceBus bus = new SubscriptionServiceBus(ep, ss);               
                bus.Deliver(env);
            }
        }
    }
}
