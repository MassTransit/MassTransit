namespace MassTransit.Containers.Tests.Windsor_Tests
{
    using Castle.MicroKernel;
    using Castle.Windsor;
    using Common_Tests;
    using NUnit.Framework;
    using Scoping;


    [TestFixture]
    public class Windsor_ScopePublish :
        Common_ScopePublish<IKernel>
    {
        readonly IWindsorContainer _container;

        public Windsor_ScopePublish()
        {
            _container = new WindsorContainer();
            _container.AddMassTransit(x =>
            {
                x.AddBus(provider => BusControl);
            });
        }

        [OneTimeTearDown]
        public void Close_container()
        {
            _container.Dispose();
        }

        protected override IPublishScopeProvider GetPublishScopeProvider()
        {
            return _container.Resolve<IPublishScopeProvider>();
        }
    }
}
