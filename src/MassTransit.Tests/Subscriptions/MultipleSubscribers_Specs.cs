using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MassTransit.Tests.TextFixtures;
using MassTransit.TestFramework;
using MassTransit.BusConfigurators;
using NUnit.Framework;
using Magnum.TestFramework;
using Magnum;
using Magnum.Extensions;

namespace MassTransit.Tests.Subscriptions
{
    [TestFixture]
    public class when_multiple_subscribers_to_same_message
        : LoopbackLocalAndRemoteTestFixture
    {
        protected override void EstablishContext()
        {
            base.EstablishContext();

            RemoteBus.ShouldHaveSubscriptionFor<MyMessage>();

            LocalBus.Publish(new MyMessage());
        }

        private List<Tuple<string, MyMessage>> receivedMessages = new List<Tuple<string, MyMessage>>();


        protected override void ConfigureRemoteBus(ServiceBusConfigurator configurator)
        {
            base.ConfigureLocalBus(configurator);

            configurator.Subscribe(cf =>
            {
                cf.Handler<MyMessage>(message => receivedMessages.Add(new Tuple<string, MyMessage>("One", message)));
                cf.Handler<MyMessage>(message => receivedMessages.Add(new Tuple<string, MyMessage>("Two", message)));
            });
        }


        [Test]
        public void each_subscriber_should_only_receive_once()
        {

            ThreadUtil.Sleep(4.Seconds());

            var byReceiver = receivedMessages.GroupBy(r => r.Item1);
            byReceiver.All(g => g.Count() == 1).ShouldBeTrue();
        }
 

    }

    public class MyMessage
    { 

    }

    
}
