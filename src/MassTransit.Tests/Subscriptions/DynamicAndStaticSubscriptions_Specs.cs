namespace MassTransit.Tests.Subscriptions
{
    using System.Linq;
    using Magnum.Extensions;
    using Magnum.TestFramework;
    using NUnit.Framework;
    using TextFixtures;
    using MassTransit.Testing;

    [TestFixture]
    public class When_a_static_and_dynamic_subscription_are_on_the_same_bus :
        LoopbackLocalAndRemoteTestFixture
    {
        FutureMessage<A> _receivedA;

        public When_a_static_and_dynamic_subscription_are_on_the_same_bus()
        {
            _receivedA = new FutureMessage<A>();
        }

        protected override void ConfigureRemoteBus(BusConfigurators.ServiceBusConfigurator configurator)
        {
            base.ConfigureRemoteBus(configurator);

            configurator.Subscribe(x =>
                {
                    x.Handler<A>((context, message) => _receivedA.Set(message));
                });
        }

        protected override void EstablishContext()
        {
            base.EstablishContext();

            LocalBus.HasSubscription<A>(8.Seconds()).Any().ShouldBeTrue("No subscription found on local bus");
        }

        [Test]
        public void Dynamic_should_receive_the_message()
        {
            var receivedDynamic = new FutureMessage<A>();

            UnsubscribeAction subscription = RemoteBus.SubscribeHandler<A>(receivedDynamic.Set);
            using (subscription.Disposable())
            {
                LocalBus.Publish(new A());

                receivedDynamic.IsAvailable(8.Seconds()).ShouldBeTrue("Dynamic not received");
                _receivedA.IsAvailable(8.Seconds()).ShouldBeTrue("Static not received");
            }
        }

        [Test]
        public void Static_should_remain_after_dynamic_removed()
        {
            var receivedDynamic = new FutureMessage<A>();

            UnsubscribeAction subscription = RemoteBus.SubscribeHandler<A>(receivedDynamic.Set);
            using (subscription.Disposable())
            {
            }

            LocalBus.Publish(new A());

            receivedDynamic.IsAvailable(8.Seconds()).ShouldBeFalse("Dynamic should not have received");
            _receivedA.IsAvailable(8.Seconds()).ShouldBeTrue("Static not received");
        }

        [Test]
        public void Static_should_receive_the_message()
        {
            LocalBus.Publish(new A());

            _receivedA.IsAvailable(8.Seconds()).ShouldBeTrue();
        }

        class A
        {
            public string Value { get; set; }
        }
    }
}
