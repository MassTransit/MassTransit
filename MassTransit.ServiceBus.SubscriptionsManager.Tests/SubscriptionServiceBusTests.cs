namespace MassTransit.ServiceBus.SubscriptionsManager.Tests
{
    using System;
    using System.Collections.Generic;
    using Messages;
    using NUnit.Framework;
    using Rhino.Mocks;
    using Rhino.Mocks.Interfaces;

    [TestFixture]
    public class SubscriptionServiceBusTests
    {
        [Test]
        public void How_does_the_ServiceBus_start_up()
        {
            MockRepository  mocks = new MockRepository();
            ISubscriptionRepository sr = mocks.CreateMock<ISubscriptionRepository>();
            ISubscriptionStorage ss = mocks.CreateMock<ISubscriptionStorage>();
            IEndpoint ep = mocks.CreateMock<IEndpoint>();

            using(mocks.Record())
            {
                ep.EnvelopeReceived += delegate { };
                LastCall.IgnoreArguments();
                ss.Add(null, ep);
                LastCall.IgnoreArguments().Repeat.Times(3);
            }
            using(mocks.Playback())
            {
                SubscriptionServiceBus bus = new SubscriptionServiceBus(ep, ss, sr);
            }
        }

        [Test]
        public void What_happens_when_a_message_is_received()
        {
            MockRepository mocks = new MockRepository();
            ISubscriptionRepository sr = mocks.CreateMock<ISubscriptionRepository>();
            ISubscriptionStorage ss = mocks.CreateMock<ISubscriptionStorage>();
            IEndpoint ep = mocks.CreateMock<IEndpoint>();
            IEndpoint returnEndpoint = mocks.CreateMock<IEndpoint>();
            IEventRaiser evt;
            IEnvelope env = mocks.CreateMock<IEnvelope>();
            string envId = Guid.NewGuid().ToString();

            using (mocks.Record())
            {
                ep.EnvelopeReceived += null;
                
                evt = LastCall.IgnoreArguments().GetEventRaiser();
                ss.Add(null, ep);
                LastCall.IgnoreArguments().Repeat.Times(3);
                Expect.Call(env.Id).Return(envId);
                Expect.Call(env.Messages).Return(new IMessage[] {new RequestCacheUpdate()});
                Expect.Call(env.Messages).Return(new IMessage[] {new RequestCacheUpdate()});
                Expect.Call(env.Id).Return(envId);
                ep.AcceptEnvelope(envId);
                Expect.Call(sr.List()).Return(new List<Subscription>());
                Expect.Call(env.ReturnTo).Return(returnEndpoint);
            }
            using (mocks.Playback())
            {
                SubscriptionServiceBus bus = new SubscriptionServiceBus(ep, ss, sr);
                evt.Raise(null, new EnvelopeReceivedEventArgs(env));
                
            }
        }
    }
}
