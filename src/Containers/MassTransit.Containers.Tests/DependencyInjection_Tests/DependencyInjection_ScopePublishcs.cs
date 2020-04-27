namespace MassTransit.Containers.Tests.DependencyInjection_Tests
{
    using System;
    using Common_Tests;
    using Microsoft.Extensions.DependencyInjection;
    using NUnit.Framework;


    [TestFixture]
    public class DependencyInjection_ScopePublish :
        Common_ScopePublish<IServiceProvider>
    {
        readonly IServiceProvider _provider;
        readonly IServiceScope _childContainer;

        public DependencyInjection_ScopePublish()
        {
            var collection = new ServiceCollection();
            collection.AddMassTransit(x =>
            {
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

        protected override IPublishEndpoint GetPublishEndpoint()
        {
            return _childContainer.ServiceProvider.GetRequiredService<IPublishEndpoint>();
        }

        protected override void AssertScopesAreEqual(IServiceProvider actual)
        {
            Assert.AreEqual(_childContainer.ServiceProvider, actual);
        }
    }
}
