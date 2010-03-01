namespace MassTransit.Tests.Reactive
{
    using System;
    using MassTransit.Reactive;
    using Messages;
    using NUnit.Framework;
    using Rhino.Mocks;

    [TestFixture]
    public class ServiceBusSubscriptionSpecs
    {
        [Test]
        public void WhenUnsubscribedIsCalledShouldCallAction()
        {
            var bus = MockRepository.GenerateStub<IServiceBus>();
            var obs = MockRepository.GenerateStub<IObserver<PingMessage>>();
            var disposed = false;
            bus.Stub(b => b.Subscribe<PingMessage>(obs.OnNext)).Return(() => disposed = true);

            var sbs = new ServiceBusSubscription<PingMessage>(bus, obs, null);
            sbs.Dispose();

            Assert.IsTrue(disposed);

        }
    }
}