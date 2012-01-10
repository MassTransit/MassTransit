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
        FutureMessage<B> _receivedB;
        FutureMessage<C> _receivedC;

        public When_a_static_and_dynamic_subscription_are_on_the_same_bus()
        {
            _receivedA = new FutureMessage<A>();
            _receivedB = new FutureMessage<B>();
            _receivedC = new FutureMessage<C>();
        }

        protected override void ConfigureRemoteBus(BusConfigurators.ServiceBusConfigurator configurator)
        {
            base.ConfigureRemoteBus(configurator);

            configurator.Subscribe(x =>
                {
                    x.Handler<A>((context, message) => _receivedA.Set(message));
                    x.Handler<B>((context, message) => _receivedB.Set(message));
                    x.Handler<C>((context, message) => _receivedC.Set(message));
                });
        }

        protected override void EstablishContext()
        {
            base.EstablishContext();

            LocalBus.HasSubscription<A>(8.Seconds()).Any().ShouldBeTrue("No subscription A found on local bus");
            LocalBus.HasSubscription<B>(8.Seconds()).Any().ShouldBeTrue("No subscription B found on local bus");
            LocalBus.HasSubscription<C>(8.Seconds()).Any().ShouldBeTrue("No subscription C found on local bus");
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

            var result = subscription();

            result.ShouldBeFalse("Should still have a static subscription");

            LocalBus.Publish(new A());

            receivedDynamic.IsAvailable(8.Seconds()).ShouldBeFalse("Dynamic should not have received");
            _receivedA.IsAvailable(8.Seconds()).ShouldBeTrue("Static not received");
        }

        [Test]
        public void Adding_many_dynamic_and_removing_should_retain_dynamics()
        {
            var dynamicA = new FutureMessage<A>();
            var dynamicB = new FutureMessage<B>();
            var dynamicC = new FutureMessage<C>();
            var dynamicD = new FutureMessage<D>();

            UnsubscribeAction subscriptionA = RemoteBus.SubscribeHandler<A>(dynamicA.Set);
            UnsubscribeAction subscriptionB = RemoteBus.SubscribeHandler<B>(dynamicB.Set);
            UnsubscribeAction subscriptionC = RemoteBus.SubscribeHandler<C>(dynamicC.Set);
            UnsubscribeAction subscriptionD = RemoteBus.SubscribeHandler<D>(dynamicD.Set);

            LocalBus.HasSubscription<D>(8.Seconds()).Any().ShouldBeTrue("No D subscription");
            try
            {
                subscriptionA().ShouldBeFalse("A static not remaining");
                subscriptionB().ShouldBeFalse("B static not remaining");
                subscriptionC().ShouldBeFalse("C static not remaining");

                LocalBus.Publish(new D());

                dynamicD.IsAvailable(8.Seconds()).ShouldBeTrue("D should have been received");
            }
            finally
            {
                subscriptionD();
            }
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
        class B
        {
            public string Value { get; set; }
        }
        class C
        {
            public string Value { get; set; }
        }
        class D
        {
            public string Value { get; set; }
        }
    }
}
