namespace MassTransit.Tests.Reactive.Samples
{
    using System;
    using System.Linq;
    using MassTransit.Transports;
    using Messages;
    using NUnit.Framework;
    using MassTransit.Reactive;
    using TestFramework;

    [Scenario]
    public class BasicExample :
        Given_a_standalone_service_bus
    {
        [Given]
        public void A_Rx_Query_Is_Setup()
        {

            obs = from c in LocalBus.AsObservable<PingMessage>()
                    let observered = true
                    select c;
        }

        IObservable<PingMessage> obs;
        bool _observed;

        [When]
        public void When_a_message_is_published()
        {
            LocalBus.Publish(new PingMessage());
        }

        [Then]
        public void Then_One_Message_should_be_observed()
        {
            Assert.AreEqual(1, obs.Count());
            Assert.IsTrue(_observed);
        }
    }
}