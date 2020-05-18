namespace MassTransit.Containers.Tests.DependencyInjection_Tests
{
    using System;
    using Common_Tests;
    using Microsoft.Extensions.DependencyInjection;
    using NUnit.Framework;
    using Scenarios;
    using TestFramework.Messages;


    [TestFixture]
    public class DependencyInjection_ScopeRequestClient :
        Common_ScopeRequestClient<IServiceProvider>
    {
        readonly IServiceProvider _provider;
        readonly IServiceScope _childContainer;

        public DependencyInjection_ScopeRequestClient()
        {
            var collection = new ServiceCollection();
            collection.AddMassTransit(x =>
            {
                x.AddRequestClient<SimpleMessageClass>(InputQueueAddress);
                x.AddRequestClient<PingMessage>();
                x.AddBus(provider => BusControl);
            });

            _provider = collection.BuildServiceProvider(true);
            _childContainer = _provider.CreateScope();
        }

        [OneTimeTearDown]
        public void Close_container()
        {
            _childContainer.Dispose();
        }

        protected override IRequestClient<SimpleMessageClass> GetSendRequestClient()
        {
            return _childContainer.ServiceProvider.GetRequiredService<IRequestClient<SimpleMessageClass>>();
        }

        protected override IRequestClient<PingMessage> GetPublishRequestClient()
        {
            return _childContainer.ServiceProvider.GetRequiredService<IRequestClient<PingMessage>>();
        }

        protected override void AssertScopesAreEqual(IServiceProvider actual)
        {
            Assert.AreEqual(_childContainer.ServiceProvider, actual);
        }
    }
}
