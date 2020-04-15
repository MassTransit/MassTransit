namespace MassTransit.Containers.Tests.Autofac_Tests
{
    using Autofac;
    using Common_Tests;
    using NUnit.Framework;
    using Scoping;


    [TestFixture]
    public class Autofac_ScopePublish :
        Common_ScopePublish<ILifetimeScope>
    {
        readonly IContainer _container;

        public Autofac_ScopePublish()
        {
            var builder = new ContainerBuilder();
            builder.AddMassTransit(x =>
            {
                x.AddBus(provider => BusControl);
            });

            _container = builder.Build();
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
