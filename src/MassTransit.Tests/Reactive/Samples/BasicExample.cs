namespace MassTransit.Tests.Reactive.Samples
{
    using System;
    using System.Linq;
    using MassTransit.Reactive;
    using Messages;
    using NUnit.Framework;
    using TestFramework;

    [Scenario]
    public class BasicExample :
        Given_a_standalone_service_bus
    {
        [Given]
        public void A_Rx_Query_Is_Setup()
        {
            obs = LocalBus.AsObservable<PingMessage>();

            obs.Subscribe(m => _observed = true);
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
            Assert.AreEqual(1, obs.Take(1).ToEnumerable().Count());
            Assert.IsTrue(_observed);
        }
    }
}