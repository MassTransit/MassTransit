namespace MassTransit.Containers.Tests.Windsor_Tests
{
    using System;
    using Castle.MicroKernel;
    using Castle.MicroKernel.Lifestyle;
    using Castle.Windsor;
    using Common_Tests;
    using NUnit.Framework;


    [TestFixture]
    public class Windsor_ScopePublish :
        Common_ScopePublish<IKernel>
    {
        readonly IWindsorContainer _container;
        readonly IDisposable _childContainer;

        public Windsor_ScopePublish()
        {
            _container = new WindsorContainer();
            _container.AddMassTransit(x =>
            {
                x.AddBus(provider => BusControl);
            });
            _childContainer = _container.BeginScope();
        }

        [OneTimeTearDown]
        public void Close_container()
        {
            _childContainer.Dispose();
            _container.Dispose();
        }

        protected override IPublishEndpoint GetPublishEndpoint()
        {
            return _container.Resolve<IPublishEndpoint>();
        }
        protected override void AssertScopesAreEqual(IKernel actual)
        {
            Assert.AreEqual(_container.Kernel, actual);
        }
    }
}
